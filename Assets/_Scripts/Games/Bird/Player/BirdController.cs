using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using Utility;

public class BirdController : Singleton<BirdController>
{
    public enum FlyState
    {
        Normal = 0,
        Dashing = 1,
        SpeedUP = 2,
        Dead = 8,
    }
    [Header("状态持续时间")]
    [SerializeField] float dashDuration = 3f;
    float dashCount = 0;
    float freezeCount = 0;

    [Header("活动范围限制")]
    [SerializeField]float maxMoveHeight = 0;
    [SerializeField]float minMoveHeight = 0;
    [Header("移动键位")]
    [SerializeField]KeyCode moveDownKey = KeyCode.S;
    [SerializeField]KeyCode moveUpKey = KeyCode.W;
    [SerializeField]KeyCode speedUpKey = KeyCode.D;
    [SerializeField]KeyCode dashKey = KeyCode.Space;
    [Header("状态")]
    FlyState playerState = FlyState.Normal;
    [Header("移动速度")]
    [SerializeField] float BirdSpeed = 0;
    [Header("接口")]
    BarrierController controller;
    [Header("死亡动画时间")]
    [SerializeField] float deathTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        controller = BarrierController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerState)
        {
            case FlyState.Normal:  { HandleNormal(); break; }
            case FlyState.SpeedUP: { HandleSpeedUp(); break; }
            case FlyState.Dashing: { HandleDash(); break; }
            case FlyState.Dead: { OnDead(); break; }
        }

        if (playerState != FlyState.Dead)
        {
            if (Input.GetKeyDown(moveDownKey)) { HandleMoveDown();}
            if (Input.GetKeyDown(moveUpKey)) HandleMoveUp();
        }
    }

    void HandleMoveDown()
    {
        if (transform.position.y < minMoveHeight) transform.position = new Vector2(transform.position.x, minMoveHeight);
        else
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - BirdSpeed * Time.deltaTime);
        }
    }

    void HandleMoveUp()
    {
        if (transform.position.y > maxMoveHeight) transform.position = new Vector2(transform.position.x, maxMoveHeight);
        else
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + BirdSpeed * Time.deltaTime);
        }
    }

    void HandleNormal()
    {
        if (Input.GetKeyDown(dashKey)) { StartDash(); }
        if (Input.GetKeyDown(speedUpKey)) { HandleSpeedUp(); }
    }

    void HandleSpeedUp()
    {
        playerState = FlyState.SpeedUP;
        controller.UpdateSpeed(playerState);

        if (!Input.GetKeyDown(speedUpKey)) EndSpeedUP();
    }

    void EndSpeedUP()
    {
        playerState = FlyState.Normal;
        controller.UpdateSpeed(playerState);
    }

    void StartDash()
    {
        playerState = FlyState.Dashing;
        controller.UpdateSpeed(playerState);
    }

    void HandleDash()
    {
        dashCount += Time.deltaTime;
        if (dashCount >= dashDuration) EndDash(); 
    }

    void EndDash()
    {
        playerState = FlyState.Normal;
        controller.UpdateSpeed(playerState);
    }

    void OnDead()
    {
        playerState = FlyState.Dead;
        controller.UpdateSpeed(playerState);
        StartCoroutine(DeathCoroutine());
        BirdGameController.Instance.Failure();
    }

    IEnumerator DeathCoroutine()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        while (renderer.color.a > 0)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a - (1f / deathTime) * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Barrier>()) OnDead();
    }
}

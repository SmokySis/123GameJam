using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public enum FlyState
    {
        Normal = 0,
        Dashing = 1,
        HurtFreeze = 2,
        Dead = 4,
    }
    [Header("状态持续时间")]
    [SerializeField] float dashDuration = 3f;
    [SerializeField] float hurtFreeze = 2f;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerState)
        {
            case FlyState.Normal:
                {
                    if (Input.GetKeyDown(dashKey)) { StartDash(); break; }
                    if (Input.GetKeyDown(moveDownKey)) { HandleMoveDown(); break; }
                    if (Input.GetKeyDown(moveUpKey)) HandleMoveUp();
                    break;
                }
            case FlyState.Dashing:
                {
                    if (dashCount >= dashDuration) { EndDash(); break; }
                    HandleDash();
                    break;
                }
            case FlyState.HurtFreeze:
                {
                    if (freezeCount >= hurtFreeze) { EndDamage(); break; }
                    OnDamage();
                    break;
                }
            case FlyState.Dead:
                {
                    OnDead();
                    break;
                }
        }





        if (playerState != FlyState.Normal) return;

       
    }

    void HandleMoveDown()
    {

    }

    void HandleMoveUp()
    {

    }

    void HandleSpeedUp()
    {

    }

    void StartDash()
    {
        playerState = FlyState.Dashing;
    }

    void HandleDash()
    {
        dashCount += Time.deltaTime;
    }

    void EndDash()
    {
        playerState = FlyState.Normal;
    }

    void OnDamage()
    {

    }

    void HandleDamage()
    {
        freezeCount += Time.deltaTime;
    }

    void EndDamage()
    {

    }

    void OnDead()
    {

    }
}

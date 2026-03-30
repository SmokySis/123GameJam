using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum DashDir
    {
        right = 0,
        left = 1,
        up = 2,
    }

    DashDir dashDir;

    public enum PlayerStates
    {
        stay = 0,
        move = 1,
        jump = 2,
        fall = 3,
        dash = 4,
    }

    public enum TimeStates
    {
        Normal = 0,
        Freeze = 1,
    }

    public static PlayerController instance;
    public static GameObject Player;
    public static Rigidbody2D rb;
    Vector3 MousePosition;
    [SerializeField] Camera camera;
    [SerializeField] GameObject LineDrawer;
    [Header("跳跃相关")]
    private float lastJumpInputTime = -10f;
    private float jumpBufferTime = 0.2f;
    [SerializeField] bool canSecondJump = true;
    public static PlayerStates PlayerState;
    [SerializeField] PlayerStates mornitoringState;
    TimeStates TimeState;
    [Header("落地检测相关")]
    bool isScalePlayed = false;
    float onGroundCount = 0;
    public Vector3 groundCheckPoint;
    public float checkRadius = 0.1f;
    public LayerMask groundLayer = 1 << 6;
    public int groundCheckPoints = 3;
    [SerializeField] bool isOnGround;
    [Header("冲刺相关")]
    bool canDash = true;
    bool isFallDashing = false;
    bool isStartDashing = false;
    float dashCount = 0;
    bool isDashing = false;
    float dashRecoverCount = 3;
    float dashRecover = 0;
    float scaleTimeCount = 0;
    bool EndCorotine = false; // 强制终止冲刺协程

    [Header("冲刺粒子特效")]
    public ParticleSystem dashTrailParticles;
    private ParticleSystem.EmissionModule emission;

    


    [Header("速度相关")]
    public static float HorizentalSpeed;
    public static float VerticalSpeed;
    public static float SlideSpeed;
    public static float weaponSpeed;

    [Header("下落相关")]
    public static float fallingAddForce;
  
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Initicialize();
        PlayerState = PlayerStates.stay;
        mornitoringState = PlayerState;
        TimeState = TimeStates.Normal;
    }

    void Update()
    {
        if(camera == null)
        {
            camera = Camera.main;
        }

        if (TimeState == TimeStates.Normal && !UIController.Instance.isPaused)
        {

            // Dash 时间计时（在 Update 中累计）
            groundCheckPoint = new Vector3(transform.position.x, transform.position.y - 0.5F * GetComponent<Renderer>().bounds.size.y, transform.position.z);
            if (isStartDashing)
            {
                dashCount += Time.deltaTime;
                if (dashCount >= PlayerData.Instance.DashTime)
                {
                    EndDash();
                    dashCount = 0;
                    isStartDashing = false;
                }
            }

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                Player.transform.localScale = Vector3.one;
            }
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                Player.transform.localScale = new Vector3(-1, 1, 1);
            }

            if (Input.GetButtonDown("Jump") ||
                (Input.GetAxisRaw("Vertical") > 0 && Input.GetButton("Jump")))
            {
                lastJumpInputTime = Time.time;
            }

            MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mornitoringState = PlayerState;

            switch (PlayerState)
            {
                case PlayerStates.stay:
                    SwitchStay();
                    break;
                case PlayerStates.move:
                    SwitchMove();
                    break;
                case PlayerStates.jump:
                    SwitchJump();
                    break;
                case PlayerStates.fall:
                    SwitchFall();
                    break;
                case PlayerStates.dash:
                    // Dash 逻辑移到 FixedUpdate
                break;
            }

            if (!isOnGround && canSecondJump && !isDashing)
            {
                bool jumpPressed = (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W));
                if (jumpPressed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, VerticalSpeed * 0.9f);
                    canSecondJump = false;
                    PlayerState = PlayerStates.jump;
                }
            }

            if (Input.GetMouseButtonDown(1) || (Input.GetKey(KeyCode.LeftShift) && canDash && !isDashing))
            {
                PlayerState = PlayerStates.dash;
                if (Input.GetAxisRaw("Vertical") > 0 || Input.GetKey(KeyCode.Space))
                {
                    dashDir = DashDir.up;
                }
                else if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    dashDir = DashDir.right;
                }
                else if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    dashDir = DashDir.left;
                }
                else
                {
                    dashDir = transform.localScale.x > 0 ? DashDir.right : DashDir.left;
                }
                StartDash();
                canDash = false;
            }
        }
    }

    void FixedUpdate()
    {

        if (TimeState == TimeStates.Normal)
        {
            if (UIController.Instance.isPaused) return;
            isOnGround = IsGrounded();


            if (isOnGround && (Time.time - lastJumpInputTime) < jumpBufferTime)
            {
                rb.velocity = new Vector2(rb.velocity.x, VerticalSpeed);
                PlayerState = PlayerStates.jump;
                canSecondJump = true;
                lastJumpInputTime = -10f;
                isOnGround = false; 
            }

            switch (PlayerState)
            {
                case PlayerStates.stay:
                    HandleStay();
                    break;
                case PlayerStates.move:
                    HandleMove();
                    break;
                case PlayerStates.jump:
                    HandleJump();
                    break;
                case PlayerStates.fall:
                    HandleFall();
                    break;
            }

            if (!canDash && !isDashing)
            {
                dashRecover += Time.fixedDeltaTime;
                if (dashRecover >= dashRecoverCount)
                {
                    canDash = true;
                    dashRecover = 0;
                }
            }

            if (isDashing && isStartDashing)
            {
                Vector2 dashVelocity = Vector2.zero;
                float dashSpeed = PlayerData.Instance.DashSpeed;
                switch (dashDir)
                {
                    case DashDir.right:
                        dashVelocity = new Vector2(dashSpeed, 0);
                        break;
                    case DashDir.left:
                        dashVelocity = new Vector2(-dashSpeed, 0);
                        break;
                    case DashDir.up:
                        dashVelocity = new Vector2(0, PlayerData.Instance.PlayerJumpSpeed);
                        break;
                }

                rb.velocity = dashVelocity;
            }
        }
    }

    public void StartDash()
    {
        isDashing = true;
        isStartDashing = true;
        dashCount = 0f;
        EndCorotine = false;

        if (dashTrailParticles != null)
        {
            emission.enabled = true;
            dashTrailParticles.Play();
        }
        if (dashDir != DashDir.up)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        rb.gravityScale = 0;
        StartCoroutine(DashScaleCoroutine(0.75f, 1.2f, PlayerData.Instance.DashTime));
    }

    public void EndDash()
    {
        PlayerState = PlayerStates.stay;
        isDashing = false;
        isStartDashing = false;
        rb.gravityScale = 1;
        scaleTimeCount = 0;

        if (dashTrailParticles != null)
        {
            emission.enabled = false;
            dashTrailParticles.Stop();
        }

        StopAllCoroutines();
    }

    IEnumerator DashScaleCoroutine(float scaleX, float scaleY, float totalTime)
    {
        float timeCount = 0;
        Vector3 startScale = Player.transform.localScale;
        Vector3 endScale = new Vector3(
            Mathf.Abs(scaleX) * Mathf.Sign(startScale.x),
            Mathf.Abs(scaleY) * Mathf.Sign(startScale.y), 0);

        Color color = Player.GetComponent<SpriteRenderer>().color;
        color.a = 0.5f;

        while (timeCount <= totalTime)
        {
            if (EndCorotine || !isDashing || !isStartDashing)
            {
                transform.localScale = startScale;
                color.a = 1;
                Player.GetComponent<SpriteRenderer>().color = color;
                EndDash();
                yield break;
            }

            timeCount += Time.deltaTime;
            float t = timeCount / totalTime;
            Vector3 newScale;

            if (t <= 0.5f)
            {
                float progress = t / 0.5f;
                newScale = Vector3.Lerp(startScale, endScale, progress);
                color.a = Mathf.Lerp(1, 0.5f, progress);
                Player.GetComponent<SpriteRenderer>().color = color;
            }
            else
            {
                float progress = (t - 0.5f) / 0.5f;
                newScale = Vector3.Lerp(endScale, startScale, progress);
                color.a = Mathf.Lerp(0.5f, 1, progress);
                Player.GetComponent<SpriteRenderer>().color = color;
            }

            transform.localScale = newScale;
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = startScale;
        color.a = 1;
        Player.GetComponent<SpriteRenderer>().color = color;
        EndDash();
    }

    private void SwitchStay()
    {
        if (!isOnGround)
        {
            PlayerState = PlayerStates.fall;
            return;
        }

        float moveInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            PlayerState = PlayerStates.move;
            return;
        }

        if ((Time.time - lastJumpInputTime) < jumpBufferTime)
        {
            PlayerState = PlayerStates.jump;
            rb.velocity = new Vector2(rb.velocity.x, VerticalSpeed);
            canSecondJump = true;
            lastJumpInputTime = -10f;
            return;
        }

        if (Input.GetAxisRaw("Vertical") < 0 && !isDashing)
        {
            PlayerState = PlayerStates.fall;
            rb.velocity = new Vector2(rb.velocity.x, -2.5f);
            return;
        }
    }

    private void SwitchMove()
    {
        if (!isOnGround)
        {
            if (rb.velocity.y < 0 || Input.GetAxisRaw("Vertical") < 0)
            {
                PlayerState = PlayerStates.fall;
                rb.velocity = new Vector2(rb.velocity.x, -2.5f);
                return;
            }
            else
            {
                PlayerState = PlayerStates.jump;
                return;
            }
        }
        if (Mathf.Abs(rb.velocity.x) <= 0.01f)
        {
            PlayerState = PlayerStates.stay;
            return;
        }

        if ((Time.time - lastJumpInputTime) < jumpBufferTime)
        {

            rb.velocity = new Vector2(rb.velocity.x, VerticalSpeed);
            canSecondJump = true;
            lastJumpInputTime = -10f;
            return;
        }
    }

    private void SwitchJump()
    {
        if (rb.velocity.y <= 0)
        {
            PlayerState = PlayerStates.fall;
            return;
        }

        // 手动下落
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            
            PlayerState = PlayerStates.fall;
            rb.velocity = new Vector2(rb.velocity.x, -2.5f);
            return;
        }
    }

    private void SwitchFall()
    {

        if (isOnGround && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            onGroundCount += Time.deltaTime;
            if (onGroundCount <= 0.05f && !isScalePlayed)
            {
                StartCoroutine(FallDashCoroutine(1.4f, 0.7f, 0.6f));
                isScalePlayed = true;
            }

            if (onGroundCount >= 0.1f)
            {
                PlayerState = PlayerStates.stay;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                canSecondJump = true;
                isScalePlayed = false;
            }
        }
        else
        {
            onGroundCount = 0;
        }
    }

    public void HandleStay()
    {
        rb.velocity = new Vector2(0.0005f, rb.velocity.y);
    }

    public void HandleMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal > 0)
        {
            rb.velocity = new Vector2(HorizentalSpeed, rb.velocity.y);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 0);
        }
        else if (horizontal < 0)
        {
            rb.velocity = new Vector2(-HorizentalSpeed, rb.velocity.y);
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 0);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public void HandleJump()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(moveInput * HorizentalSpeed, rb.velocity.y);
    }

    public void HandleFall()
    {
        
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * HorizentalSpeed, rb.velocity.y);
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            rb.AddForce(new Vector2(0, -fallingAddForce));
        }
    }

    private bool IsGrounded()
    {
        int groundHits = 0;
        Vector3[] checkPoints = {
            groundCheckPoint,
            groundCheckPoint + Vector3.left * (0.5f - checkRadius),
            groundCheckPoint + Vector3.right * (0.5f - checkRadius),
        };
        foreach (Vector3 point in checkPoints)
        {
            if (Physics2D.OverlapCircle(point, checkRadius, groundLayer))
            {
                groundHits++;
            }
        }
        return groundHits >= 1;
    }

    IEnumerator FallDashCoroutine(float scaleX, float scaleY, float totalTime)
    {
        float timeCount = 0;
        float t = 0;
        Vector3 startScale = new Vector3(Player.transform.localScale.x, Player.transform.localScale.y, 0);
        Vector3 endScale = new Vector3(Mathf.Abs(scaleX) * Mathf.Sign(Player.transform.localScale.x), Mathf.Abs(scaleY) * Mathf.Sign(Player.transform.localScale.y));
        Vector3 newScale;
        isFallDashing = true;
        while (timeCount <= totalTime)
        {
            if (isDashing)
            {
                transform.localScale = new Vector3(Mathf.Sign(startScale.x), Mathf.Sign(startScale.y));
                isFallDashing = false;
                yield break;
            }


            timeCount += Time.fixedDeltaTime;
            t = timeCount / totalTime;
            if (t <= 0.5f)
            {
                float progress = t / 0.5f;
                newScale = Vector3.Lerp(startScale, endScale, progress);
            }
            else
            {
                float progress = (t - 0.5f) / 0.5f;
                newScale = Vector3.Lerp(endScale, startScale, progress);
            }
            transform.localScale = newScale;
            yield return null;
        }
        transform.localScale = new Vector3(Mathf.Sign(startScale.x), Mathf.Sign(startScale.y));
        isFallDashing = false;
        yield break;
    }
    void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = isOnGround ? Color.green : Color.red;
            Vector3 center = groundCheckPoint;

            for (int i = 0; i < groundCheckPoints; i++)
            {
                float offsetX = 0f;
                if (groundCheckPoints > 1)
                {
                    offsetX = (i - (groundCheckPoints - 1) / 2f) * (0.5f - checkRadius);
                }
                Gizmos.DrawSphere(center + new Vector3(offsetX, 0, 0), checkRadius);
            }
        }
    }

    void Initicialize()
    {
        groundCheckPoint = new Vector3(transform.position.x, transform.position.y - 0.5F * GetComponent<Renderer>().bounds.size.y, transform.position.z);
        Player = instance.gameObject;
        rb = Player.GetComponent<Rigidbody2D>();
        if (dashTrailParticles != null)
        {
            emission = dashTrailParticles.emission;
            emission.enabled = false;
        }
        HorizentalSpeed = PlayerData.Instance.PlayerSpeed;
        VerticalSpeed = PlayerData.Instance.PlayerJumpSpeed;
        SlideSpeed = PlayerData.Instance.WallSlideSpeed;
        fallingAddForce = PlayerData.Instance.FallingAddForce;
        camera = Camera.main;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer != 6) return;
        if (PlayerState != PlayerStates.fall && PlayerState != PlayerStates.jump) return;
        if (Input.GetAxisRaw("Horizontal") == 0) return;

        foreach (var contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                rb.velocity = new Vector2(rb.velocity.x, SlideSpeed);
                if (!canSecondJump)
                {
                    canSecondJump = true;
                }
                break;
            }
        }
    }
}
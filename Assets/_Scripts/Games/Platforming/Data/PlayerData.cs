using UnityEngine;
public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    [Header("玩家预制体")]
    public GameObject PlayerPrefab;

    [Header("移动相关")]
    public float PlayerSpeed = 5;
    public float PlayerJumpSpeed = 5;
    public float DashSpeed = 100;
    public float WallSlideSpeed = -2;
    public float DashTime = 0.6f;
    public float PlayerGravityScale = 1;
    public float FallingAddForce = 50;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}

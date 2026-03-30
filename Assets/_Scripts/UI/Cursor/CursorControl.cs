using UnityEngine;
using Utility;
public class CustomCursorClick : Singleton<CustomCursorClick>
{
    [Header("默认鼠标贴图")]
    [SerializeField] private Texture2D normalCursor;

    [Header("按下时鼠标贴图")]
    [SerializeField] private Texture2D pressedCursor;

    [Header("热点位置")]
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    [Header("鼠标模式")]
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    protected override bool _isDonDestroyOnLoad => true;
    private void Start()
    {
        SetNormalCursor();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetPressedCursor();
        }

        if (Input.GetMouseButtonUp(0))
        {
            SetNormalCursor();
        }
    }

    private void SetNormalCursor()
    {
        if (normalCursor != null)
        {
            Cursor.SetCursor(normalCursor, hotspot, cursorMode);
        }
    }

    private void SetPressedCursor()
    {
        if (pressedCursor != null)
        {
            Cursor.SetCursor(pressedCursor, hotspot, cursorMode);
        }
    }
}
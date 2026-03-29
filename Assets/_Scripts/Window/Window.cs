using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum WindowState
{
    Foreground, Background
}

public class Window : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [LabelText("窗口名称")]
    public string Name;
    [LabelText("窗口名称UI")]
    public Text NameUI;
    [LabelText("窗口图标")]
    public Sprite Icon;
    [LabelText("窗口图标UI")]
    public Image IconUI;
    [LabelText("窗口前台耗电量")]
    public float ForegroundDrain = 0;
    [LabelText("窗口后台耗电量")]
    public float BackgroundDrain = 0;
    [LabelText("疲惫值")]
    public float Tired = 0;

    [HideInInspector]
    public float TickTime = 0;
    [HideInInspector]
    public float TiredTickTime = 0;
    [HideInInspector]
    public WindowState WindowState;

    private RectTransform rectTransform;
    private Canvas parentCanvas;

    // 拖拽用
    private bool isDragging = false;
    private Vector2 pointerOffset;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        if (NameUI) NameUI.text = Name;
        if (IconUI) IconUI.sprite = Icon;
        SetForeground();
    }

    public float ConsumePower(float rate = 1)
    {
        TickTime += Time.deltaTime;
        if (TickTime > 1)
        {
            switch (WindowState)
            {
                case WindowState.Foreground:
                    TickTime = 0;
                    return ForegroundDrain;

                case WindowState.Background:
                    TickTime = 0;
                    return BackgroundDrain * rate;
            }
        }
        return 0;
    }

    public float AddTired()
    {
        TiredTickTime += Time.deltaTime;
        if (TiredTickTime < 1) return 0;

        TiredTickTime = 0;
        if (WindowState == WindowState.Foreground)
            return Tired;

        return 0;
    }

    public void OnPointerClick(PointerEventData data)
    {
        SetForeground();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetForeground();

        isDragging = true;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || rectTransform == null || parentCanvas == null) return;

        RectTransform canvasRect = parentCanvas.transform as RectTransform;

        Vector2 localPointerPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPos))
        {
            rectTransform.localPosition = localPointerPos - pointerOffset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void SetForeground()
    {
        transform.SetAsLastSibling();

        if (WindowsController.Instance.ForegroundWindow != null &&
            WindowsController.Instance.ForegroundWindow != this)
        {
            WindowsController.Instance.ForegroundWindow.WindowState = WindowState.Background;
        }

        WindowState = WindowState.Foreground;
        WindowsController.Instance.SetForeground(this);
    }
}
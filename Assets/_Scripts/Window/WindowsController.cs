using DG.Tweening;
using DG.Tweening.Core.Easing;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using TaskSystem.Event;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class WindowsController : Singleton<WindowsController>
{
    [SerializeField, LabelText("窗口列表")]
    private List<Window> _windows;
    public Window ForegroundWindow { get; private set; }
    private bool _openTask;
    public bool OpenTask => _openTask;
    private float _powerConsume = 0;
    private float _powerRate = 1;
    private float _tired = 0;
    public List<Sprite> Images;
    public Image MassagePanel;
    private Coroutine _panelCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        TaskUpdate();
        ConsumeCount();
    }
    public void PlayMessagePanel(int taskID)
    {
        if (_panelCoroutine != null)
            StopCoroutine(_panelCoroutine);

        _panelCoroutine = StartCoroutine(ShowMessagePanel(taskID));
    }
    private IEnumerator ShowMessagePanel(int taskID)
    {
        switch (taskID)
        {
            case 1002:
                MassagePanel.sprite = Images[1];
                break;
            case 1003:
                MassagePanel.sprite = Images[2];
                break;

            case 2002:
                MassagePanel.sprite = Images[3];
                break;
            case 2003:
                MassagePanel.sprite = Images[4];
                break;

            case 3002:
                MassagePanel.sprite = Images[5];
                break;
            case 3003:
                MassagePanel.sprite = Images[6];
                break;

            case 4002:
                MassagePanel.sprite = Images[7];
                break;
            case 4003:
                MassagePanel.sprite = Images[8];
                break;

            case 5002:
                MassagePanel.sprite = Images[9];
                break;
            case 5003:
                MassagePanel.sprite = Images[10];
                break;

            case 6002:
                MassagePanel.sprite = Images[11];
                break;
            case 6003:
                MassagePanel.sprite = Images[12];
                break;
        }
        MassagePanel.gameObject.SetActive(true);
        RectTransform panelRect = MassagePanel.rectTransform;
        RectTransform canvasRect = MassagePanel.canvas.GetComponent<RectTransform>();

        // 先停止旧动画，避免重复调用时错乱
        panelRect.DOKill();

        float panelWidth = panelRect.rect.width;

        // 假设锚点在中心时：
        // 完全隐藏在右侧：panel中心点 = canvas宽度一半 + panel宽度一半
        float hiddenX = canvasRect.rect.width * 0.5f + panelWidth * 0.5f;

        // 像QQ弹窗一样先弹出来一点的位置
        float popX = hiddenX - panelWidth - 30f;

        // 最终回到原位：左边缘贴着Canvas右边缘
        // 即 panel中心点 = canvas右边缘 + panel宽度一半
        float finalX = hiddenX;

        // 先放到右侧外面
        Vector2 startPos = panelRect.anchoredPosition;
        panelRect.anchoredPosition = new Vector2(hiddenX + 50f, startPos.y);

        // 弹出 + 回弹
        Sequence seq = DOTween.Sequence();
        seq.Append(panelRect.DOAnchorPosX(popX, 0.45f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.5f);
        seq.Append(panelRect.DOAnchorPosX(finalX, 2.5f).SetEase(Ease.InOutCubic));

        yield return seq.WaitForCompletion();

        _panelCoroutine = null;
    }
    public void OpenTaskCo()
    {
        _openTask = true;
        StartCoroutine(DeltaTimeCo());
    }
    private IEnumerator DeltaTimeCo()
    {
        while (true)
        {
            TaskManager.Instance.TaskEventCenter.RaiseBegin(new Frame3UpdateEvent());
            yield return new WaitForSeconds(3);
        }
    }
    private void LateUpdate()
    {
        Consume();
    }
    private void TaskUpdate()
    {
        TaskManager.Instance.TaskEventCenter.RaiseBegin(new FrameUpdateEvent());
        TaskManager.Instance.TaskEventCenter.RaiseRunning(new FrameUpdateEvent());
        if (TaskManager.Instance.CanActivateNecessaryTask())
            TaskManager.Instance.TaskEventCenter.RaiseBegin(new ActivateNecessaryEvent());
        else if (TaskManager.Instance.CanActivateUnnecessaryTask())
            TaskManager.Instance.TaskEventCenter.RaiseBegin(new ActivateUnnecessaryEvent());
    }
    private void ConsumeCount()
    {
        float newRate = 1;
        foreach (Window window in _windows)
        {
            if (window.gameObject.activeSelf && window.isActiveAndEnabled)
            {
                _powerConsume += window.ConsumePower(_powerRate);
                _tired += window.AddTired();
                newRate += 0.15f;
            }
        }
        _powerRate = Mathf.Max(newRate - 0.15f, 1);
    }
    private void Limit()
    {

    }
    private void Consume()
    {
        UIController.Instance.batteryPowerPercent -= 0.01f * _powerConsume;
        UIController.Instance.tiringPercent += 0.01f * _tired;
        _tired = 0;
        _powerConsume = 0;
    }
    public void SetForeground(Window window) => ForegroundWindow = window;
    private void Init()
    {
        if (_windows != null && _windows.Count > 0)
            SetForeground(_windows[_windows.Count-1]);
    }
}

using DG.Tweening.Core.Easing;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using TaskSystem.Event;
using UnityEngine;
using Utility;

public class WindowsController : Singleton<WindowsController>
{
    [SerializeField, LabelText("´°¿ÚÁÐ±í")]
    private List<Window> _windows;
    public Window ForegroundWindow { get; private set; }
    private bool _openTask;
    public bool OpenTask => _openTask;
    private float _powerConsume = 0;
    private float _powerRate = 1;
    private float _tired = 0;
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

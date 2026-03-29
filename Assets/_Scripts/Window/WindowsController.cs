using System.Collections;
using System.Collections.Generic;
using Utility;
using UnityEngine;
using TaskSystem;
using TaskSystem.Event;
using Sirenix.OdinInspector;

public class WindowsController : Singleton<WindowsController>
{
    [SerializeField, LabelText("´°¿ÚÁÐ±í")]
    private List<Window> _windows;
    public Window ForegroundWindow { get; private set; }
    private float _powerConsume = 0;
    private float _powerRate = 1;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        TaskUpdate();
        ConsumePower();
    }
    private void LateUpdate()
    {
        DelPowerConsume();
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
    private void ConsumePower()
    {
        float newRate = 1;
        foreach (Window window in _windows)
        {
            if (window.gameObject.activeSelf && window.isActiveAndEnabled)
            {
                _powerConsume += window.ConsumePower(_powerRate);
                newRate += 0.15f;
            }
        }
        _powerRate = Mathf.Max(newRate - 0.15f, 1);
    }
    private void DelPowerConsume()
    {
        UIController.Instance.batteryPowerPercent -= 0.01f * _powerConsume;
        _powerConsume = 0;
    }
    public void SetForeground(Window window) => ForegroundWindow = window;
    private void Init()
    {
        if (_windows != null && _windows.Count > 0)
            SetForeground(_windows[0]);
    }
}

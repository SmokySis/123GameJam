using System.Collections;
using System.Collections.Generic;
using Utility;
using UnityEngine;
using TaskSystem;
using TaskSystem.Event;

public class WindowsController : Singleton<WindowsController>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TaskUpdate();
    }
    private void TaskUpdate()
    {
        TaskManager.Instance.TaskEventCenter.RaiseBegin(new FrameUpdateEvent());
        TaskManager.Instance.TaskEventCenter.RaiseRunning(new FrameUpdateEvent());
        if (TaskManager.Instance.CanActivateNecessaryTask())
            TaskManager.Instance.TaskEventCenter.RaiseBegin(new ActivateNecessaryEvent());
        else if(TaskManager.Instance.CanActivateUnnecessaryTask())
            TaskManager.Instance.TaskEventCenter.RaiseBegin(new ActivateUnnecessaryEvent());
    }
}

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using TaskSystem.Test;
using UnityEngine;

public class TestUpdate : MonoBehaviour
{
    [Button]
    private void Invoke() => TaskManager.Instance.TaskEventCenter.RaiseBegin(new GameStartedEvent());
    [Button]
    private void End() => TaskManager.Instance.TaskEventCenter.RaiseRunning(new GameEndEvent());
    private void Update()
    {
        Debug.Log(TaskManager.Instance.IsTaskWaiting(0));
        Debug.Log(TaskManager.Instance.IsTaskActive(0));
    }
}

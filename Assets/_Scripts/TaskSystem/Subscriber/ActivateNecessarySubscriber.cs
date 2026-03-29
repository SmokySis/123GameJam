using EventSystem;
using System;
using System.Collections.Generic;
using TaskSystem.Event;
using UnityEngine;

namespace TaskSystem.Subscriber
{
    [Serializable]
    public sealed class ActivateNecessarySubscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new ActivateNecessarySubscriber();
        [Serializable]
        private sealed class Listener : EventListener<ActivateNecessaryEvent>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in ActivateNecessaryEvent gameEvent)
            {
                Debug.Log("1321432");
                if (!TaskManager.Instance.CanActivateNecessaryTask()) return;
                TaskManager.Instance.Lock();
                UIController.Instance.SetDetailedMessage(TaskLoader.Instance.GetTaskData(_taskID).text);
                UIController.Instance.SetMissionButton(() => { TaskManager.Instance.RequestActivateTask(_taskID); TaskManager.Instance.EndLock(); });
            }
        }
    }
}
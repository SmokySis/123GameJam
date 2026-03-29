using EventSystem;
using System;
using System.Collections.Generic;
using TaskSystem.Event;
using UnityEngine;

namespace TaskSystem.Subscriber
{
    [Serializable]
    public sealed class FirstTaskSubscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new FirstTaskSubscriber();
        [Serializable]
        private sealed class Listener : EventListener<Frame3UpdateEvent>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in Frame3UpdateEvent gameEvent)
            {
                Debug.Log(_taskID+"hear");
                if (!WindowsController.Instance.OpenTask || !TaskManager.Instance.CanActivateNecessaryTask())
                    return;
                Debug.Log(_taskID);
                TaskManager.Instance.Lock();
                TaskManager.Instance.RequestActivateTask(_taskID);
                TaskManager.Instance.EndLock(3);
            }
        }
    }
}

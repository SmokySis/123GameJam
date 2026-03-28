using EventSystem;
using System;
using System.Collections.Generic;
using TaskSystem.Event;

namespace TaskSystem.Subscriber
{
    [Serializable]
    public sealed class StartLabReportSubscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new StartLabReportSubscriber();
        [Serializable]
        private sealed class Listener : EventListener<StartLabReportEvent>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in StartLabReportEvent gameEvent) => TaskManager.Instance.RequestActivateTask(_taskID);
        }
    }
}

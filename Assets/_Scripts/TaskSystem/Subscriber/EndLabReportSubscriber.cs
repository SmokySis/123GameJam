using EventSystem;
using System;
using System.Collections.Generic;
using TaskSystem.Event;

namespace TaskSystem.Subscriber
{
    [Serializable]
    public sealed class EndLabReportSubscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new EndLabReportSubscriber();
        [Serializable]
        private sealed class Listener : EventListener<EndLabReportEvent>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in EndLabReportEvent gameEvent) => TaskManager.Instance.RequestCompleteTask(_taskID);
        }
    }
}
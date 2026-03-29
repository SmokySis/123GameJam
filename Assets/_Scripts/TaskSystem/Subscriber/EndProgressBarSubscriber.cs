using EventSystem;
using System;
using System.Collections.Generic;
using TaskSystem.Event;

namespace TaskSystem.Subscriber
{
    [Serializable]
    public sealed class EndProgressBarSubscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new EndProgressBarSubscriber();
        [Serializable]
        private sealed class Listener : EventListener<EndProgressBarEvent>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in EndProgressBarEvent gameEvent) => TaskManager.Instance.RequestActivateTask(_taskID);
        }
    }
}

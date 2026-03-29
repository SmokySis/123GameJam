using EventSystem;
using System;
using System.Collections.Generic;
using TaskSystem.Event;

namespace TaskSystem.Subscriber
{
    [Serializable]
    public sealed class EndThirdQTESubscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new EndThirdQTESubscriber();
        [Serializable]
        private sealed class Listener : EventListener<EndThirdQTEEvent>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in EndThirdQTEEvent gameEvent) => TaskManager.Instance.RequestActivateTask(_taskID);
        }
    }
}

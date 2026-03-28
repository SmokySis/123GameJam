using EventSystem;
using System;
using System.Collections.Generic;
using TaskSystem.Event;

namespace TaskSystem.Subscriber
{
    [Serializable]
    public sealed class StartResumeSubscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new StartResumeSubscriber();
        [Serializable]
        private sealed class Listener : EventListener<StartResumeEvent>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in StartResumeEvent gameEvent) => TaskManager.Instance.RequestActivateTask(_taskID);
        }
    }
}

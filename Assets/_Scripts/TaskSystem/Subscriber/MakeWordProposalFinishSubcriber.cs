using EventSystem;
using System;
using System.Collections.Generic;
using TaskSystem.Event;

namespace TaskSystem.Subscriber
{
    [Serializable]
    public sealed class MakeWordProposalFinishSubcriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new MakeWordProposalFinishSubcriber();
        [Serializable]
        private sealed class Listener : EventListener<MakeWordProposalFinishEvent>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in MakeWordProposalFinishEvent gameEvent) => TaskManager.Instance.RequestCompleteTask(_taskID);
        }
    }
}

using EventSystem;
using System;
using System.Collections.Generic;
using TaskSystem.Event;

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
        private sealed class Listener : EventListener<FirstTaskEvent>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in FirstTaskEvent gameEvent)
            {
                UIController.Instance.SetDetailedMessage(TaskLoader.Instance.GetTaskData(_taskID).text);
                UIController.Instance.SetMissionButton(() =>TaskManager.Instance.RequestActivateTask(_taskID));
            }
        }
    }
}

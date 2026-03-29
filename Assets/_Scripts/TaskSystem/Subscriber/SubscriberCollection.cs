using EventSystem;
using System;
using System.Collections.Generic;
using TaskSystem.Event;

namespace TaskSystem.Subscriber
{
    public struct End1AndStart2Event : IGameEvent { public int oldID; }
    [Serializable]
    public sealed class Start2Subscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new Start2Subscriber();
        [Serializable]
        private sealed class Listener : EventListener<End1AndStart2Event>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in End1AndStart2Event gameEvent)
            {
                if (_taskID == gameEvent.oldID + 1)
                    CoroutineRunner.Instance.Delay(3, () => TaskManager.Instance.RequestActivateTask(_taskID));
            }
        }
    }
    public struct End2AndStart3Event : IGameEvent { public int oldID; }
    [Serializable]
    public sealed class Start3Subscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new Start3Subscriber();
        [Serializable]
        private sealed class Listener : EventListener<End2AndStart3Event>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in End2AndStart3Event gameEvent)
            {
                if (_taskID == gameEvent.oldID + 1)
                    CoroutineRunner.Instance.Delay(3, () => TaskManager.Instance.RequestActivateTask(_taskID));
            }
        }
    }
    public struct End1Event : IGameEvent { public int ID; public End1Event(int id) => ID = id; }
    [Serializable]
    public sealed class End1Subscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new End1Subscriber();
        [Serializable]
        private sealed class Listener : EventListener<End1Event>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in End1Event gameEvent)
            {
                if (_taskID == gameEvent.ID)
                {
                    TaskManager.Instance.TaskEventCenter.RaiseBegin(new End1AndStart2Event() { oldID = _taskID });
                    TaskManager.Instance.RequestCompleteTask(_taskID);
                }
            }
        }
    }
    public struct End2Event : IGameEvent { public int ID; public End2Event(int id) => ID = id; }
    [Serializable]
    public sealed class End2Subscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new End2Subscriber();
        [Serializable]
        private sealed class Listener : EventListener<End2Event>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in End2Event gameEvent)
            {
                if (_taskID == gameEvent.ID)
                {
                    TaskManager.Instance.TaskEventCenter.RaiseBegin(new End2AndStart3Event() { oldID = _taskID });
                    TaskManager.Instance.RequestCompleteTask(_taskID);
                }
            }
        }
    }
    public struct End3Event : IGameEvent { public int ID; public End3Event(int id) => ID = id; }
    [Serializable]
    public sealed class End3Subscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }
        public IEventSubscriber DeepCopy() => new End3Subscriber();
        [Serializable]
        private sealed class Listener : EventListener<End3Event>
        {
            private readonly int _taskID;
            public Listener(int taskID) => _taskID = taskID;
            protected override void OnEvent(in End3Event gameEvent)
            {
                if (_taskID == gameEvent.ID)
                    TaskManager.Instance.RequestCompleteTask(_taskID);
            }
        }
    }
}

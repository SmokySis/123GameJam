using EventSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaskSystem.Test
{
    [Serializable]
    public sealed class AutoActivateOnGameStartedSubscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }

        public IEventSubscriber DeepCopy()
        {
            return new AutoActivateOnGameStartedSubscriber();
        }

        [Serializable]
        private sealed class Listener : EventListener<GameStartedEvent>
        {
            private readonly int _taskID;

            public Listener(int taskID)
            {
                _taskID = taskID;
            }

            protected override void OnEvent(in GameStartedEvent gameEvent)
            {
                Debug.Log($"[TaskTest] Task({_taskID}) received GameStartedEvent, request activate.");
                TaskManager.Instance.RequestActivateTask(_taskID);
            }
        }
    }
    [Serializable]
    public sealed class EndSubscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            yield return new Listener(taskID);
        }

        public IEventSubscriber DeepCopy()
        {
            return new EndSubscriber();
        }

        [Serializable]
        private sealed class Listener : EventListener<GameEndEvent>
        {
            private readonly int _taskID;

            public Listener(int taskID)
            {
                _taskID = taskID;
            }

            protected override void OnEvent(in GameEndEvent gameEvent)
            {
                Debug.Log($"[TaskTest] Task({_taskID}) received GameEndEvent, request activate.");
                TaskManager.Instance.RequestCompleteTask(_taskID);
            }
        }
    }
    [Serializable]
    public sealed class APPOpenSubscriber : IEventSubscriber
    {
        public IEnumerable<IEventListener> GetEventListeners(int taskID)
        {
            Debug.Log("111");
            yield return new Listener(taskID);
        }

        public IEventSubscriber DeepCopy()
        {
            return new APPOpenSubscriber();
        }

        [Serializable]
        private sealed class Listener : EventListener<APPOpenEvent>
        {
            private readonly int _taskID;

            public Listener(int taskID)
            {
                _taskID = taskID;
            }

            protected override void OnEvent(in APPOpenEvent gameEvent)
            {
                TaskManager.Instance.RequestActivateTask(_taskID);
                
            }
        }
    }
}
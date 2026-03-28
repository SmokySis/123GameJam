using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystem
{
    public sealed class EventRouter
    {
        private const int DEFAULT_LIST_COUNT = 4;
        private const int DEFAULT_TASK_LISTENERS_COUNT = 128;
        // 按事件类型分桶
        private readonly Dictionary<Type, Bucket> _buckets = new();
        // 记录每个 taskID 注册过哪些 listener，便于反注册
        private readonly Dictionary<int, List<IEventListener>> _taskListeners = new(DEFAULT_TASK_LISTENERS_COUNT);
        private Bucket GetOrCreateBucket(Type eventType)
        {
            if (!_buckets.TryGetValue(eventType, out Bucket bucket))
            {
                bucket = new Bucket();
                _buckets.Add(eventType, bucket);
            }

            return bucket;
        }
        /// <summary>
        /// 为某个任务注册单个监听器
        /// </summary>
        public void RegisterListener(int taskID, IEventListener listener)
        {
            if (listener == null)
            {
                Debug.LogError("EventRouter Register Error: Listener Is Null.");
                return;
            }
            if (!_taskListeners.TryGetValue(taskID, out List<IEventListener> list))
            {
                list = new List<IEventListener>(DEFAULT_LIST_COUNT);
                _taskListeners.Add(taskID, list);
            }
            list.Add(listener);
            Bucket bucket = GetOrCreateBucket(listener.EventType);
            bucket.Add(listener);
        }
        /// <summary>
        /// 为某个任务注册一个订阅器中的全部监听器
        /// </summary>
        public void Register(int taskID, IEventSubscriber subscriber)
        {
            if (subscriber == null)
            {
                Debug.LogError("EventRouter Register Error: Subscriber Is Null.");
                return;
            }
            IEnumerable<IEventListener> listeners = subscriber.GetEventListeners(taskID);
            if (listeners == null) return;
            foreach (IEventListener listener in listeners)
                RegisterListener(taskID, listener);
        }
        /// <summary>
        /// 为某个任务注册多个订阅器
        /// </summary>
        public void RegisterAll(int taskID, IEnumerable<IEventSubscriber> subscribers)
        {
            if (subscribers == null) return;
            foreach (IEventSubscriber subscriber in subscribers)
                Register(taskID, subscriber);
        }
        /// <summary>
        /// 注销某个任务注册的全部监听器
        /// </summary>
        public void UnregisterAll(int taskID)
        {
            if (!_taskListeners.TryGetValue(taskID, out List<IEventListener> list)) return;
            foreach (IEventListener listener in list)
            {
                if (listener == null) continue;
                if (_buckets.TryGetValue(listener.EventType, out Bucket bucket))
                    bucket.Remove(listener);
            }
            list.Clear();
            _taskListeners.Remove(taskID);
        }
        /// <summary>
        /// 派发事件
        /// </summary>
        public void Raise<TEvent>(in TEvent e) where TEvent : struct, IGameEvent
        {
            if (_buckets.TryGetValue(typeof(TEvent), out Bucket bucket))
                bucket.Raise(in e);
        }
        private sealed class Bucket
        {
            private readonly List<IEventListener> _listeners = new(8);
            public void Add(IEventListener listener)
            {
                if (listener == null)
                    return;

                _listeners.Add(listener);
            }
            public void Remove(IEventListener listener)
            {
                int index = _listeners.IndexOf(listener);
                if (index < 0)
                    return;

                int last = _listeners.Count - 1;
                _listeners[index] = _listeners[last];
                _listeners.RemoveAt(last);
            }
            public void Raise<TEvent>(in TEvent e) where TEvent : struct, IGameEvent
            {
                // 为避免派发过程中监听器列表被修改导致迭代异常，
                // 这里用 for 循环比 foreach 更稳一些
                for (int i = 0; i < _listeners.Count; i++)
                {
                    IEventListener listener = _listeners[i];
                    listener?.Invoke(e);
                }
            }
        }
    }
}
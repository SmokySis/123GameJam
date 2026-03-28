using System.Collections.Generic;
using System;

namespace EventSystem
{
    public interface IGameEvent { }
    #region 事件系统接口
    public interface IEventSubscriber
    {
        /// <summary>
        /// 要订阅的事件监听器集合（对象可缓存复用）
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEventListener> GetEventListeners(int taskID);
        IEventSubscriber DeepCopy();
    }
    /// <summary>
    /// 强类型事件监听器（不装箱，不反射）
    /// </summary>
    public interface IEventListener
    {
        Type EventType { get; }
        void Invoke(in IGameEvent gameEvent);
    }
    [Serializable]
    public abstract class EventListener<TEvent> : IEventListener where TEvent : struct, IGameEvent
    {
        public Type EventType => typeof(TEvent);
        public void Invoke(in IGameEvent gameEvent)
        {
            if (gameEvent is TEvent e)
                OnEvent(in e);
        }
        protected abstract void OnEvent(in TEvent gameEvent);
    }
    #endregion
}
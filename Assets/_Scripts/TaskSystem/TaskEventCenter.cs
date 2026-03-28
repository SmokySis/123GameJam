using EventSystem;

namespace TaskSystem
{
    public sealed class TaskEventCenter
    {
        private readonly EventRouter _beginEventRouter = new();
        private readonly EventRouter _runtimeEventRouter = new();
        public void RegisterBeginEvent(int taskID, params IEventSubscriber[] beginEventSubscribers) => _beginEventRouter.RegisterAll(taskID, beginEventSubscribers);
        public void UnregisterBeginEvent(int taskID) => _beginEventRouter.UnregisterAll(taskID);
        public void RegisterRuntimeEvent(int taskID, params IEventSubscriber[] runtimeEventSubscribers) => _runtimeEventRouter.RegisterAll(taskID, runtimeEventSubscribers);
        public void UnregisterRuntimeEvent(int taskID) => _runtimeEventRouter.UnregisterAll(taskID);
        public void RaiseBegin<TEvent>(in TEvent e) where TEvent : struct, IGameEvent => _beginEventRouter.Raise(in e);
        public void RaiseRunning<TEvent>(in TEvent e) where TEvent : struct, IGameEvent => _runtimeEventRouter.Raise(in e);
    }
}
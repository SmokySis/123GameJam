
using EventSystem;
using System.Linq;

namespace TaskSystem
{
    public sealed class Task
    {
        private readonly TaskData _data;
        private readonly TaskRuntimeData _runtimeData;
        private readonly IEventSubscriber[] _activationSubscribers;
        private readonly IEventSubscriber[] _runtimeSubscribers;
        public TaskData Data => _data;
        public TaskRuntimeData RuntimeData => _runtimeData;
        public IEventSubscriber[] ActivationSubscribers => _activationSubscribers;
        public IEventSubscriber[] RuntimeSubscribers => _runtimeSubscribers;
        public Task(TaskData data, TaskRuntimeData runtimeData)
        {
            _data = data;
            _runtimeData = runtimeData;

            _activationSubscribers = data.ActivationSubscribers?
                .Select(x => x?.DeepCopy())
                .ToArray() ?? System.Array.Empty<IEventSubscriber>();

            _runtimeSubscribers = data.RuntimeSubscribers?
                .Select(x => x?.DeepCopy())
                .ToArray() ?? System.Array.Empty<IEventSubscriber>();
        }
    }
}

using UnityEngine;

namespace TaskSystem
{
    public enum TaskState
    {
        WaitingToBegin,
        Active,
        Completed,
        Failed
    }
    public sealed class TaskRuntimeData
    {
        public TaskState State;
        public int CurrentProgress;
        public bool PendingActivate;
        public bool PendingComplete;
        public bool PendingFail;
        public bool IsActivated => State == TaskState.Active;
        public bool IsCompleted => State == TaskState.Completed;
        public GameObject TextObj;
    }

}

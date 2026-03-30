using DG.Tweening;
using PoolSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace TaskSystem
{
    public class TaskManager : Singleton<TaskManager>
    {
        [SerializeField, LabelText("最大同时可存在的必要任务")]
        private int _maxNecessaryCount = 6;
        [SerializeField, LabelText("最大同时可存在的可选任务")]
        private int _maxUnnecessaryCount = 0;
        [SerializeField, LabelText("必要任务信息栏父对象")]
        private Transform _necessaryParent;
        [SerializeField, LabelText("可选任务信息栏父对象")]
        private Transform _unnecessaryParent;
        [SerializeField, LabelText("信息预制体")]
        private GameObject _textPrefab;
        [SerializeField, LabelText("文本出现速度")]
        private float charsPerSecond = 20f;
        [SerializeField, LabelText("文本框")]
        private VerticalLayoutGroup layoutGroup;
        private readonly Dictionary<int, Task> _idToTask = new();
        private readonly HashSet<int> _waitingTasks = new();
        private readonly HashSet<int> _activeNecessaryTasks = new();
        private readonly HashSet<int> _activeUnnecessaryTasks = new();
        private readonly HashSet<int> _completedTasks = new();
        private readonly HashSet<int> _failedTasks = new();
        private readonly List<int> _pendingActivateTasks = new();
        private readonly List<int> _pendingCompleteTasks = new();
        private readonly List<int> _pendingFailTasks = new();
        private readonly TaskEventCenter _taskEventCenter = new();
        private bool _isInitialized;
        private bool _activateLock;
        protected override bool _isDonDestroyOnLoad => true;
        public TaskEventCenter TaskEventCenter => _taskEventCenter;
        public bool IsInitialized => _isInitialized;
        public event Action<Task> OnTaskActivated;
        public event Action<Task> OnTaskCompleted;
        public event Action<Task> OnTaskFailed;
        protected override void Awake() => base.Awake();
        private void LateUpdate()
        {
            if (!_isInitialized)
                return;
            FlushLifecycleRequests();
        }
        public void Initialize(IReadOnlyDictionary<int, TaskData> allTaskData)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("TaskManager Initialize Warning: Already Initialized.");
                return;
            }
            if (_necessaryParent == null)
            {
                Debug.LogError("TaskManager Initialize Error: NecessaryParent Is Null");
                return;
            }
            if (_unnecessaryParent == null)
            {
                Debug.LogError("TaskManager Initialize Error: UnnecessaryParent Is Null");
                return;
            }
            if (_textPrefab == null)
            {
                Debug.LogError("TaskManager Initialize Error: TextPrefab Is Null");
                return;
            }
            if (allTaskData == null)
            {
                Debug.LogError("TaskManager Initialize Error: allTaskData Is Null.");
                return;
            }
            ClearAllInternalState();
            foreach (var pair in allTaskData)
            {
                int taskID = pair.Key;
                TaskData data = pair.Value;
                Task task = TaskLoader.Instance.CreateTask(data, new TaskRuntimeData { State = TaskState.WaitingToBegin });
                AddTaskToWait(taskID, task);
            }
            _isInitialized = true;
        }
        private void ClearAllInternalState()
        {
            _idToTask.Clear();
            _waitingTasks.Clear();
            _activeNecessaryTasks.Clear();
            _activeUnnecessaryTasks.Clear();
            _completedTasks.Clear();
            _failedTasks.Clear();

            _pendingActivateTasks.Clear();
            _pendingCompleteTasks.Clear();
            _pendingFailTasks.Clear();
        }
        public bool TryGetTask(int taskID, out Task task) => _idToTask.TryGetValue(taskID, out task);
        public bool IsTaskWaiting(int taskID) => _waitingTasks.Contains(taskID);
        public bool IsTaskActive(int taskID) => _activeNecessaryTasks.Contains(taskID) || _activeUnnecessaryTasks.Contains(taskID);
        public bool IsTaskCompleted(int taskID) => _completedTasks.Contains(taskID);
        public bool IsTaskFailed(int taskID) => _failedTasks.Contains(taskID);

        public void AddTaskToWait(int taskID, Task task)
        {
            if (task == null)
            {
                Debug.LogError("TaskManager AddTaskToWait Error: Task Is Null.");
                return;
            }
            if (_idToTask.ContainsKey(taskID))
            {
                Debug.LogError($"TaskManager AddTaskToWait Error: Duplicate task ID {taskID}");
                return;
            }
            task.RuntimeData.State = TaskState.WaitingToBegin;
            task.RuntimeData.PendingActivate = false;
            task.RuntimeData.PendingComplete = false;
            task.RuntimeData.PendingFail = false;
            _idToTask.Add(taskID, task);
            _waitingTasks.Add(taskID);
            _taskEventCenter.RegisterBeginEvent(taskID, task.ActivationSubscribers);
        }
        public void RequestActivateTask(int taskID)
        {
            if (!_idToTask.TryGetValue(taskID, out Task task)) return;
            if (task.RuntimeData.State != TaskState.WaitingToBegin) return;
            if (task.RuntimeData.PendingActivate) return;
            task.RuntimeData.PendingActivate = true;
            _pendingActivateTasks.Add(taskID);
        }
        public void RequestCompleteTask(int taskID)
        {
            if (!_idToTask.TryGetValue(taskID, out Task task)) return;
            if (task.RuntimeData.State != TaskState.Active) return;
            if (task.RuntimeData.PendingComplete) return;
            task.RuntimeData.PendingComplete = true;
            _pendingCompleteTasks.Add(taskID);
        }
        public void RequestFailTask(int taskID)
        {
            if (!_idToTask.TryGetValue(taskID, out Task task)) return;
            if (task.RuntimeData.State != TaskState.Active && task.RuntimeData.State != TaskState.WaitingToBegin) return;
            if (task.RuntimeData.PendingFail) return;
            task.RuntimeData.PendingFail = true;
            _pendingFailTasks.Add(taskID);
        }
        private bool CanActivateTask(Task task)
        {
            if (task == null)
                return false;
            if (task.Data.IsNecessary)
                return _activeNecessaryTasks.Count < _maxNecessaryCount;
            return _activeUnnecessaryTasks.Count < _maxUnnecessaryCount;
        }
        public bool CanActivateNecessaryTask() => _activeNecessaryTasks.Count < _maxNecessaryCount && !_activateLock;
        public bool CanActivateUnnecessaryTask() => _activeNecessaryTasks.Count < _maxUnnecessaryCount && !_activateLock;
        public bool Lock() => _activateLock = true;
        public bool EndLock() => _activateLock = false;
        public void EndLock(int second) => StartCoroutine(EndLoakCo(second));
        private IEnumerator EndLoakCo(int second)
        {
            yield return new WaitForSeconds(second);
            EndLock();
            yield return null;
        }

        private bool ActivateTask(int taskID)
        {
            if (!_idToTask.TryGetValue(taskID, out Task task))
                return false;
            if (task.RuntimeData.State != TaskState.WaitingToBegin)
                return false;
            if (!CanActivateTask(task))
            {
                task.RuntimeData.PendingActivate = false;
                return false;
            }
            _taskEventCenter.UnregisterBeginEvent(taskID);
            _waitingTasks.Remove(taskID);
            _taskEventCenter.RegisterRuntimeEvent(taskID, task.RuntimeSubscribers);
            task.RuntimeData.State = TaskState.Active;
            task.RuntimeData.PendingActivate = false;
            if (task.Data.IsNecessary)
                _activeNecessaryTasks.Add(taskID);
            else
                _activeUnnecessaryTasks.Add(taskID);
            ShowInfo(task);
            OnTaskActivated?.Invoke(task);
            return true;
        }
        private bool CompleteTask(int taskID)
        {
            if (!_idToTask.TryGetValue(taskID, out Task task))
                return false;
            if (task.RuntimeData.State != TaskState.Active)
                return false;
            _taskEventCenter.UnregisterRuntimeEvent(taskID);
            if (task.Data.IsNecessary)
                _activeNecessaryTasks.Remove(taskID);
            else
                _activeUnnecessaryTasks.Remove(taskID);
            task.RuntimeData.State = TaskState.Completed;
            task.RuntimeData.PendingComplete = false;
            task.RuntimeData.PendingFail = false;
            _completedTasks.Add(taskID);
            OnTaskCompleted?.Invoke(task);
            HideInfo(task);
            return true;
        }
        private bool FailTask(int taskID)
        {
            if (!_idToTask.TryGetValue(taskID, out Task task))
                return false;
            if (task.RuntimeData.State == TaskState.Active)
            {
                _taskEventCenter.UnregisterRuntimeEvent(taskID);

                if (task.Data.IsNecessary)
                    _activeNecessaryTasks.Remove(taskID);
                else
                    _activeUnnecessaryTasks.Remove(taskID);
            }
            else if (task.RuntimeData.State == TaskState.WaitingToBegin)
            {
                _taskEventCenter.UnregisterBeginEvent(taskID);
                _waitingTasks.Remove(taskID);
            }
            else
            {
                return false;
            }
            task.RuntimeData.State = TaskState.Failed;
            task.RuntimeData.PendingActivate = false;
            task.RuntimeData.PendingComplete = false;
            task.RuntimeData.PendingFail = false;
            _failedTasks.Add(taskID);
            OnTaskFailed?.Invoke(task);
            return true;
        }
        public bool RemoveTask(int taskID)
        {
            if (!_idToTask.TryGetValue(taskID, out Task task))
                return false;

            switch (task.RuntimeData.State)
            {
                case TaskState.WaitingToBegin:
                    _taskEventCenter.UnregisterBeginEvent(taskID);
                    _waitingTasks.Remove(taskID);
                    break;

                case TaskState.Active:
                    _taskEventCenter.UnregisterRuntimeEvent(taskID);
                    if (task.Data.IsNecessary)
                        _activeNecessaryTasks.Remove(taskID);
                    else
                        _activeUnnecessaryTasks.Remove(taskID);
                    break;
                case TaskState.Completed:
                    _completedTasks.Remove(taskID);
                    break;

                case TaskState.Failed:
                    _failedTasks.Remove(taskID);
                    break;
            }
            _idToTask.Remove(taskID);
            return true;
        }
        string GetDifficultyColor(TaskDifficulty difficulty)
        {
            switch (difficulty)
            {
                case TaskDifficulty.简单:
                    return "<color=green>";
                case TaskDifficulty.中等:
                    return "<color=yellow>";
                case TaskDifficulty.困难:
                    return "<color=red>";
                default:
                    return "<color=white>";
            }
        }
        string ResetColor() => "</color>";
        private void ShowInfo(Task task)
        {
            GameObject textObj;
            if (task.Data.IsNecessary)
                textObj = GameObjectPoolCenter.Instance.GetInstance(_textPrefab, Vector3.zero, Quaternion.identity, parent: _necessaryParent, changeParent: true);
            else
                textObj = GameObjectPoolCenter.Instance.GetInstance(_textPrefab, Vector3.zero, Quaternion.identity, parent: _unnecessaryParent, changeParent: true);
            textObj.GetComponent<RectTransform>().localScale = Vector3.one;
            Text uiText = textObj.GetComponent<Text>();
            if (uiText == null)
            {
                Debug.LogError("ShowInfo Error: Text Component Is Null.");
                return;
            }
            string content = $"{task.Data.Description} \n{GetDifficultyColor(task.Data.TaskDifficulty)}【{task.Data.TaskDifficulty}】{ResetColor()}";
            uiText.text = string.Empty;
            int currentCount = 0;
            float duration = content.Length <= 0 ? 0f : content.Length / charsPerSecond;
            DOTween.To(() => currentCount, x => { currentCount = x; uiText.text = content.Substring(0, currentCount); }, content.Length, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (layoutGroup != null)
                {
                    LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.GetComponent<RectTransform>());
                }
            });
            task.RuntimeData.TextObj = textObj;
        }
        private void HideInfo(Task task)
        {
            if (task == null || task.RuntimeData == null || task.RuntimeData.TextObj == null)
                return;
            GameObject textObj = task.RuntimeData.TextObj;
            Text uiText = textObj.GetComponent<Text>();
            if (uiText == null)
            {
                GameObjectPoolCenter.Instance.Release(textObj);
                task.RuntimeData.TextObj = null;
                return;
            }
            DOTween.Kill(textObj);
            Color originalColor = uiText.color;
            Color greenColor = Color.green;
            greenColor.a = originalColor.a;
            Sequence seq = DOTween.Sequence().SetTarget(textObj);
            // 先变绿
            seq.Append(
                DOTween.To(
                    () => 0f,
                    t =>
                    {
                        uiText.color = Color.Lerp(originalColor, greenColor, t);
                    },
                    1f,
                    0.15f
                ).SetEase(Ease.Linear)
            );
            // 再淡出
            seq.Append(
                DOTween.To(
                    () => uiText.color.a,
                    a =>
                    {
                        Color c = uiText.color;
                        c.a = a;
                        uiText.color = c;
                    },
                    0f,
                    0.5f
                ).SetEase(Ease.Linear)
            );
            seq.OnComplete(() =>
            {
                uiText.color = originalColor;
                GameObjectPoolCenter.Instance.Release(textObj);
                task.RuntimeData.TextObj = null;
            });
        }
        public void FlushLifecycleRequests()
        {
            if (_pendingActivateTasks.Count > 0)
            {
                for (int i = 0; i < _pendingActivateTasks.Count; i++)
                {
                    int taskID = _pendingActivateTasks[i];
                    ActivateTask(taskID);
                }
                _pendingActivateTasks.Clear();
            }
            if (_pendingCompleteTasks.Count > 0)
            {
                for (int i = 0; i < _pendingCompleteTasks.Count; i++)
                {
                    int taskID = _pendingCompleteTasks[i];
                    CompleteTask(taskID);
                }
                _pendingCompleteTasks.Clear();
            }
            if (_pendingFailTasks.Count > 0)
            {
                for (int i = 0; i < _pendingFailTasks.Count; i++)
                {
                    int taskID = _pendingFailTasks[i];
                    FailTask(taskID);
                }
                _pendingFailTasks.Clear();
            }
        }
    }
}
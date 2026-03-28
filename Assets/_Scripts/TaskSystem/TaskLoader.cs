using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace TaskSystem
{
    public class TaskLoader : Singleton<TaskLoader>
    {
        private Dictionary<int, TaskData> _datas;
        protected override bool _isDonDestroyOnLoad => true;
        public IReadOnlyDictionary<int, TaskData> AllTaskData => _datas;
        protected override void Awake() => base.Awake();
        public void LoadAllTaskData()
        {
            if (_datas != null) return;
            TaskData[] datas = Resources.LoadAll<TaskData>("TaskDatas");
            _datas = new Dictionary<int, TaskData>(datas.Length);
            foreach (TaskData data in datas)
            {
                if (data == null)
                {
                    Debug.LogError("TaskLoader LoadAllTaskData Error: Data Is Null.");
                    continue;
                }
                if (_datas.ContainsKey(data.ID))
                {
                    Debug.LogError($"TaskLoader LoadAllTaskData Error: Duplicate ID {data.ID}");
                    continue;
                }
                _datas.Add(data.ID, data);
            }
            Debug.Log($"Load {datas.Length} Data");
        }
        public Task CreateTask(TaskData data, TaskRuntimeData runtimeData)
        {
            if (data == null)
            {
                Debug.LogError("TaskLoader CreateTask Error: Data Is Null.");
                return null;
            }
            if (runtimeData == null)
            {
                Debug.LogError("TaskLoader CreateTask Error: RuntimeData Is Null.");
                return null;
            }
            return new Task(data, runtimeData);
        }
    }
}
using UnityEngine;

namespace TaskSystem
{
    public class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            TaskLoader.Instance.LoadAllTaskData();
            TaskManager.Instance.Initialize(TaskLoader.Instance.AllTaskData);
        }
    }
}
using EventSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TaskSystem
{
    [CreateAssetMenu(menuName = "Task/TaskData", fileName = "TaskData")]
    public class TaskData : SerializedScriptableObject
    {
        [LabelText("任务ID")]
        public int ID;
        [LabelText("任务描述")]
        [TextArea]
        public string Description;
        [LabelText("启动条件")]
        [OdinSerialize, ListDrawerSettings(ShowFoldout = true, DefaultExpandedState = true)]
        public IEventSubscriber[] ActivationSubscribers = new IEventSubscriber[0];
        [LabelText("运行事件")]
        [OdinSerialize, ListDrawerSettings(ShowFoldout = true, DefaultExpandedState = true)]
        public IEventSubscriber[] RuntimeSubscribers = new IEventSubscriber[0];
        [LabelText("是否必要")]
        public bool IsNecessary;
        [LabelText("基础分数")]
        public float Score;
        
    }
}
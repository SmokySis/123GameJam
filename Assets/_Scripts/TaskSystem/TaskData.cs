using EventSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace TaskSystem
{
    public enum TaskDifficulty
    {
        简单=0,
        中等=1,
        困难=2
    }
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
        [LabelText("任务难度")]
        public TaskDifficulty TaskDifficulty= TaskDifficulty.简单;
        [LabelText("任务接取对话"), TextArea]
        public List<string> text;
    }
}
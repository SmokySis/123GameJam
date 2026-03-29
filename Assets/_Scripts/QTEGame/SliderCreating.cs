using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using UnityEngine;

public class SliderCreating : MonoBehaviour
{
    [Header("是否随时间增加,填0（是）或2（否）")]
    public int isUpdate;//是否随时间增加

    public int difficulty;//难度
    [Header("三种难度加的数值,填百分比")]
    public float dif0 = 0f;
    public float dif1 = 0f;
    public float dif2 = 0f;
    public GameObject sliderPrefab;
    public RectTransform panelRect;//承载的panel
    public int taskID;  
    public TaskData taskData;

    private GameObject sliderClone;    
    private void Start()
    {
        taskData = TaskLoader.Instance.GetTaskData(taskID);
        if (taskData != null )
        {
            difficulty = (int)taskData.TaskDifficulty;
        }
        sliderClone = Instantiate(sliderPrefab,panelRect);
        sliderClone.gameObject.SetActive(false);
       
        if (TaskManager.Instance.IsTaskActive(taskID))
        {
            sliderClone.gameObject.SetActive(true);
            sliderClone.GetComponent<ProgressBar>().barMode = (ProgressBar.BarMode)isUpdate;
            sliderClone.GetComponent<ProgressBar>().difficulty = this.difficulty;
            sliderClone.GetComponent<ProgressBar>().dif0 = this.dif0;
            sliderClone.GetComponent<ProgressBar>().dif1 = this.dif1;
            sliderClone.GetComponent<ProgressBar>().dif2 = this.dif2;

        }
    }
    
}

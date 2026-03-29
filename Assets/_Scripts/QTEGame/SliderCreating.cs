using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using UnityEngine;

public class SliderCreating : MonoBehaviour
{
    public GameObject sliderPrefab;
    public RectTransform panelRect;//³ÐÔØµÄpanel
    public int taskID;  

    private GameObject sliderClone;    
    private void Start()
    {
        sliderClone = Instantiate(sliderPrefab,panelRect);
        sliderClone.gameObject.SetActive(false);
       
        if (TaskManager.Instance.IsTaskActive(taskID))
        {
            sliderClone.gameObject.SetActive(true);
        }
    }
    
}

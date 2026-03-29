using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using TaskSystem.Event;
using UnityEngine;
using UnityEngine.UI;

public class TypingManager : MonoBehaviour
{
    public int diffculty;//传入的难度系数
    [Header("三种难度加的数值")]
    public float dif0 = 0f;
    public float dif1 = 0f;
    public float dif2 = 0f;

    private GameObject sliderClone;
    private ProgressBar progressBar;
    bool isStart = false;
    public float startTime = 0.25f;
    private Typing typing;

    public RectTransform panel;
    public GameObject targetText;
    public GameObject inputField;
    private bool isGet;


    private GameObject text;
    private GameObject field;
    private EndThirdQTEEvent endThirdQTEEvent;
    private void Update()
    {
        if (sliderClone == null)
            sliderClone = GameObject.FindWithTag("Slider");
        if (sliderClone != null && !isGet)
        {
            progressBar = sliderClone.GetComponent<ProgressBar>();
            isGet = true;
        }

        if (progressBar == null)
        {
            return;
        }

        if (progressBar.GetProgress() > startTime && !isStart)
        {
            isStart = true;
            text = Instantiate(targetText, panel);
            field = Instantiate(inputField, panel);
            typing = field.GetComponent<Typing>();
        }
        if (field != null && typing.isEqual)
        {
            Score(diffculty);
            Destroy(field);
            Destroy(text);
            TaskManager.Instance.TaskEventCenter.RaiseRunning<EndThirdQTEEvent>(endThirdQTEEvent);
        }
        
    }
    private void Score(int dif)
    {
        ProgressBar gameObject = GameObject.FindWithTag("Slider").gameObject.GetComponent<ProgressBar>();
        switch (dif)
        {
            case 0:
                gameObject.currentProgress += dif0;
                break;
            case 1:
                gameObject.currentProgress += dif1;
                break;
            case 2:
                gameObject.currentProgress += dif2;
                break;

        }
    }
}

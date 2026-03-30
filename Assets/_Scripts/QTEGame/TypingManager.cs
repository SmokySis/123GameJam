using Sirenix.OdinInspector.Editor.GettingStarted;
using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using TaskSystem.Event;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class TypingManager : MonoBehaviour
{
    public int difficulty;//传入的难度系数
    [Header("三种难度加的数值")]
    public float dif0 = 1f;
    public float dif1 = 1f;
    public float dif2 = 1f;

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
    private void OnEnable()
    {
        isStart = false;
        isGet = false;
        typing = null;
        sliderClone = null;
        progressBar = null;        
        text = null;
        field = null;
    }
    private void OnDisable()
    {
        Destroy(field);
        Destroy(text);
    }
    private void Update()
    {
        if (sliderClone == null)
            sliderClone = this.gameObject.GetComponent<SliderCreating>().sliderClone;
        if (sliderClone != null && !isGet)
        {
            progressBar = sliderClone.GetComponent<ProgressBar>();
            difficulty = progressBar.difficulty;
            isGet = true;
        }
        if (!sliderClone.activeSelf)
            return;

        if (progressBar.GetProgress() > startTime && !isStart)
        {
            isStart = true;
            text = Instantiate(targetText, panel);
            field = Instantiate(inputField, panel);
            typing = field.GetComponent<Typing>();
            text.GetComponent<TextGeneration>().difficulty = this.difficulty;
        }
        if (field != null && typing.isEqual)
        {
            Score(difficulty);
            Destroy(field);
            Destroy(text);
            TaskManager.Instance.TaskEventCenter.RaiseRunning<EndThirdQTEEvent>(endThirdQTEEvent);
        }
        
    }
    private void Score(int dif)
    {
        ProgressBar gameObject = sliderClone.GetComponent<ProgressBar>();
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

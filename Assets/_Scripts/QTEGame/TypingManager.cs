using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingManager : MonoBehaviour
{
    public Slider slider;
    private ProgressBar progressBar;
    bool isStart = false;
    public float startTime = 0.25f;
    private Typing typing;

    public RectTransform panel;
    public GameObject targetText;
    public GameObject inputField;

    private GameObject text;
    private GameObject field;
    private void Start()
    {
        progressBar = slider.GetComponent<ProgressBar>();
    }
    private void Update()
    {
        if (progressBar.GetProgress() > startTime && !isStart)
        {
            isStart = true;
            text = Instantiate(targetText, panel);
            field = Instantiate(inputField, panel);
            typing = field.GetComponent<Typing>();
        }
        if (field != null && typing.isEqual)
        {
            Destroy(field);
            Destroy(text);
        }
        
    }

}

using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using UnityEngine;
using UnityEngine.UI;

public class TypingManager : MonoBehaviour
{

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
            Destroy(field);
            Destroy(text);
        }
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLinkLineManager : MonoBehaviour
{
    public Slider slider;
    private ProgressBar progressBar;
    bool isStart = false;
    public float startTime = 0.25f;   

    public RectTransform panel;

    [Header("Ô¤ÖÆ¼þ")]
    public GameObject buttonLinkLine;
    public GameObject buttonLinkLineTemPlate;  
    private ButtonLinkLine buttonLinkLineScript;
    
    private GameObject templete;
    private GameObject linkLine;
    void Start()
    {
        progressBar = slider.GetComponent<ProgressBar>();       
        
    }
    private void Update()
    {
        if (progressBar.GetProgress() > startTime && !isStart)
        {
            isStart = true;
            templete = Instantiate(buttonLinkLineTemPlate, panel);
            linkLine = Instantiate(buttonLinkLine, panel);
            buttonLinkLineScript = linkLine.gameObject.GetComponent<ButtonLinkLine>();
        }

        if (buttonLinkLineScript != null && buttonLinkLineScript.isCompleted)
        {           
            Destroy(templete);
            Destroy(linkLine);
        }
    }

}

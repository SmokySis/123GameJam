using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLinkLineManager : MonoBehaviour
{
    public RectTransform panel;
    public GameObject buttonLinkLine;
    public GameObject buttonLinkLineTemPlate;    
    void Start()
    {
       Instantiate(buttonLinkLineTemPlate, panel); 
       Instantiate(buttonLinkLine,panel); 
    }
    
}

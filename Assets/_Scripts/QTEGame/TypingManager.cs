using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingManager : MonoBehaviour
{
    public RectTransform panel;
    public GameObject targetText;
    public GameObject inputField;
    private void Start()
    {
        Instantiate(targetText,panel);
        Instantiate(inputField,panel);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

public class Typing : MonoBehaviour
{
    public Text myText;
    public Text targetText;
    public bool isEqual;

    private void Start()
    {
        targetText = GameObject.FindWithTag("Template").GetComponent<Text>();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Check();
        }
    }
    public void Check()
    {
        if (myText != null)
        {
            string currentText = myText.text;
            isEqual = currentText == targetText.text;
        }
        if (isEqual)
        {
            print("Good");
        }
        else
        {
            print("Bad");
        }
    }
}

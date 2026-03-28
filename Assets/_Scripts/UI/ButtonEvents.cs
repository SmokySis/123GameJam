using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    [SerializeField]GameObject panel;
    public void StartButtonOnclick()
    {
        panel.SetActive(true);
    }
    public void CloseButtonOnClick()
    {
        panel.SetActive(false);
    }
}

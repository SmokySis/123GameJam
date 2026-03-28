using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    UIController controller;
    private void Awake()
    {
        controller = UIController.Instance;
    }

    [SerializeField]GameObject panel;
    public void StartButtonOnclick() => panel.SetActive(true);
    public void CloseButtonOnClick() => panel.SetActive(false);
    public void StartSettingButtonOnClick() => controller.SetPause();
    public void CloseSettingButtonOnClick() => controller.SetPlay();
    public void StartAudioOnClick() => controller.SetAudioActive();
    public void CloseAudioOnClick() => controller.CloseAudioPanel();

    public void BackToMainMenuOnClick()
    {

    }
}

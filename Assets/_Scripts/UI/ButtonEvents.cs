using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaskSystem;
using TaskSystem.Test;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    UIController controller;
    private void Awake()
    {
        controller = UIController.Instance;
    }

    [SerializeField] GameObject panel;
    public void StartButtonOnclick() { panel.SetActive(true); TaskManager.Instance.TaskEventCenter.RaiseBegin(new APPOpenEvent()); AudioManager.Instance.Play("Au_UI_Click"); }
    public void CloseButtonOnClick() { panel.SetActive(false); AudioManager.Instance.Play("Au_UI_Click"); }
    public void StartSettingButtonOnClick() { controller.SetPause(); AudioManager.Instance.Play("Au_UI_Click"); }
    public void CloseSettingButtonOnClick() { controller.SetPlay(); AudioManager.Instance.Play("Au_UI_Click"); }
    public void StartAudioOnClick() { controller.SetAudioActive(); AudioManager.Instance.Play("Au_UI_Click"); }
    public void CloseAudioOnClick() { controller.CloseAudioPanel(); AudioManager.Instance.Play("Au_UI_Click"); }

    public void StartPlatformButtonOnClick() { if (GameController.Instance.isActive) return; panel.SetActive(true); GameController.Instance.LoadGame(); AudioManager.Instance.Play("Au_UI_Click"); }
    public void ClosePlatformButtonOnClick() { GameController.Instance.ResetGame(); AudioManager.Instance.Play("Au_UI_Click"); panel.SetActive(false); }

    public void BackToMainMenuOnClick() { LoadSceneManager.Instance.LoadMenuScene(); AudioManager.Instance.Play("Au_UI_Click"); }

    public void LoadMainSceneOnClick() { LoadSceneManager.Instance.LoadMainScene();}

    public void ExitGameOnClick() {AudioManager.Instance.Play("Au_UI_Click"); Application.Quit();}
}

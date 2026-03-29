using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum WindowState
{
    Foreground, Background
}
public class Window : MonoBehaviour
{
    [LabelText("窗口名称")]
    public string Name;
    [LabelText("窗口名称UI")]
    public Text NameUI;
    [LabelText("窗口图标")]
    public Sprite Icon;
    [LabelText("窗口图标UI")]
    public Image IconUI;
    [LabelText("窗口前台耗电量")]
    public float ForegroundDrain = 0;
    [LabelText("窗口后台耗电量")]
    public float BackgroundDrain = 0;
    [HideInInspector]
    public float TickTime = 0;
    [HideInInspector]
    public WindowState WindowState;
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        if (NameUI) NameUI.text = Name;
        if (IconUI) IconUI.sprite = Icon;
        WindowState = transform.GetSiblingIndex() == 0 ? WindowState.Foreground : WindowState.Background;
    }
    public float ConsumePower(float rate=1)
    {
        TickTime += Time.deltaTime;
        if (TickTime > 1)
            switch (WindowState)
            {
                case WindowState.Foreground: TickTime = 0; return ForegroundDrain;
                case WindowState.Background: TickTime = 0; return BackgroundDrain*rate;
            }
        return 0;
    }
}

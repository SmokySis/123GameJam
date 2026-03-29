using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePanelEvent : PanelClickEvents
{
    [SerializeField] GameObject _panel;
    private void Awake()
    {
        controller = UIController.Instance;
    }
    protected override void PanelOnClick_Left()
    {
        Debug.Log("OnClick");
        controller.StopMessageCoroutine();
        controller.ResetMessagePanel();
        _panel.SetActive(true);
        //controller.DetailPanel.SetActive(true);
        //controller.StartShowDialogue();
    }

    protected override void PanelOnClick_Right()
    {
    }

    protected override void PanelOnClick_Top()
    {
    }
}

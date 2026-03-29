using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePanelEvent : PanelClickEvents
{
    private void Awake()
    {
        controller = UIController.Instance;
    }
    protected override void PanelOnClick_Left()
    {
        Debug.Log("OnClick");
        controller.StopMessageCoroutine();
        controller.ResetMessagePanel();
        controller.DetailPanel.SetActive(true);
    }

    protected override void PanelOnClick_Right()
    {
    }

    protected override void PanelOnClick_Top()
    {
    }
}

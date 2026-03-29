using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PanelClickEvents : MonoBehaviour, IPointerClickHandler
{
    protected UIController controller;
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left: { PanelOnClick_Left(); break; }
            case PointerEventData.InputButton.Right: { PanelOnClick_Right(); break; }
            default: { PanelOnClick_Top(); break; }
        }

    }

    protected abstract void PanelOnClick_Left();

    protected abstract void PanelOnClick_Right();

    protected abstract void PanelOnClick_Top();
}

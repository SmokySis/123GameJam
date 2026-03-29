using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstReport : MonoBehaviour,IPointerClickHandler
{
  public void OnPointerClick(PointerEventData eventData)
    {
        WindowsController.Instance.OpenTaskCo();
        gameObject.SetActive(false);
    }
}

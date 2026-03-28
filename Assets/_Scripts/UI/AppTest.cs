using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppTest : MonoBehaviour
{
   
    void Update()
    {
        UIController.Instance.batteryPowerPercent -= 0.01f * Time.deltaTime;
    }
}

using System.Collections;
using System;
using Utility;
using UnityEngine;

public class CoroutineRunner : Singleton<CoroutineRunner>
{
    //protected override bool _isDonDestroyOnLoad => true;
    public void Delay(float seconds, Action action)=> StartCoroutine(DelayCoroutine(seconds, action));

    private IEnumerator DelayCoroutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
}

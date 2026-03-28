using PoolSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public enum CreatePos
{
    Upper = 0,
    Lower = 1,
    Down = 2,
}

public class BarrierController : Singleton<BarrierController>
{
    [Header("速度参数")]
    [SerializeField] float normalSpeed = 5f;
    [SerializeField] float dashSpeed = 10f;
    [Header("障碍物预制体")]
    GameObject BarrierPrefab;
    [Header("障碍物刷新点")]
    [SerializeField] Vector3 UpperPosition;
    [SerializeField] Vector3 MiddlePosition;
    [SerializeField] Vector3 LowerPosition;
    [Header("所有已激活障碍的列表")]
    LinkedList<Barrier> barriers = new LinkedList<Barrier>();

    void RefreshBarrier(GameObject barrier)
    {
        if (!barrier.GetComponent<Barrier>()) barrier.AddComponent<Barrier>();


    }

    public void CreateNewBarrier(CreatePos pos)
    {
        Vector3 createPos = Vector3.zero;

        switch (pos)
        {
            case CreatePos.Upper: { createPos = UpperPosition; break; }
            case CreatePos.Lower: { createPos = LowerPosition; break; }
            default: { createPos = MiddlePosition; break; }
        }

        GameObject creation = GameObjectPoolCenter.Instance.GetInstance(BarrierPrefab, createPos, Quaternion.identity);
        RefreshBarrier(creation);
    }
}

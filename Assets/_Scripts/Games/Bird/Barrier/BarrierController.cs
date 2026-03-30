using PoolSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utility;

public enum CreatePos
{
    Upper = 0,
    Lower = 1,
    Middle = 2,
}

public class BarrierController : Singleton<BarrierController>
{
    [Header("速度参数")]
    [SerializeField] float normalSpeed = 5f;
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float SpeedUPSpeed = 8f;
    [Header("障碍物长度参数")]
    [SerializeField] float barrierMinLength = 0f;
    [SerializeField] float barrierMaxLength = 0f;
    float currentSpeed = 0;
    [Header("障碍物预制体")]
    GameObject BarrierPrefab;
    [Header("障碍物刷新点")]
    [SerializeField] Vector3 UpperPosition;
    [SerializeField] Vector3 MiddlePosition;
    [SerializeField] Vector3 LowerPosition;
    [Header("障碍物回收界限")]
    float minPosition = -20f;
    [Header("所有已激活障碍的列表")]
    LinkedList<Barrier> barriers = new LinkedList<Barrier>();
    [Header("事件")]
    UnityEvent<float> UpdateSpeedEvent;
    [Header("刷新距离")]
    [SerializeField] float createDistance;
    [SerializeField] float currentDis = 0;
    float totalDistance = 0;
    [Header("获胜距离")]
    float victoryDistance = BirdGameController.Instance.victoryDistance;

    void RefreshBarrier(GameObject barrier,float length)
    {
        if (!barrier.GetComponent<Barrier>()) barrier.AddComponent<Barrier>();
        barriers.AddLast(barrier.GetComponent<Barrier>());
        barrier.GetComponent<Rigidbody2D>().velocity = new Vector2(-1 * currentSpeed,0);
        UpdateSpeedEvent.AddListener(barrier.GetComponent<Barrier>().UpdateSpeed);
    }

    public void ReleaseBarrier()
    {
        Barrier release = barriers.First?.Value;
        UpdateSpeedEvent.RemoveListener(release.UpdateSpeed);
        release.ResetSpeed();
    }

    public void UpdateSpeed(BirdController.FlyState state)
    {
        switch (state)
        {
            case BirdController.FlyState.Normal: { currentSpeed  = normalSpeed; break; }
            case BirdController.FlyState.SpeedUP: { currentSpeed = SpeedUPSpeed; break; }
            case BirdController.FlyState.Dashing: { currentSpeed = dashSpeed; break; };
            case BirdController.FlyState.Dead:   { currentSpeed = 0; break; };
        }

        UpdateSpeedEvent?.Invoke(currentSpeed);
    }

    public void CreateNewBarrier(CreatePos pos,float length)
    {
        Vector3 createPos = Vector3.zero;

        switch (pos)
        {
            case CreatePos.Upper: { createPos = UpperPosition; break; }
            case CreatePos.Lower: { createPos = LowerPosition; break; }
            default: { createPos = MiddlePosition; break; }
        }

        GameObject creation = GameObjectPoolCenter.Instance.GetInstance(BarrierPrefab, createPos, Quaternion.identity);
        RefreshBarrier(creation,length);
    }

    void SetBarrier()
    {
        float length = Random.Range(barrierMinLength, barrierMaxLength);
        int posID = Random.Range(0, 3);
        CreateNewBarrier((CreatePos)posID, length);
        currentDis -= createDistance;
    }

    private void Update()
    {
        currentDis += Time.deltaTime * currentSpeed;
        totalDistance += Time.deltaTime * currentSpeed;
        if (totalDistance > victoryDistance) BirdGameController.Instance.FinishGame();
        if (currentDis >= createDistance) SetBarrier();
    }

    public float GetMinPos() => minPosition;
}

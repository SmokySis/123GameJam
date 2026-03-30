using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    float minPos;
    Rigidbody2D rb;
    BarrierController controller;
    private void Awake()
    {
        if (GetComponent<Rigidbody2D>() == null) { gameObject.AddComponent<Rigidbody2D>(); }
        rb = GetComponent<Rigidbody2D>();
        controller = BarrierController.Instance;
        minPos = controller.GetMinPos();
    }

    public void UpdateSpeed(float newSpeed)
    {
        rb.velocity = new Vector2 (-1 *  newSpeed, 0);
    }

    public void ResetSpeed() { rb.velocity = Vector2.zero; }

    private void Update()
    {
        if (transform.position.x <= minPos) controller.ReleaseBarrier();
    }
}

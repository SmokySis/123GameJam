using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stokes : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null) GameController.Instance.Failure();
    }
}

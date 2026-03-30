using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] GameObject currentPlayer = null;
    Transform playerTransform = null;
    public void SetPlayer(GameObject player) { currentPlayer = player;if (player != null) playerTransform = player.transform; else playerTransform = null; }

    public void Reset() {currentPlayer = null; playerTransform = null; transform.position = Vector2.zero; }

    private void Start()
    {
        if (currentPlayer != null) playerTransform = currentPlayer.transform;
        else transform.position = Vector2.zero;
    }

    private void Update()
    {
        //Debug.Log(playerTransform ? playerTransform.position : "null");
        if (currentPlayer != null) transform.position = new Vector3(playerTransform.position.x, playerTransform.transform.position.y, transform.position.z);
    }
}

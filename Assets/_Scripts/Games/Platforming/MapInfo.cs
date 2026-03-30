using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    [SerializeField] GameObject mapPlayer;
    [SerializeField] Vector2 InitPlayerPos;
    public int mapID = -1;

    public GameObject GetPlayer() => mapPlayer;
    public void ResetPlayer()  { mapPlayer.transform.position = InitPlayerPos; mapPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero; }

    private void Awake()
    {
        if (!GameController.Instance.Maps.Contains(this)) GameController.Instance.Maps.Add(this);
    }
}

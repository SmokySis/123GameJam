using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class GameController : Singleton<GameController>
{
    [SerializeField]public List<MapInfo> Maps = new List<MapInfo>();
    [SerializeField] GameObject gamePanel;
    MapInfo activeMap = null;
    public bool isActive = false;
    public void LoadGame()
    {
        isActive = true;
        int mapID = Random.Range(0,Maps.Count);
        for (int i = 0; i < Maps.Count; i++)
        {
            if (Maps[i].mapID == mapID) activeMap = Maps[i];
        }

        activeMap.gameObject.SetActive(true);
        CameraController.Instance.SetPlayer(activeMap.GetPlayer());
        Debug.Log(activeMap.GetPlayer());
    }

    public void ResetGame()
    {
        isActive = false;
        activeMap.gameObject.SetActive(false);
        activeMap.ResetPlayer();
        CameraController.Instance.SetPlayer(null);
        activeMap = null;
    }

    public void FinishGame(float reduceTiring)
    {
        UIController.Instance.tiringPercent -= reduceTiring;
        GameObject.FindWithTag("Score").GetComponent<Text>().text = (System.Convert.ToSingle(GameObject.FindWithTag("Score").GetComponent<Text>().text) + 1000).ToString();
        ResetGame();
        if (gamePanel) gamePanel.SetActive(false);
    }

    public void Failure()
    {
        activeMap.ResetPlayer();
    }
}

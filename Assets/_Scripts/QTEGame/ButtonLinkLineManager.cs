using Sirenix.OdinInspector.Editor.GettingStarted;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TaskSystem;
using TaskSystem.Event;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLinkLineManager : MonoBehaviour
{

    public int difficulty;//传入的难度系数
    [Header("三种难度加的数值")]
    public float dif0 = 1f;
    public float dif1 = 1f;
    public float dif2 = 1f;

    private GameObject sliderClone;
    private ProgressBar progressBar;
    private bool isStart = false;
    private bool isGet = false;
    public float startTime = 0.25f;   

    public RectTransform panel;



    [Header("预制件")]
    public GameObject buttonLinkLine;
    //public GameObject buttonLinkLineTemPlate;  
    private ButtonLinkLine buttonLinkLineScript;
    
    //private GameObject templete;
    private GameObject linkLine;


    public int spawnCount = 9;
    public float squareSize = 200f;
    public float minDistance = 210f;
    private List<Vector2> spawnPoints;
    private bool isGenerate = false;

    private EndSecondQTEEvent endSecondQTEEvent;
    private void OnEnable()
    {
        isStart = false;
        isGet = false;
        isGenerate = false;
        spawnPoints = new List<Vector2>();
        sliderClone = null;
        progressBar = null;
        buttonLinkLineScript = null;
        linkLine = null;
    }
    private void OnDisable()
    {
        Destroy(linkLine);
    }

    private void Update()
    {
        if (sliderClone == null)
            sliderClone = this.gameObject.GetComponent<SliderCreating>().sliderClone;
        if (sliderClone != null && !isGet)
        {
            progressBar = sliderClone.GetComponent<ProgressBar>();
            difficulty = progressBar.difficulty;
            isGet = true;
        }
        if (!sliderClone.activeSelf)
            return;

        switch (difficulty)
        {
            case 0:
                spawnCount = 4;
                break;
            case 1:
                spawnCount = 6;
                break;
            case 2:
                spawnCount = 9;
                break;
            
        }


        if (progressBar.GetProgress() > startTime && !isStart)
        {
            isStart = true;
            //templete = Instantiate(buttonLinkLineTemPlate, panel);
            linkLine = Instantiate(buttonLinkLine, panel);
            buttonLinkLineScript = linkLine.gameObject.GetComponent<ButtonLinkLine>();
            GenerateNonOverlappingPoints();
            if(spawnPoints.Count == spawnCount && !isGenerate)
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    RectTransform squareRect = buttonLinkLineScript.targetButtons[i].GetComponent<RectTransform>();
                    if (squareRect != null)
                    {
                        squareRect.anchoredPosition = spawnPoints[i];
                        squareRect.sizeDelta = new Vector2(squareSize, squareSize);
                    }               
                }
                for (int i = spawnCount; i < 9; i++)
                {
                    Destroy(buttonLinkLineScript.targetButtons[i].gameObject);
                    buttonLinkLineScript.targetButtons[i] = null;

                }
                buttonLinkLineScript.targetButtons = buttonLinkLineScript.targetButtons.Take(spawnCount).ToArray();//截断

            }
        }

        if (buttonLinkLineScript != null && buttonLinkLineScript.isCompleted)
        {
            //Destroy(templete);
            Score(difficulty);
            Destroy(linkLine);
            TaskManager.Instance.TaskEventCenter.RaiseRunning<EndSecondQTEEvent>(endSecondQTEEvent);
        }
    }
    void GenerateNonOverlappingPoints()
    {
        //清空列表
        spawnPoints.Clear();
        int maxAttempts = 100; // 防止死循环

        while (spawnPoints.Count < spawnCount && maxAttempts > 0)
        {
            // 生成 Panel 范围内的随机坐标
            Vector2 randomPos = new Vector2(
                UnityEngine.Random.Range(panel.rect.xMin + squareSize / 2, panel.rect.xMax - squareSize / 2),
                UnityEngine.Random.Range(panel.rect.yMin + squareSize / 2, panel.rect.yMax - squareSize / 2)
            );

            // 检查是否与已有点距离过近
            bool isOverlapping = false;
            foreach (Vector2 point in spawnPoints)
            {
                if (Vector2.Distance(randomPos, point) < minDistance)
                {
                    isOverlapping = true;
                    maxAttempts--;
                    break;
                }
            }

            if (!isOverlapping)
            {
                spawnPoints.Add(randomPos);
                maxAttempts = 100; // 重置尝试次数
            }
        }

    }
    private void Score(int dif)
    {
        ProgressBar gameObject = GameObject.FindWithTag("Slider").gameObject.GetComponent<ProgressBar>();
        switch (dif)
        {
            case 0:
                gameObject.currentProgress += dif0;
                break;
            case 1:
                gameObject.currentProgress += dif1;
                break;
            case 2:
                gameObject.currentProgress += dif2;
                break;

        }
    }
}

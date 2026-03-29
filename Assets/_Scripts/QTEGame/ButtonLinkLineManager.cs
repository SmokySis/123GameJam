using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLinkLineManager : MonoBehaviour
{
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
    private List<Vector2> spawnPoints = new List<Vector2>();
    private bool isGenerate = false;    
    private void Update()
    {
        if (sliderClone == null)
            sliderClone = GameObject.FindWithTag("Slider");
        if (sliderClone != null && !isGet)
        {
            progressBar = sliderClone.GetComponent<ProgressBar>();
            isGet = true;
        }
        if (progressBar == null)
            return;
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
                                   
            }
        }

        if (buttonLinkLineScript != null && buttonLinkLineScript.isCompleted)
        {           
            //Destroy(templete);
            Destroy(linkLine);
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
                Random.Range(panel.rect.xMin + squareSize / 2, panel.rect.xMax - squareSize / 2),
                Random.Range(panel.rect.yMin + squareSize / 2, panel.rect.yMax - squareSize / 2)
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
}

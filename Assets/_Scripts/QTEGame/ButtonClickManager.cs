using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickManager : MonoBehaviour
{


    private GameObject sliderClone;
    private ProgressBar progressBar;
    public GameObject buttonClickPrefab;
    public RectTransform panelRect;//承载的panel
                                   
    public int difficulty;//难度系数
    [Header("三种难度加的数值")]
    public float dif0 = 0.1f;
    public float dif1 = 0.05f;
    public float dif2 = 0.02f;

    [Header("三种难度速度变化")]
    public float speed0 = 0f;
    public float speed1 = 0f;
    public float speed2 = 0f;

    public int spawnCount = 10;
    public float squareSize = 200f;    
    public float minDistance = 210f;
    private List<Vector2> spawnPoints = new List<Vector2>();
    bool isGet;
    bool isStart = false;
    bool isGenerate = false;
    public float startTime = 0.25f;
    private void OnEnable()
    {
        isStart = false;
        isGet = false;
        isGenerate = false;
        spawnPoints = new List<Vector2>();
        sliderClone = null;
        progressBar = null;        
    }
    private void OnDisable()
    {
        
    }
    private void Update()
    {
        if (sliderClone == null)
            sliderClone = GameObject.FindWithTag("Slider");
        if (sliderClone != null && !isGet)
        {
            progressBar = sliderClone.GetComponent<ProgressBar>();
            difficulty = progressBar.difficulty;
            isGet = true;
        }
        if (progressBar == null)
            return;
        //等到开始任务后几秒再开始造点
        if(progressBar.GetProgress() > startTime && !isStart)
        {
            isStart = true;
            GenerateNonOverlappingPoints();
        }
        if(spawnPoints.Count == 10 && !isGenerate)
        {
            isGenerate = true;
            StartCoroutine(Generate());        
        }
        
    }
    // 生成不重叠的随机点
    void GenerateNonOverlappingPoints()
    {
        //清空列表
        spawnPoints.Clear();
        int maxAttempts = 100; // 防止死循环

        while (spawnPoints.Count < spawnCount && maxAttempts > 0)
        {
            // 生成 Panel 范围内的随机坐标
            Vector2 randomPos = new Vector2(
                Random.Range(panelRect.rect.xMin + squareSize / 2, panelRect.rect.xMax - squareSize / 2),
                Random.Range(panelRect.rect.yMin + squareSize / 2, panelRect.rect.yMax - squareSize / 2)
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
    IEnumerator Generate()
    {
        foreach (Vector2 pos in spawnPoints)
        {
            GameObject square = Instantiate(buttonClickPrefab, panelRect);
            square.GetComponent<ButtonClick>().diffculty = this.difficulty;//设置难度
            square.GetComponent<ButtonClick>().dif0 = this.dif0;
            square.GetComponent<ButtonClick>().dif1 = this.dif1;
            square.GetComponent<ButtonClick>().dif2 = this.dif2;
            square.GetComponent<ButtonClick>().speed0 = this.speed0;
            square.GetComponent<ButtonClick>().speed1 = this.speed1;
            square.GetComponent<ButtonClick>().speed2 = this.speed2;


            RectTransform squareRect = square.GetComponent<RectTransform>();
            if (squareRect != null)
            {
                squareRect.anchoredPosition = pos;
                squareRect.sizeDelta = new Vector2(squareSize, squareSize);
            }
            yield return new WaitForSeconds(1f);
        }

    }
}

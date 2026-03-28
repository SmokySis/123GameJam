using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickManager : MonoBehaviour
{
    public GameObject buttonClickPrefab;
    public RectTransform panelRect;//承载的panel   
    public int spawnCount = 10;
    public float squareSize = 100f;    
    public float minDistance = 110f;
    private List<Vector2> spawnPoints = new List<Vector2>();
    private void Start()
    {
        GenerateNonOverlappingPoints();
        if(spawnPoints.Count == 10)
        {
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

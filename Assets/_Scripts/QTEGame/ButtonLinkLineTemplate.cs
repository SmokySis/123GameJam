using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonLinkLineTemplate : MonoBehaviour
{
    public RectTransform panel;
    public GameObject linePrefab;
    public List<Button> targetButtons;
    public Button[] buttons;
    private List<GameObject> lineList = new List<GameObject>();

    void Start()
    {
        ShuffleList<Button>(targetButtons);
        for (int i = 0; i < targetButtons.Count - 1; i++)
        {
            DrawLineBetween(targetButtons[i], targetButtons[i + 1]);
        }
    }

    /// <summary>
    /// 洗牌算法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    void DrawLineBetween(Button from, Button to)
    {
        // 生成线
        GameObject line = Instantiate(linePrefab, panel);
        RectTransform lineRect = line.GetComponent<RectTransform>();
        RectTransform fromRect = from.GetComponent<RectTransform>();
        RectTransform toRect = to.GetComponent<RectTransform>();

        // 两点位置
        Vector2 fromPos = fromRect.anchoredPosition;
        Vector2 toPos = toRect.anchoredPosition;

        // 中点
        lineRect.anchoredPosition = (fromPos + toPos) / 2f;

        // 长度
        float distance = Vector2.Distance(fromPos, toPos);
        lineRect.sizeDelta = new Vector2(distance, 8); // 8是线宽

        // 角度
        Vector2 dir = toPos - fromPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineRect.localEulerAngles = new Vector3(0, 0, angle);

        lineList.Add(line);
    }

}
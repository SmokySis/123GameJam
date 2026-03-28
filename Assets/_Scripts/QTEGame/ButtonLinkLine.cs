using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


public class ButtonLinkLine : MonoBehaviour
{        
    public RectTransform panel;
    public GameObject linePrefab;   
    public Button[] targetButtons; //这个数组用于计录按钮 
    public List<Button> clickList = new List<Button>();
    private List<GameObject> lineList = new List<GameObject>();
    private ButtonLinkLineTemplate template;
    public int[] buttons = new int[9];//这个数组用于接收模板的顺序索引
    public int[] myButtons = new int[9];//这个数组用于计录点击连线的索引  
    public bool isCompleted = false;

    void Start()
    {        
        template = GameObject.FindWithTag("Template").GetComponent<ButtonLinkLineTemplate>();
        // 遍历所有按钮，给每个按钮添加点击事件
        for (int i = 0; i < targetButtons.Length; i++)
        {
            int index = i;
            targetButtons[index].onClick.AddListener(() => OnBtnClick(targetButtons[index]));
        }
        
    }

    /// <summary>
    /// 按钮点击时执行
    /// </summary>
    void OnBtnClick(Button btn)
    {
        // 如果这个按钮已经点过了，直接返回，不重复添加
        if (clickList.Contains(btn))
            return;

        // 把当前点击的按钮加入列表
        clickList.Add(btn);

        // 至少两个点才画线
        if (clickList.Count >= 2)
        {
            Button lastBtn = clickList[clickList.Count - 2];
            Button currBtn = clickList[clickList.Count - 1];

            DrawLineBetween(lastBtn, currBtn);
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

    /// <summary>
    /// 清空所有连线和记录，重新开始
    /// </summary>
    public void ClearAll()
    {
        foreach (var line in lineList)
        {
            Destroy(line);
        }
        lineList.Clear();
        clickList.Clear();
    }
    /// <summary>
    /// 提交
    /// </summary>
    public void Check()
    {
        for (int i = 0; i < template.targetButtons.Count; i++)
        {
            int index = System.Array.IndexOf(template.buttons, template.targetButtons[i]);
            buttons[i] = index;//接收顺序      
        }
        for (int i = 0; i < clickList.Count; i++)
        {
            int myIndex = System.Array.IndexOf(targetButtons, clickList[i]);
            myButtons[i] = myIndex;//接收顺序
        }
        bool same = myButtons.SequenceEqual(buttons);
        bool same2 = myButtons.SequenceEqual(buttons.Reverse());

        if (same || same2)
        {
            print("Good");
        }
        else
        {
            print("Bad");
        }
        isCompleted = same || same2;
    }  
    
}
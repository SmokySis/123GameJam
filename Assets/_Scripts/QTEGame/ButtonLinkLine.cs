using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


public class ButtonLinkLine : MonoBehaviour
{        
    public RectTransform panel;
    public GameObject linePrefab;
    public Camera uiCamera;



    public Button[] targetButtons; //这个数组用于计录按钮 
    public int maxClickCount = 9;     // 最多点几个按钮（数组上限）

    public List<Button> clickList = new List<Button>();
    private List<GameObject> lineList = new List<GameObject>();
    //private ButtonLinkLineTemplate template;

    private GameObject currentFollowLine; // 当前跟随鼠标的线
    private Button lastBtn;

    //public int[] buttons = new int[9];//这个数组用于接收模板的顺序索引
    //public int[] myButtons = new int[9];//这个数组用于计录点击连线的索引  
    public bool isCompleted = false;

    void Start()
    {        
        //template = GameObject.FindWithTag("Template").GetComponent<ButtonLinkLineTemplate>();
        // 遍历所有按钮，给每个按钮添加点击事件
        for (int i = 0; i < targetButtons.Length; i++)
        {
            int index = i;
            targetButtons[index].onClick.AddListener(() => OnBtnClick(targetButtons[index]));
        }
        uiCamera = GameObject.FindWithTag("MainCamera").gameObject.GetComponent<Camera>();
    }
    void Update()
    {      
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Check();
        }   
        // 如果有正在跟随鼠标的线，并且还没点满
        if (currentFollowLine != null && clickList.Count < maxClickCount)
        {
            UpdateFollowLineToMouse();
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
        // 已经点满了，不再响应
        if (clickList.Count >= maxClickCount)
            return;
        // 把当前点击的按钮加入列表
        clickList.Add(btn);
        
        // 如果是第一个按钮：只创建跟随线
        if (clickList.Count == 1)
        {
            lastBtn = btn;
            CreateFollowLine(btn);
        }
    
        else
        {
            // 固定上一条跟随线到当前按钮
            FixLastLineTo(btn);

            // 没点满才继续创建新跟随线
            if (clickList.Count<maxClickCount)
            {
                lastBtn = btn;
                CreateFollowLine(btn);
            }
            else
            {
                currentFollowLine = null;
            }
        }
    }
    // 创建一条：一端粘按钮，另一端跟鼠标
    void CreateFollowLine(Button fromBtn)
    {
        var line = Instantiate(linePrefab, panel);
        RectTransform fromRect = fromBtn.GetComponent<RectTransform>();
        RectTransform lineRect = line.GetComponent<RectTransform>();

        // 初始化位置，避免瞬间闪一下
        Vector2 a = fromRect.anchoredPosition;
        lineRect.anchoredPosition = a;
        lineRect.sizeDelta = new Vector2(0, 8);
        currentFollowLine = line;
        lineList.Add(line);
    }
    // 把跟随线固定到目标按钮
    void FixLastLineTo(Button targetBtn)
    {
        if (currentFollowLine == null) return;

        var fromRect = lastBtn.GetComponent<RectTransform>();
        var toRect = targetBtn.GetComponent<RectTransform>();
        var lineRect = currentFollowLine.GetComponent<RectTransform>();

        Vector2 a = fromRect.anchoredPosition;
        Vector2 b = toRect.anchoredPosition;

        lineRect.anchoredPosition = (a + b) / 2f;
        lineRect.sizeDelta = new Vector2(Vector2.Distance(a, b), 8);

        float angle = Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
        lineRect.localEulerAngles = new Vector3(0, 0, angle);
    }
    // 更新跟随线：一端 lastBtn，一端鼠标
    void UpdateFollowLineToMouse()
    {
        var lineRect = currentFollowLine.GetComponent<RectTransform>();
        var fromRect = lastBtn.GetComponent<RectTransform>();

        Vector2 a = fromRect.anchoredPosition;
        //正确转换鼠标坐标到 UI 局部坐标（修复关键）
        Vector2 mouseUIPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panel,                  // 你的父面板
            Input.mousePosition,    // 鼠标屏幕坐标
            uiCamera,                   // 画布相机 Overlay 模式填 null
            out mouseUIPos          // 输出正确的 UI 坐标
        );

        Vector2 b = mouseUIPos;

        lineRect.anchoredPosition = (a + b) / 2f;
        lineRect.sizeDelta = new Vector2(Vector2.Distance(a, b), 8);

        float angle = Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
        lineRect.localEulerAngles = new Vector3(0, 0, angle);
    }

    /// <summary>
    /// 清空所有连线和记录，重新开始
    /// </summary>
    public void ClearAll()
    {
        foreach (var l in lineList)
        {
            Destroy(l);
        }
        lineList.Clear();
        clickList.Clear();
        currentFollowLine = null;
        lastBtn = null;
    }
    /// <summary>
    /// 提交
    /// </summary>
    public void Check()
    {
        //for (int i = 0; i < template.targetButtons.Count; i++)
        //{
        //    int index = System.Array.IndexOf(template.buttons, template.targetButtons[i]);
        //    buttons[i] = index;//接收顺序      
        //}
        //for (int i = 0; i < clickList.Count; i++)
        //{
        //    int myIndex = System.Array.IndexOf(targetButtons, clickList[i]);
        //    myButtons[i] = myIndex;//接收顺序
        //}
        //bool same = myButtons.SequenceEqual(buttons);
        //bool same2 = myButtons.SequenceEqual(buttons.Reverse());

        //if (same || same2)
        //{
        //    print("Good");
        //}
        //else
        //{
        //    print("Bad");
        //}
        if (clickList.Count == maxClickCount)
        {
            isCompleted = true;
            print("Good");
        }
        else
        {
            print("Bad");
        }
    }  
    
}
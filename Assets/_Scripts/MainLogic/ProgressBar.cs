using UnityEngine;
using UnityEngine.UI;
using TaskSystem;
using TaskSystem.Event;
using TaskSystem.Subscriber;

public class ProgressBar : MonoBehaviour
{    
    public enum BarMode
    {
        AutoFill,         // 纯自动读条
        QTEDrive,         // QTE驱动（不按不动）
        QTEOptional       // QTE选做
    }
    public Slider slider;
    public int id;
    public float totalTime = 100f;      
    public bool autoStart = true;
    public SliderCreating sc;

    public BarMode barMode;
    public int difficulty;//难度
   
    public float dif0 = 0f;
    public float dif1 = 0f;
    public float dif2 = 0f;

    // 当前进度
    public float currentProgress;
    private bool isRunning;

    

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        slider.minValue = 0;
        slider.maxValue = 1;
    }

    void Start()
    {
        if (autoStart)
            StartBar();
    }

    void Update()
    {
        if (!isRunning) return;
        
        if (barMode == BarMode.AutoFill)
        {
            currentProgress += Time.deltaTime * AddProgress(difficulty);
        }       
        //else if (barMode == BarMode.QTEOptional)
        //{
        //    currentProgress += Time.deltaTime / totalTime;

            
        //}
        else if (barMode == BarMode.QTEDrive)
        {
            
        }
        // 限制 0~1
        currentProgress = Mathf.Clamp01(currentProgress);
        slider.value = currentProgress;
        // 读条完成
        if (currentProgress >= 1)
        {
            OnBarComplete();
            switch (id % 1000)
            {               
                case 1:
                    TaskManager.Instance.TaskEventCenter.RaiseRunning(new End1Event() {ID=id });
                    break;
                case 2:
                    TaskManager.Instance.TaskEventCenter.RaiseRunning(new End2Event() { ID = id });
                    break;
                case 3:
                    TaskManager.Instance.TaskEventCenter.RaiseRunning(new End3Event() { ID = id });
                    break;              
               
            }
            sc.taskID++;

            AddScore();//计算分数加分

        }
    }

    // 开始读条
    public void StartBar()
    {
        isRunning = true;
        currentProgress = 0;
        slider.value = 0;
    }

    // 停止读条
    public void StopBar()
    {
        isRunning = false;
    }

    // 读条完成
    void OnBarComplete()
    {
        StopBar();
        Debug.Log("进度条完成！");
        
    }

    private float AddProgress(int dif)
    {        
        switch (dif)
        {
            case 0:
                return dif0;               
            case 1:
                return dif1;               
            case 2:
                return dif2;
            default:
                return 0f;
        }
    }
    //外部获取当前进度
    //可用于QTE弹出时机设置
    public float GetProgress()
    {
        return currentProgress;
    }
    public void AddScore()
    {
        //需要一个Score
        Text text = GameObject.FindWithTag("Score").GetComponent<Text>();
        //需要一个Magnification
        float mag = GameObject.FindWithTag("Magnification").GetComponent<Magnification>().GetCurrentMagnification();
        float scr = sc.taskData.Score;
        float res = scr * mag;

        float temp = System.Convert.ToSingle(text.text);
        temp += res;
        text.text = System.Convert.ToString(temp);

    }
}
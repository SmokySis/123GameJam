using UnityEngine;
using UnityEngine.UI;
using TaskSystem;
using TaskSystem.Event;

public class ProgressBar : MonoBehaviour
{    
    public enum BarMode
    {
        AutoFill,         // 纯自动读条
        QTEDrive,         // QTE驱动（不按不动）
        QTEOptional       // QTE选做
    }
    public Slider slider;          
    public float totalTime = 20f;      
    public bool autoStart = true;  
    
    public BarMode barMode;


    // 当前进度
    private float currentProgress;
    private bool isRunning;

    private EndProgressBarEvent endProgressBarEvent;

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
            currentProgress += Time.deltaTime / totalTime;
        }       
        else if (barMode == BarMode.QTEOptional)
        {
            currentProgress += Time.deltaTime / totalTime;

            
        }
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
            TaskManager.Instance.TaskEventCenter.RaiseRunning<EndProgressBarEvent>(endProgressBarEvent);//对吗
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

    //外部获取当前进度
    //可用于QTE弹出时机设置
    public float GetProgress()
    {
        return currentProgress;
    }
}
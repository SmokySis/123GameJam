using UnityEngine;
using UnityEngine.UI;

public class MagnificationSystem : MonoBehaviour
{
    [Header("倍率设置")]
    public float magnification = 1.0f; // 当前倍率
    public float resetTime = 5f;       // 多久没增长就重置

    private float _lastIncreaseTime;          // 最后一次增长时间
    public  Slider[] _targetSliders;          // 所有带Tag的Slider
    public  float[] _lastSliderValues;        // 记录每个Slider上一帧的值

    void Start()
    {
        // 初始化时间
        _lastIncreaseTime = Time.time;
    }

    void Update()
    {
        // 1. 找到所有 【激活状态】 且 【Tag=Slider】 的Slider
        _targetSliders = FindActiveSlidersWithTag();

        if (_targetSliders == null || _targetSliders.Length == 0)
            return;

        // 第一次运行时初始化记录数组
        if (_lastSliderValues == null || _lastSliderValues.Length != _targetSliders.Length)
        {
            _lastSliderValues = new float[_targetSliders.Length];
        }

        // 2. 检测每个滑块的 value 变化
        bool hasIncreased = CheckSliderValueIncrease();

        // 3. 如果有增长，刷新时间
        if (hasIncreased)
        {
            _lastIncreaseTime = Time.time;
        }

        // 4. 5秒没增长 → 重置回 1.0
        if (Time.time - _lastIncreaseTime >= resetTime)
        {
            magnification = 1.0f;
            _lastIncreaseTime = Time.time; // 避免连续重置
        }
    }

    /// <summary>
    /// 查找 激活状态、Tag=Slider 的所有 Slider
    /// </summary>
    Slider[] FindActiveSlidersWithTag()
    {
        // 找到场景中所有 Slider（包含失活）
        Slider[] allSliders = FindObjectsOfType<Slider>(includeInactive: true);
        System.Collections.Generic.List<Slider> activeList = new System.Collections.Generic.List<Slider>();

        foreach (Slider slider in allSliders)
        {
            // 只保留：激活 + Tag正确
            if (slider.gameObject.activeSelf && slider.CompareTag("Slider"))
            {
                activeList.Add(slider);
            }
        }

        return activeList.ToArray();
    }

    /// <summary>
    /// 检测所有Slider是否增长了0.1
    /// </summary>
    bool CheckSliderValueIncrease()
    {
        bool increased = false;

        for (int i = 0; i < _targetSliders.Length; i++)
        {
            Slider slider = _targetSliders[i];
            float oldValue = _lastSliderValues[i];
            float newValue = slider.value;

            // 计算档位：每 0.1 为一个档位
            int oldTier = Mathf.FloorToInt(oldValue / 0.1f);
            int newTier = Mathf.FloorToInt(newValue / 0.1f);

            // 每多一个档位 → 倍率 +0.1
            int tierDiff = newTier - oldTier;
            if (tierDiff > 0)
            {
                magnification += tierDiff * 0.1f;
                increased = true;
            }

            // 更新记录的值
            _lastSliderValues[i] = newValue;
        }

        return increased;
    }

    /// <summary>
    /// 外部获取当前倍率（你其他脚本直接调用这个）
    /// </summary>
    public float GetCurrentMagnification()
    {
        return magnification;
    }

    /// <summary>
    /// 手动重置倍率
    /// </summary>
    public void ResetMagnification()
    {
        magnification = 1.0f;
        _lastIncreaseTime = Time.time;
    }
}
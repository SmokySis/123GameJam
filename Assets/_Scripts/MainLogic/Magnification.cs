using UnityEngine;
using UnityEngine.UI;

public class Magnification : MonoBehaviour
{
    public float magnification = 1.0f;
    public float resetTime = 5f;

    private float _lastIncreaseTime;
    private Slider[] _currentSliders;
    private int[] _lastTiers; // 用档位记录，绝对精准
    public Text text1;
    public Text text2;

    void Start()
    {
        float i = 0;
        text2.text = i.ToString(); 

        _lastIncreaseTime = Time.time;
        magnification = 1.0f;
    }

    void Update()
    {
        FindActiveSliders();
        if (_currentSliders == null) return;

        bool increased = CheckTiers();

        if (increased)
            _lastIncreaseTime = Time.time;

        if (Time.time - _lastIncreaseTime > resetTime)
        {
            magnification = 1.0f;
            _lastIncreaseTime = Time.time;
        }
        magnification = Mathf.Round(magnification * 10f) / 10f;
        text1.text = (magnification.ToString() + "x");

    }

    void FindActiveSliders()
    {
        Slider[] all = FindObjectsOfType<Slider>(true);
        System.Collections.Generic.List<Slider> list = new();

        foreach (var s in all)
        {
            if (s.gameObject.activeSelf && s.CompareTag("Slider"))
                list.Add(s);
        }

        _currentSliders = list.ToArray();

        // 同步档位数组长度
        if (_lastTiers == null || _lastTiers.Length != _currentSliders.Length)
            _lastTiers = new int[_currentSliders.Length];
    }

    bool CheckTiers()
    {
        bool changed = false;

        for (int i = 0; i < _currentSliders.Length; i++)
        {
            float val = _currentSliders[i].value;
            int nowTier = Mathf.RoundToInt(val * 10); // 关键：乘10取整，绝对精准

            if (nowTier > _lastTiers[i])
            {
                int add = nowTier - _lastTiers[i];
                magnification += add * 0.1f;
                changed = true;
            }

            _lastTiers[i] = nowTier;
        }

        return changed;
    }

    public float GetMagnification()
    {
        //最终获取时，强制四舍五入到一位小数（保证是 2.0，不是 1.99999）
        return Mathf.Round(magnification * 10) / 10f;
    }

    public void ResetMag()
    {
        magnification = 1.0f;
        _lastIncreaseTime = Time.time;
    }
}
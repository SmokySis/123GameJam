using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
public class UIController : Singleton<UIController>
{
    [Header("UI组件_时间")]
    [SerializeField] TextMeshProUGUI timeShower;
    [Header("UI组件_电量")]
    [SerializeField] public float batteryPowerPercent = 1f;
    [SerializeField] Image batteryFillImage;
    [SerializeField] TextMeshProUGUI batteryText;
    [Header("UI组件_疲劳度")]
    [SerializeField] public float tiringPercent = 0f;
    [SerializeField] Image tiringFillImage; 
    [Header("Color")]
    [SerializeField] Color fullBatteryColor = Color.green;
    [SerializeField] Color midBatteryColor = Color.yellow;
    [SerializeField] Color lowBatteryColor = Color.red;
    [SerializeField] Color fullTiringColor = Color.red;
    [SerializeField] Color midTiringColor = Color.yellow;
    [SerializeField] Color lowTiringColor = Color.green;
    [Header("UI状态")]
    public bool isPaused = false;
    public bool isAudioOpen = false;
    [Header("设置面板")]
    public GameObject SettingPanel;
    public GameObject AudioPanel;

    void Start()
    {
        StartCoroutine(UpdateTime());
    }

    private void Update()
    {
        RefreshBattery();
        RefreshTiring();

        if (Input.GetKeyDown(KeyCode.Escape)) HandleESC();
    }

    IEnumerator UpdateTime()
    {
        WaitForSeconds time = new WaitForSeconds(1f);
        while (true) 
        {
            timeShower.text = System.DateTime.Now.ToString("HH:mm:ss");
            yield return time;
        }
    }

    void RefreshBattery()
    {
        batteryFillImage.fillAmount = batteryPowerPercent;
        batteryText.text = $"{(int)(batteryPowerPercent * 100)}%";
        if (batteryPowerPercent > 0.75f) { batteryFillImage.color = fullBatteryColor; return; }
        if (batteryPowerPercent > 0.25f) { batteryFillImage.color = midBatteryColor; return; }
        batteryFillImage.color = lowBatteryColor;
    }

    void RefreshTiring()
    {
        tiringFillImage.fillAmount = tiringPercent;
        if (tiringPercent > 0.75f) { tiringFillImage.color = fullTiringColor; return; }
        if (tiringPercent > 0.25f) { tiringFillImage.color = midTiringColor; return; }
        tiringFillImage.color = lowTiringColor;
    }

    public void SetPause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        SettingPanel.SetActive(true);
    }

    public void SetPlay()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SettingPanel.SetActive(false);
    }

    public void SetAudioActive()
    {
        SettingPanel.SetActive(false);
        AudioPanel.SetActive(true);
        isAudioOpen = true;
    }

    public void CloseAudioPanel()
    {
        AudioPanel.SetActive(false);
        SettingPanel.SetActive(true);
        isAudioOpen = false;
    }

    void HandleESC()
    {
        if (isPaused)
        {
            if (isAudioOpen) { CloseAudioPanel(); isAudioOpen = false; return; }
            else { SetPlay(); return; }
        }
        else
        {
            SetPause();
        }

    }
}

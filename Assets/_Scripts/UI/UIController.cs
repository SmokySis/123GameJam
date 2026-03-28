using AudioSystem;
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
    [Header("Audio接口")]
    AudioMixerRouter mixer;
    [Header("UI组件_音量")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] TextMeshProUGUI masterText;
    [SerializeField] TextMeshProUGUI bgmText;
    [SerializeField] TextMeshProUGUI sfxText;
    [Header("音量大小")]
    float masterVolume = 1;
    float bgmVolume = 1;
    float sfxVolume = 1;
    [Header("Color")]
    [SerializeField] Color fullBatteryColor = Color.green;
    [SerializeField] Color midBatteryColor = Color.yellow;
    [SerializeField] Color lowBatteryColor = Color.red;
    [SerializeField] Color fullTiringColor = Color.red;
    [SerializeField] Color midTiringColor = Color.yellow;
    [SerializeField] Color lowTiringColor = Color.green;
    [Header("UI状态")]
    [HideInInspector]public bool isPaused = false;
    [HideInInspector]public bool isAudioOpen = false;
    [Header("设置面板")]
    public GameObject SettingPanel;
    public GameObject AudioPanel;

    private void Awake()
    {
        base.Awake();
        mixer = AudioManager.Instance.Router;
    }
    void Start()
    {
        StartCoroutine(UpdateTime());
        masterSlider.onValueChanged.AddListener(OnMasterSliderValueChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);
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

    void OnMasterSliderValueChanged(float value)
    {
        masterVolume = value;
        masterText.text = ((int)(value * 100)).ToString();
        RefreshVolumn();
    }

    void OnBGMSliderValueChanged(float value)
    {
        bgmVolume = value;
        bgmText.text = ((int)(value * 100)).ToString();
        RefreshVolumn();    
    }

    void OnSFXSliderValueChanged(float value)
    {
        sfxVolume = value;
        sfxText.text = ((int)(value * 100)).ToString();
        RefreshVolumn();
    }

    void RefreshVolumn()
    {
        mixer.TrySetBusVolume(AudioBus.BGM,bgmVolume * masterVolume);
        mixer.TrySetBusVolume(AudioBus.SFX,sfxVolume * masterVolume);
    }

    void InitVolumn()
    {
        masterSlider.value = masterVolume;
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;
    }
}

using AudioSystem;
using PoolSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

enum TiringStates
{ 
    none_Tiring = 0,
    low_Tiring = 1,
    mid_Tiring = 2,
    high_Tiring = 3,
    full_Tiring = 4,
}


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
    [SerializeField] Window bilibili;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image tiringHead;
    [SerializeField] List<Sprite> tiringHeads;
    [SerializeField] GameObject tiringWarning;

    [Header("UI组件_音量")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] TextMeshProUGUI masterText;
    [SerializeField] TextMeshProUGUI bgmText;
    [SerializeField] TextMeshProUGUI sfxText;
    [Header("设置面板")]
    public GameObject SettingPanel;
    public GameObject AudioPanel;
    public GameObject MessagePanel;
    public GameObject DetailPanel;
    public GameObject DetailedTextPanel;
    [Header("Audio接口")]
    AudioMixerRouter mixer;
    [Header("Pool实例")]
    GameObjectPoolCenter poolCenter;
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
    [HideInInspector] public bool isPaused = false;
    [HideInInspector] public bool isAudioOpen = false;
    [Header("协程")]
    Coroutine MessageCoroutine;
    Coroutine DialogueCoroutine;
    [Header("消息弹窗_")]
    //[SerializeField] Text MessageText;
    [SerializeField] Vector2 UIPostion;
    [SerializeField] Vector2 InitPosition;
    [SerializeField] float slideInSpeed = 200;
    [SerializeField] float stayTime = 3;
    [SerializeField] float detailShowInterval = 1;
    [SerializeField] GameObject MessageButtonPrefab;
    [SerializeField] GameObject TextContainerPrefab;
    List<string> detailedMessages;
    List<TextContainer> messageContainers = new List<TextContainer>();
    GameObject CurrentMissionButton;
    [SerializeField] TiringStates currentTiringState;

    private void Awake()
    {
        base.Awake();
        poolCenter = GameObjectPoolCenter.Instance;
        mixer = AudioManager.Instance.Router;
    }
    void Start()
    {
        StartCoroutine(UpdateTime());
        masterSlider.onValueChanged.AddListener(OnMasterSliderValueChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);

        // StartCoroutine(MessagePanelCoroutine("Testing"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HandleESC();
        RefreshBattery();
        RefreshTiring();
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

    TiringStates RefreshTiring()
    {
        tiringFillImage.fillAmount = tiringPercent;
        if (tiringPercent > 1f) { StartCoroutine(TiringDownCo()); return TiringStates.full_Tiring; }
        if (tiringPercent > 0.9f) { tiringFillImage.color = fullTiringColor; tiringHead.sprite = tiringHeads[3]; return TiringStates.high_Tiring; }
        if (tiringPercent > 0.75f) { tiringFillImage.color = fullTiringColor; tiringHead.sprite = tiringHeads[2]; return TiringStates.mid_Tiring; }
        if (tiringPercent > 0.25f) { tiringFillImage.color = midTiringColor; tiringHead.sprite = tiringHeads[1]; return TiringStates.low_Tiring; }
        tiringHead.sprite = tiringHeads[0];
        if (tiringPercent < 0f )
            tiringPercent = 0;
        tiringFillImage.color = lowTiringColor;
        return TiringStates.none_Tiring;
    }

    void HandleTiringState(TiringStates settedState)
    {
        if (settedState != currentTiringState)
        {

        }
    }

    private IEnumerator TiringDownCo()
    {
        tiringWarning.gameObject.SetActive(true);
        bool isActive = bilibili.gameObject.activeSelf;
        Image image = bilibili.GetComponent<Image>();
        float num = image.color.a;
        image.color = new Color() { r = image.color.r, g = image.color.g, b = image.color.b, a = 1f };
        bilibili.gameObject.SetActive(true);
        bilibili.SetForeground();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        tiringHead.sprite = tiringHeads[3];
        while (tiringPercent > 0)
        {
            //tiringPercent -= 0.01f;
            yield return new WaitForSeconds(1);
        }
        tiringPercent = 0;
        tiringWarning.gameObject.SetActive(false);
        image.color = new Color() { r = image.color.r, g = image.color.g, b = image.color.b, a = num };
        bilibili.gameObject.SetActive(isActive);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
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
        mixer.TrySetBusVolume(AudioBus.BGM, bgmVolume * masterVolume);
        mixer.TrySetBusVolume(AudioBus.SFX, sfxVolume * masterVolume);
    }

    void InitVolumn()
    {
        masterSlider.value = masterVolume;
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;
    }

    IEnumerator MessagePanelCoroutine(string message)
    {
        RectTransform rect = MessagePanel.GetComponent<RectTransform>();
        //MessageText.text = message;
        rect.gameObject.SetActive(true);
        while (rect.anchoredPosition.x > UIPostion.x)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x - slideInSpeed * Time.deltaTime, rect.anchoredPosition.y);
            if (rect.anchoredPosition.x < UIPostion.x) rect.anchoredPosition = UIPostion;
            yield return null;
        }

        yield return new WaitForSeconds(stayTime);

        while (rect.anchoredPosition.x < InitPosition.x)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + slideInSpeed * Time.deltaTime, rect.anchoredPosition.y);
            if (rect.anchoredPosition.x > InitPosition.x) rect.anchoredPosition = InitPosition;
            yield return null;
        }
        ResetMessagePanel();
    }
    public void GetMessage(string message)
    {
        MessageCoroutine = StartCoroutine(MessagePanelCoroutine(message));
    }

    public void SetDetailedMessage(List<string> messages)
    {
        detailedMessages = messages;
        for (int i = 0; i < detailedMessages.Count; i++)
        {
            GameObject container = poolCenter.GetInstance(TextContainerPrefab, Vector3.zero, Quaternion.identity, DetailedTextPanel.transform, null, true);
            messageContainers.Add(container.GetComponent<TextContainer>());
            container.GetComponent<TextContainer>().dialogueText.text = detailedMessages[i];
            container.GetComponent<RectTransform>().localScale = Vector3.one;
            container.SetActive(false);
        }
    }

    IEnumerator ShowDialogueCoroutine()
    {
        Debug.Log("ShowDialoge");
        if (detailedMessages == null) yield break;

        for (int i = 0; i < detailedMessages.Count; i++)
        {
            messageContainers[i].gameObject.SetActive(true);
            messageContainers[i].SetSize();
            yield return new WaitForSeconds(detailShowInterval);
        }

        if (CurrentMissionButton != null) CurrentMissionButton.SetActive(true);
    }

    public void StartShowDialogue()
    {
        DialogueCoroutine = StartCoroutine(ShowDialogueCoroutine());
    }

    public void ReleaseDetailedPanel()
    {
        for (int i = detailedMessages.Count - 1; i >= 0; i--)
        {
            poolCenter.Release(messageContainers[i].gameObject);
        }
        messageContainers.Clear();
    }

    public void SetMissionButton(Action action)
    {
        GameObject buttonObject = poolCenter.GetInstance(MessageButtonPrefab, Vector3.zero, Quaternion.identity, DetailedTextPanel.transform, null, true);
        Button button = buttonObject.GetComponent<Button>() ? buttonObject.GetComponent<Button>() : buttonObject.AddComponent<Button>();
        button.GetComponent<RectTransform>().localScale = Vector3.one;
        button.onClick.AddListener(() => { action?.Invoke(); });
        CurrentMissionButton = buttonObject;
        CurrentMissionButton.SetActive(false);
    }

    public void ReleaseMissionButton()
    {
        CurrentMissionButton.GetComponent<Button>().onClick.RemoveAllListeners();
        poolCenter.Release(CurrentMissionButton);
    }

    public void StopMessageCoroutine()
    {
        if (MessageCoroutine != null) { StopCoroutine(MessageCoroutine); MessageCoroutine = null; }
        else return;
    }

    public void ResetMessagePanel() { MessagePanel.GetComponent<RectTransform>().anchoredPosition = InitPosition; MessagePanel.SetActive(false); }
}

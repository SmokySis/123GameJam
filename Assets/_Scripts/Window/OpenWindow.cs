using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenWindow : MonoBehaviour,IPointerClickHandler
{
    public Image MassagePanel;
    public GameObject panel;
    private Coroutine _panelCoroutine;

    private void Start()
    {
        MassagePanel = GetComponent<Image>();
        PlayMessagePanel(0);
    }

    public void PlayMessagePanel(int taskID)
    {
        if (_panelCoroutine != null)
            StopCoroutine(_panelCoroutine);

        _panelCoroutine = StartCoroutine(ShowMessagePanel(taskID));
    }

    private IEnumerator ShowMessagePanel(int taskID)
    {
        MassagePanel.gameObject.SetActive(true);
        RectTransform panelRect = MassagePanel.rectTransform;
        RectTransform canvasRect = MassagePanel.canvas.GetComponent<RectTransform>();

        // 先停止旧动画，避免重复调用时错乱
        panelRect.DOKill();

        float panelWidth = panelRect.rect.width;

        // 假设锚点在中心时：
        // 完全隐藏在右侧：panel中心点 = canvas宽度一半 + panel宽度一半
        float hiddenX = canvasRect.rect.width * 0.5f + panelWidth * 0.5f;

        // 像QQ弹窗一样先弹出来一点的位置
        float popX = hiddenX - panelWidth - 30f;

        // 先放到右侧外面
        Vector2 startPos = panelRect.anchoredPosition;
        panelRect.anchoredPosition = new Vector2(hiddenX + 50f, startPos.y);

        // 只弹出，不回到原位
        Tween tween = panelRect.DOAnchorPosX(popX, 0.45f).SetEase(Ease.OutBack);

        yield return tween.WaitForCompletion();

        _panelCoroutine = null;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        panel.SetActive(true);
        gameObject.SetActive(false);
    }
}
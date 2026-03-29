using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextContainer : MonoBehaviour
{
    public Text dialogueText;
    public Sprite dialogueBackground;
    public Sprite dialoguePicture;
    [SerializeField] Image BackgroundImage;
    [SerializeField] RectTransform TextRect;
    [SerializeField] RectTransform BackgroundRect;

    private void Start()
    {
        SetSize();
    }
    IEnumerator SetBackgroundSize()
    {
        yield return null;
        BackgroundRect.anchoredPosition = TextRect.anchoredPosition;
        BackgroundRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TextRect.rect.width + 20);
        BackgroundRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TextRect.rect.height + 15);
    }
    public void SetSize()
    {
        StartCoroutine(SetBackgroundSize());
    }
}

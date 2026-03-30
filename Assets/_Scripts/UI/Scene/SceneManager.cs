using UnityEngine.SceneManagement;
using UnityEngine;
using Utility;
using System.Collections;
using UnityEngine.UI;

public class LoadSceneManager : Singleton<LoadSceneManager>
{
    [SerializeField] string MainSceneName;
    [SerializeField] string MenuSceneName;
    [SerializeField] GameObject LoadPanel;
    [Header("时间参数")]
    [SerializeField] float fadeInTime = 3f;
    [SerializeField] float darkTime = 1f;
    public void LoadMainScene() => StartCoroutine(LoadMainCoroutine());
    public void LoadMenuScene() => StartCoroutine(LoadMenuCoroutine());

    IEnumerator LoadMainCoroutine()
    {

        Image image = LoadPanel.GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        LoadPanel.SetActive(true);
        while (image.color.a < 1)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + (1 / fadeInTime) * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(darkTime);
        SceneManager.LoadScene(MainSceneName);
    }

    IEnumerator LoadMenuCoroutine()
    {
        Image image = LoadPanel.GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        LoadPanel.SetActive(true);
        SceneManager.LoadScene(MenuSceneName);
        while (image.color.a > 0)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - (1 / fadeInTime) * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(darkTime);
        LoadPanel.SetActive(false);
    }
    protected override bool _isDonDestroyOnLoad => true;
    private void Awake()
    {
        base.Awake();
    }
}

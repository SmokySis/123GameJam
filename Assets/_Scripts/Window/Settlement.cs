using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Settlement : Utility.Singleton<Settlement>
{
    [Header("UI")]
    public GameObject EndPanel;
    public Text totalScore;
    public Text Comment;
    public GameObject EndButton;

    [Header("Fade")]
    public float panelFadeDuration = 0.8f;

    [Header("Typing")]
    public float titleTypeInterval = 0.08f;
    public float commentTypeInterval = 0.08f;

    [Header("Score")]
    public float scoreCountDuration = 0.4f;

    [Header("Rank Threshold")]
    public float excellentScore = 90f;
    public float normalScore = 60f;

    [Header("Rank Text")]
    public string excellentComment = "任务毁灭者";
    public string normalComment = "DDL 狂魔";
    public string badComment = "拖延症患者";

    private float _finalScore;
    private bool _isDone;
    [Button]
    public void End(float power, float score)
    {
        if (power <= 0 && !_isDone)
        {
            _finalScore = score;
            _isDone = true;
            StartCoroutine(EndScene());
        }
    }

    private IEnumerator EndScene()
    {
        EndPanel.SetActive(true);
        EndButton.SetActive(false);

        totalScore.text = "";
        Comment.text = "";

        Image panelImage = EndPanel.GetComponent<Image>();
        if (panelImage != null)
        {
            Color c = panelImage.color;
            c.a = 0f;
            panelImage.color = c;

            float timer = 0f;
            while (timer < panelFadeDuration)
            {
                timer += Time.deltaTime;
                float a = Mathf.Clamp01(timer / panelFadeDuration);

                c.a = a;
                panelImage.color = c;

                yield return null;
            }

            c.a = 1f;
            panelImage.color = c;
        }

        yield return StartCoroutine(TypeText(totalScore, "获得分数", titleTypeInterval));
        totalScore.text += "\n";

        yield return StartCoroutine(CountScore(totalScore, _finalScore, scoreCountDuration));

        yield return new WaitForSeconds(0.15f);

        yield return StartCoroutine(TypeText(Comment, "评价", commentTypeInterval));
        Comment.text += "\n";

        string rank = GetCommentByScore(_finalScore);
        yield return StartCoroutine(TypeText(Comment, Comment.text + rank, commentTypeInterval, true));

        EndButton.SetActive(true);
    }

    private IEnumerator TypeText(Text target, string content, float interval, bool overwrite = false)
    {
        if (overwrite)
            target.text = "";

        for (int i = 0; i < content.Length; i++)
        {
            target.text += content[i];
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator CountScore(Text target, float finalScore, float duration)
    {
        float timer = 0f;
        int scoreInt = Mathf.RoundToInt(finalScore);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            int current = Mathf.RoundToInt(Mathf.Lerp(0, scoreInt, t));

            target.text = "获得分数\n" + current;
            yield return null;
        }

        target.text = "获得分数\n" + scoreInt;
    }

    private string GetCommentByScore(float score)
    {
        if (score >= excellentScore)
            return excellentComment;

        if (score >= normalScore)
            return normalComment;

        return badComment;
    }
}
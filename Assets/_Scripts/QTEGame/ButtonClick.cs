using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonClick : MonoBehaviour
{
    public int diffculty;//传入的难度系数
    [Header("三种难度加的数值")]
    public float dif0 = 0f;
    public float dif1 = 0f;
    public float dif2 = 0f;

    private float speed = 0f;
    [Header("三种难度速度变化")]
    public float speed0 = 0.05f;
    public float speed1 = 0f;
    public float speed2 = 0f;
    public Animator anim;
    public Button button;
    public Image image;
    public Vector3 buttonScale;
    public Vector3 imageScale;
    private bool isEnd;//是否已经结束
    private bool isClicked;//是否已经被点击
    public ProgressBar g;
    private void Start()
    {       
        isEnd = false;
        isClicked = false;
        image.transform.localScale = Vector3.zero;//初始将判定部分置零
        buttonScale = button.transform.localScale;
        switch (diffculty)
        {
            case 0:
                speed = speed0;
                break;
            case 1:
                speed = speed1;
                break;
            case 2:
                speed = speed2;
                break;
        }
    }
    private void Update()
    {
        if (isEnd == false)
        {
            image.transform.localScale += Vector3.one * speed * Time.deltaTime;//判定部分随时间增大       
        }
        imageScale = image.transform.localScale;     
        if (image.transform.localScale.x - Vector3.one.x * 1.2f > 0)
        {
            NotClicked();
        }
    }
    /// <summary>
    /// 用于判定点击按钮精确度的方法
    /// </summary>
    public void Judgment()
    {
        isEnd = true;
        isClicked = true;
        Vector3 difference = buttonScale - imageScale;       
        if (-0.10f < difference.x  && difference.x < 0.10f)
        {
            Score(diffculty);
            print("Perfect");
            print(difference);
        }
        if ((-0.2f < difference.x && difference.x < -0.1f) || (0.1f < difference.x && difference.x < 0.2f))
        {
            Score(diffculty);
            print("Good");
            print(difference);
        }
        if (( difference.x < -0.2f) || (0.2f < difference.x))
        {
            Score(diffculty);
            print("Bad");
            print(difference);
        }
        anim.SetBool("IsEnd", true);
        
    }
    private void Score(int dif)
    {   
        switch (dif)
        {
            case 0:
                g.currentProgress += dif0;
                break;
            case 1:
                g.currentProgress += dif1;
                break;
            case 2:
                g.currentProgress += dif2;
                break;
            
        }

    }
    //动画之后摧毁物体
    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
    //超时没点
    public void NotClicked()
    {
        isEnd = true;
        print("Miss");
        anim.SetBool("IsEnd", true);
    }
}

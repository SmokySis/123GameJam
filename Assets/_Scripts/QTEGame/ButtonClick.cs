using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonClick : MonoBehaviour
{
    public Animator anim;
    public Button button;
    public Image image;
    public Vector3 buttonScale;
    public Vector3 imageScale;
    private bool isClicked;//是否已经被点击
    private void Start()
    {       
        isClicked = false;
        image.transform.localScale = Vector3.zero;//初始将判定部分置零
        buttonScale = button.transform.localScale;
    }
    private void Update()
    {
        if (isClicked == false)
        {
            image.transform.localScale += Vector3.one * 0.002f * Time.timeScale;//判定部分随时间增大       
        }
        imageScale = image.transform.localScale;        
    }
    /// <summary>
    /// 用于判定点击按钮精确度的方法
    /// </summary>
    public void Judgment()
    {
        isClicked = true;
        Vector3 difference = buttonScale - imageScale;       
        if (-0.10f < difference.x  && difference.x < 0.10f)
        {
            print("Perfect");
            print(difference);
        }
        if ((-0.2f < difference.x && difference.x < -0.1f) || (0.1f < difference.x && difference.x < 0.2f))
        {
            print("Good");
            print(difference);
        }
        if (( difference.x < -0.2f) || (0.2f < difference.x))
        {
            print("Bad");
            print(difference);
        }
        
        
    }
    //动画之后摧毁物体
    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
    
}

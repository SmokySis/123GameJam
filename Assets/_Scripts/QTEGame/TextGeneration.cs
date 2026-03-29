using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGeneration : MonoBehaviour
{
    public Text text;

    public int difficulty;
    private int dif;
    private int dif0 = 5;
    private int dif1 = 10;
    private int dif2 = 20;

    void Start()
    {
        switch (difficulty)
        {
            case 0:
                dif = dif0;
                break;
            case 1:
                dif = dif1;
                break;
            case 2:
                dif = dif2;
                break;
        }
        // 生成：长度随机（1~10）、字母随机的字符串
        string randomStr = GetRandomLetters(); 
        text.text = randomStr;
    }

    /// <summary>
    /// 从 A-Z 随机取若干个字母，返回字符串
    /// </summary>
    string GetRandomLetters()
    {
        // 26 个大写字母
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";       
        int length = dif;
        string result = "";

        // 循环取字母
        for (int i = 0; i < length; i++)
        {
            // 随机取一个字母下标
            int index = Random.Range(0, letters.Length);
            // 拼接字符串
            result += letters[index];
        }

        return result;
    }
}
    


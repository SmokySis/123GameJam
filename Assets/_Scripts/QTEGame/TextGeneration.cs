using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGeneration : MonoBehaviour
{
    public Text text;

    void Start()
    {
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
        int length = Random.Range(1, 11);
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
    


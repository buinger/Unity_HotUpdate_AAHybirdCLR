using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTool
{
    /// <summary>
    /// 防止文件有不明字符，故操作一遍
    /// </summary>
    /// <param name="jsonContent"></param>
    /// <returns></returns>
    public static string GetJsonStr(string json)
    {
        char startChar = default;
        char endChar = default;
        int startIndex = 0; ;

        for (int i = 0; i < json.Length; i++)
        {
            if (json[i].Equals('{') || json[i].Equals('['))
            {
                startIndex = i;
                break;
            }
        }

        json = json.Substring(startIndex, json.Length - 1);


        for (int i = 0; i < json.Length; i++)
        {
            if (json[i].Equals('{'))
            {
                startChar = '{';
                endChar = '}';
                break;
            }
            else if (json[i].Equals('['))
            {
                startChar = '[';
                endChar = ']';
                break;
            }
        }

        int lastIndex = GetLastIndex(json, startChar, endChar);

        return json.Substring(0, lastIndex + 1);

    }

    /// <summary>
    /// 获取Json结尾字符的下标
    /// </summary>
    /// <param name="s"></param>
    /// <param name="startchar"></param>
    /// <param name="endChar"></param>
    /// <returns></returns>
    public static int GetLastIndex(string s, char startchar, char endChar)
    {
        Stack<char> stack = new Stack<char>();
        int lastIndex = 0;

        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == startchar)
            {
                stack.Push(s[i]);
            }
            else
            {
                if (s[i].Equals(endChar))
                {
                    stack.Pop();
                    if (stack.Count == 0)
                    {
                        lastIndex = i;
                        break;
                    }
                }
            }
        }

        return lastIndex;
    }
}

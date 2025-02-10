using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTool
{

    public static bool GetRandomBool(int percentOf1 = 2)
    {
        int targetNum = percentOf1;
        if (targetNum != 2)
        {
            targetNum = Mathf.Clamp(targetNum, 2, int.MaxValue);
        }
        return UnityEngine.Random.Range(0, targetNum) == 0;
    }
    public static int GetRandomValue(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float GetRandomValue(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public static T GetRandomEnumValue<T>()
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
}

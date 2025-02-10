using UnityEngine;
using System;
using System.Globalization;
//using static UnityEditor.Progress;


class TimeTool
{
    public static double DiffSeconds(DateTime startTime, DateTime endTime)
    {
        TimeSpan secondSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return secondSpan.TotalSeconds;
    }
    public static double DiffMinutes(DateTime startTime, DateTime endTime)
    {
        TimeSpan minuteSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return minuteSpan.TotalMinutes;
    }
    public static double DiffHours(DateTime startTime, DateTime endTime)
    {
        TimeSpan hoursSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return hoursSpan.TotalHours;
    }
    public static double DiffDays(DateTime startTime, DateTime endTime)
    {
        TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return daysSpan.TotalDays;
    }
    public static int DiffDaysInt(DateTime startTime, DateTime endTime)
    {
        TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return daysSpan.Days;
    }

    public static string TimeStamp2Str(string timeStamp)
    {
        if (timeStamp != null && timeStamp != "")
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(1713436661996);
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            DateTime startTime = localTime;
            DateTime dt = startTime.AddMilliseconds(double.Parse(timeStamp));
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }
        else
        {
            Debug.LogError("时间戳为空");
            return "";
        }
    }

    /// <summary>
    /// ��ȡʱ���
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStamp()
    {

        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }


    //参数格式"2024-05-04"

    public static long GetTimeStamp(string dateStr)
    {
        DateTime date;
        bool isValidDate = DateTime.TryParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date);

        if (!isValidDate)
        {
            throw new ArgumentException("The provided date is not in the correct format.");
        }

        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan ts = date.ToUniversalTime() - epoch;
        return Convert.ToInt64(ts.TotalMilliseconds);
    }
}



[System.Serializable]
public class ColdTimeMachine
{

    public float cdTime = 10;
    private float passTime = 0;


    public bool PassingAuto()
    {
        passTime += Time.deltaTime;
        if (passTime >= cdTime)
        {
            Reset();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PassingManual()
    {
        passTime += Time.deltaTime;
        if (passTime >= cdTime)
        {

            return true;
        }
        else
        {
            return false;
        }
    }


    // public ColdTimeMachine(float _cdTime)
    // {
    //     cdTime = _cdTime;
    // }

    public void ChangeCdTime(float _cdTime)
    {
        cdTime = _cdTime;
    }

    public void Reset()
    {
        passTime = 0;

    }



}
[System.Serializable]
public class RandomTime
{
    public float nowTime = 2;
    public float minTime = 1;
    public float maxTime = 5;

    public string GetExcelValueStr()
    {
        return nowTime.ToString() + "*" + minTime.ToString() + "*" + maxTime.ToString();
    }

    public RandomTime(float _min, float _max)
    {
        minTime = _min;
        maxTime = _max;
        SetNewTime();
    }

    public void Ini()
    {
        if (nowTime == 0)
        {
            nowTime = 6;
        }
    }

    public void SetNewTime()
    {
        nowTime = UnityEngine.Random.Range(minTime, maxTime);
    }

}

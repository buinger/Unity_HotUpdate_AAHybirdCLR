using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoHideTip_Window : RunTimeWindow
{
    public float showTime = 2;
    public Text infoText;

    public void SetTextValue(string str, float time=2)
    {
        infoText.text = str;
        showTime = time;
        StartCoroutine(HideTip());
    }

    IEnumerator HideTip()
    {
        yield return new WaitForSeconds(showTime);
        CloseUi();
    }
}
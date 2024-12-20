using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmAndCancel_Window : Tip_Window
{
    public Button confirmButton;

    public void SetAllValue(ConfirmTipData data)
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            data.confirmEvent.Invoke();
            CloseUi();
        });
        SetTextValue(data.tipString);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
}


public class ConfirmTipData
{
    public string tipString;
    public Action confirmEvent;

    public ConfirmTipData(string tipStr, Action confirmEvent)
    {
        tipString = tipStr;
        this.confirmEvent = confirmEvent;

    }
}

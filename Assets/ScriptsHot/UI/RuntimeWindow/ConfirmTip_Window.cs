using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmTip_Window : Tip_Window
{
    public Button confirmButton;

    public void SetAllValue(IniData data)
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


    public class IniData
    {
        public string tipString;
        public Action confirmEvent;

        public IniData(string tipStr, Action confirmEvent)
        {
            this.tipString = tipStr;
            this.confirmEvent = confirmEvent;

        }
    }
}

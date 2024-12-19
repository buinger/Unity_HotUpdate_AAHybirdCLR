using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tip_Window : RunTimeWindow
{
    public Text infoText;
    public Button closeButton;
    protected override void OnEnable()
    {
        base.OnEnable();    
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => {
            CloseUi();
        });
    }
    public void SetTextValue(string str)
    {
        infoText.text = str;
    }
}

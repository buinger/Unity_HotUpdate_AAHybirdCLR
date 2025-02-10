using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSelection_Window : RunTimeWindow
{
    public Text titleText;
    public Button tempButton;
    Transform buttonContainer;

    protected override void Awake()
    {
        base.Awake();
        tempButton.gameObject.SetActive(false);
        buttonContainer = tempButton.transform.parent;
    }

    public void SetTitleAndButton(MultiSelectionData data)
    {
        foreach (Transform item in buttonContainer)
        {
            if (item != tempButton.transform)
            {
                Destroy(item.gameObject);
            }
        }

        titleText.text = data.title;
        foreach (var item in data.buttonDic)
        {
            Button button = Instantiate(tempButton, buttonContainer);
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<Text>().text = item.Key;
            button.onClick.AddListener(() =>
            {
                item.Value();
                CloseUi();
            });
        }
    }

}

public class MultiSelectionData
{

    public string title;
    public Dictionary<string, Action> buttonDic;

    public MultiSelectionData(string title, Dictionary<string, Action> buttonDic)
    {
        this.title = title;
        this.buttonDic = buttonDic;
    }
}
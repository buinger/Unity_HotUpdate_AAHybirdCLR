using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBookPageButton : MonoBehaviour
{
    public string pageName = "";
    public Transform pageTrans;
    // Start is called before the first frame update
    void Start()
    {
        if (pageTrans != null)
        {
            pageName = pageTrans.name;
        }
        Button button = transform.GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            if (pageName == "")
            {
                UiManager.NowBook?.ChangePageTo(1);
                return;
            }
            else
            {
                UiManager.NowBook?.ChangePageByPageName(pageName);
            }
        });

    }


}

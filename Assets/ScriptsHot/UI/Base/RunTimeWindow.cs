using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class RunTimeWindow : RuntimeUi
{
    private RectTransform rectTrans;
    int jishu=0;
    protected virtual void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    protected virtual void OnEnable()
    {
        jishu += 1;
        Debug.Log(rectTrans + "???" + jishu.ToString());
        rectTrans.transform.SetParent(UiManager.NowBook.transform, false);
        rectTrans.sizeDelta = new Vector2(Screen.width, Screen.height);
        rectTrans.transform.localPosition = Vector3.zero;
    }

   


}

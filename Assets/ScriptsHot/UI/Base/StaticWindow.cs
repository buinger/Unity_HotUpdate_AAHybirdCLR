using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticWindow : StaticUi
{
    void Awake()
    {
        //yield return new WaitUntil(() => UiManager.instance != null);
        UiManager.DoDelayFreamDo(() =>
        {
            UiManager.instance.StartCoroutine(Ini());
        });
        
    }


    protected abstract IEnumerator Ini();
}

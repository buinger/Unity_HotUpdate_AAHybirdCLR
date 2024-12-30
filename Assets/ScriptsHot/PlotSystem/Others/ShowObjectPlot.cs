using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowObjectPlot : Plot
{
    public GameObject[] targetObjs;
    public ChildPlotInformation plotBeforeClose;
    protected override void Ini(Action onIniOver)
    {
        SetObjsActive(false);
        onIniOver.Invoke();
    }

    protected override IEnumerator MainLogic()
    {
        SetObjsActive(true);
        yield return null;
    }

    protected override void ResetPlot()
    {
        SetObjsActive(false);
    }

    protected void SetObjsActive(bool active)
    {
        foreach (var item in targetObjs)
        {
            item.SetActive(active);
        }
    }
}



public abstract class ShowAndCloseObjPlot : ShowObjectPlot
{

    protected override IEnumerator MainLogic()
    {
        SetObjsActive(true);
        yield return StartCoroutine(LogicBeforeClose());
        yield return StartCoroutine(StartNewPlot(plotBeforeClose));
        SetObjsActive(false);
    }

    protected abstract IEnumerator LogicBeforeClose();
}

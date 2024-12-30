using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LinearPlot<PlotT> : Plot
{
    

    public List<PlotT> plotModel=new List<PlotT>();

   protected List<ChildPlotInformation> childPlotModels = new List<ChildPlotInformation>();


    protected override void Ini(Action onIniOver)
    {
        foreach (var item in plotModel)
        {
            ChildPlotInformation temp = new ChildPlotInformation(true, item as Plot);
            childPlotModels.Add(temp);
        }
        onIniOver.Invoke();
    }
    

    protected override IEnumerator MainLogic()
    {

        for (int i = 0; i < childPlotModels.Count; i++)
        {
            yield return StartNewPlot(childPlotModels[i]);
        }
    }

    protected override void ResetPlot()
    {

    }
}




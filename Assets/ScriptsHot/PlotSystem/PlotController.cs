 
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlotController : MonoBehaviour
{

    [Header("设置父Plot模型")]
    public List<Plot> allFatherPlotTemplate = new List<Plot>();

    public bool iniOnAwake = true;
    public bool autoPlay = false;

    [Header("以下参数仅供浏览------------------------------------------------------")]
    public string tip = "下面参数禁止编辑";
    public static PlotController instance;
    [Header("当前运行的父Plot")]
    public Plot playingPlot;
    [Tooltip("所有Plot")]
    [HideInInspector]
    List<Plot> allPlots = new List<Plot>();
    [Tooltip("所有父Plot")]
    public List<Plot> allFatherPlots = new List<Plot>();
    [Tooltip("所有子Plot")]
    [HideInInspector]
    List<Plot> allChildPlots = new List<Plot>();
    public List<Action> fatherPlotStartEvents = new List<Action>();
    bool iniFlag = false;

    public Action<int> onPlotPlay;

    public Action onLastPlotOver;

    private void Awake()
    {
     
        if (iniOnAwake)
        {
            Ini();
        }
    }


    public static void ReleaseStaticData()
    {
        instance = null;
        AnimationPlot.ReleaseSoleEntities();
    }

    public void Ini()
    {
        if (iniFlag == true) return;
        iniFlag = true;


        allFatherPlots.Clear();

        foreach (var item in allFatherPlotTemplate)
        {
            Plot entityPlot = Instantiate(item.gameObject).GetComponent<Plot>();
            allFatherPlots.Add(entityPlot);
        }

        instance = this;
        //获取父plot开始事件，并设定结束事件
        foreach (var item in allFatherPlots)
        {
            //提前注册父Plot
            Register(item);
            Action actionTemp = null;
            item.BindStartAction(ref actionTemp);
            fatherPlotStartEvents.Add(actionTemp);
            //注册结束自动下一个Plot事件
            item.onPlotOver += PlayNextPlot;
        }
 

        if (autoPlay)
        {
            //播放第一个Plot
            PlayFirstPlot();
        }



    }

    [ContextMenu("播放第一个Plot")]
    void PlayFirstPlot()
    {
        PlayPlotByIndex(0);
    }



    /// <summary>
    /// 每个Plot都会在这里注册，为了统一管理
    /// </summary>
    /// <param name="target"></param>
    public void Register(Plot target)
    {
        if (!allPlots.Contains(target))
        {
            allPlots.Add(target);
        }
        if (!allFatherPlots.Contains(target))
        {
            if (!allChildPlots.Contains(target))
            {
                allChildPlots.Add(target);
            }
        }
        else
        {
            target.ChangeToFather();
        }
    }


    public void RecycleChildPlot()
    {
        foreach (var item in allChildPlots)
        {
            if (item.gameObject.activeSelf == true)
            {
                GameObjectPoolTool.PutInPool(item.gameObject);
            }
        }
    }



    
    public void PlayPlotByIndex(int index, bool ifRestart = false)
    {
        if (ifRestart)
        {
            if (playingPlot != null)
            {
                playingPlot.StopPlot();
            }
        }
        fatherPlotStartEvents[index].Invoke();
        if (onPlotPlay != null)
        {
            onPlotPlay(index);
        }
    }
    public void StopAllPlot()
    {
        foreach (var item in allFatherPlots)
        {
            item.StopPlot();
            playingPlot = null;
        }

    }


    public void PlayPlotByName(string name, bool ifRestart = false)
    {
        int aimIndex = -1;
        for (int i = 0; i < allFatherPlots.Count; i++)
        {
            string nameTemp = allFatherPlots[i].gameObject.name;
            if (nameTemp == name)
            {
                int index = i;
                aimIndex = index;
                break;
            }
        }
        if (aimIndex != -1)
        {
            PlayPlotByIndex(aimIndex, ifRestart);
        }

    }


    public void PlayNextPlot()
    {
        if (playingPlot != null)
        {
            int nowIndex = allFatherPlots.IndexOf(playingPlot);
            if (nowIndex < allFatherPlots.Count - 1)
            {
                PlayPlotByIndex(nowIndex + 1);
            }
            else
            {
                //执行最终流程
                if (onLastPlotOver != null)
                {
                    onLastPlotOver.Invoke();
                }
            }
        }
        else
        {
            Debug.LogError("尚未运行任何父Plot，故向后不执行");
            //StartPlotByIndex(0);
        }
    }
    public void PlayPreviousPlot()
    {
        if (playingPlot != null)
        {
            int nowIndex = allFatherPlots.IndexOf(playingPlot);
            if (nowIndex > 0)
            {
                PlayPlotByIndex(nowIndex - 1);
            }
        }
        else
        {
            Debug.LogError("尚未运行任何父Plot，故向前不执行Plot");
            // StartPlotByIndex(allFatherPlots.Count-1);
        }
    }




}

[System.Serializable]
class plo
{
    public Button[] startButtons;
    public Plot fatherPlot;
}

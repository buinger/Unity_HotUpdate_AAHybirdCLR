using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public abstract class QiTransition : MonoBehaviour
{
    [Header("------------------通用设置---------------------")]
    [Header("设置动画刷新方式")]
    public UpdateType updateType = UpdateType.Normal;

    [Header("-------------------To---------------------")]
    //public string Intro_To_Parent = "-------------------To设置---------------------";
    [Header("激活时是否重新播放动画")]
    public bool to_ReplayOnEnable = false;
    [Header("是否在开始时就播放动画")]
    public bool to_PlayOnStart = true;
    [Header("播放完一次自动进入out动画，仅仅在单次播放有效")]
    public bool to_LinkOutTransition = false;
    [Header("循环模式（单次/从头循环/yoyo球）")]
    public TransitionRepeatType to_RepeatType = TransitionRepeatType.Once;
    [Header("进入消耗的时间")]
    public float to_OnecePassTime = 0.3f;
    [Header("设置动画初始延迟时间")]
    public float to_DelayTime = 0f;
    [Header("设置动画曲线")]
    public Ease to_Ease = Ease.Linear;
    [Header("To事件")]
    public QiTransitionEvent toEvents;


    [Header("-------------------Out---------------------")]
    // public string Intro_Out_Parent = "-------------------Out设置---------------------";
    [Header("离开消耗的时间（非custom无视）")]
    public float out_OnecePassTime = 0.3f;
    [Header("设置动画初始延迟时间")]
    public float out_DelayTime = 0f;
    [Header("设置动画曲线")]
    public Ease out_Ease = Ease.Linear;
    [Header("Out事件")]
    public QiTransitionEvent outEvents;


    public bool isPlay
    {
        get { return tweenerOut.IsPlaying() || tweenerTo.IsPlaying(); }

    }



    protected Tweener tweenerTo;
    protected Tweener tweenerOut;


    protected abstract void Ini();

    public void Awake() { Ini(); }
    protected abstract void SetStartData();
    protected abstract void SetEndData();
    protected abstract void SetOutPos();
    public abstract void ResetToStartData();

    private void OnDisable()
    {
        tweenerTo.Pause();
        tweenerOut.Pause();
    }

    bool playOnStartFlag = false;

    private void OnEnable()
    {
        if (to_ReplayOnEnable)
        {
            playOnStartFlag = true;
            ManualRePlayTo();
        }
        //else
        //{
        //    if (tweenerTo.IsPlaying() == false)
        //    {
        //        tweenerTo.Play();
        //    }
        //    else
        //    {
        //        to_PlayOnStart = true;
        //    }
        //}
    }


    private void Start()
    {
        if (to_PlayOnStart && playOnStartFlag == false)
        {
            playOnStartFlag = true;
            tweenerTo.Play();
            toEvents.onTransitionStart.Invoke();
        }
    }




    [ContextMenu("手动暂停所有动画")]
    public void ManualPause()
    {
        if (tweenerTo.IsPlaying() == true)
        {
            tweenerTo.Pause();

        }
        if (tweenerOut.IsPlaying() == true)
        {
            tweenerOut.Pause();

        }
    }
    [ContextMenu("手动继续播放to动画")]
    public void ManualContinuePlayTo()
    {
        if (tweenerTo.IsPlaying() == false)
        {
            if (tweenerOut.IsPlaying())
            {
                //ManualRePlay();
                Debug.Log("无法继续,out动作正在进行");
            }
            else
            {
                tweenerTo.Play();
            }

        }

    }

    [ContextMenu("手动重新to动画")]
    public void ManualRePlayTo()
    {
        tweenerOut.Pause();
        toEvents.onTransitionStart.Invoke();
        tweenerTo.Restart(true);
    }

    [ContextMenu("手动重新out动画")]
    public void ManualRePlayOut()
    {
        PlayOutTrans();
    }


    protected virtual void PlayOutTrans()
    {
        tweenerTo.Pause();
        outEvents.onTransitionStart.Invoke();
    }

}

[System.Serializable]
public class QiTransitionEvent
{
    public UnityEvent onTransitionStart;
    public UnityEvent onTransitionPlaying;
    public UnityEvent onTransitionEnd;
}


public enum TransitionType_Vector3
{
    TransX,
    TransY,
    TransZ,
    TransXY,
    TransXZ,
    TransYZ,
    TransAll,
}
public enum TransitionRepeatType
{
    Once,
    Loop,
    PingPang

}
public enum CoordinateType
{
    World,
    Self

}

public enum TransitionMoveOutType
{
    Back,
    Foward,
    Custom

}

public enum TransitionType_Color
{
    type,
    Image,
    RawImage

}


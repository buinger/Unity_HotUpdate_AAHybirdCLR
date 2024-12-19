using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererColorTransition : QiTransition
{

    [Header("-------------------To设置---------------------")]
    [Header("初始颜色")]
    public Color start_Color;
    private SpriteRenderer target;
    public string Intro_To_Self = "-------------------To设置---------------------";
    [Header("去的颜色")]
    public Color to_Color;
    [Header("移动轴设定，默认全轴")]
    public TransitionType_Vector3 to_TransitionAxis = TransitionType_Vector3.TransAll;

    [Header("-------------------Out设置---------------------")]
    public string Intro_Out_Self = "-------------------Out设置---------------------";
    [Header("离开的颜色")]
    public Color out_Color;
    private Color aimOut_Color;
    [Header("离开的模式，custom为自定义，back为反向移动(无视out数据)，foward为向前方移动(无视out数据)")]
    public TransitionMoveOutType transitionMoveOutType = TransitionMoveOutType.Back;
    [Header("移动轴设定，默认全轴（非custom无视）")]
    public TransitionType_Vector3 out_TransitionAxis = TransitionType_Vector3.TransAll;









    protected override void Ini()
    {
        target = transform.GetComponent<SpriteRenderer>();
        tweenerTo = CreateTweener(target, start_Color, to_Color, to_RepeatType, to_OnecePassTime, to_TransitionAxis, updateType, to_DelayTime, to_Ease, toEvents);
        switch (transitionMoveOutType)
        {
            case TransitionMoveOutType.Back:
                aimOut_Color = start_Color;
                tweenerOut = CreateTweener(target, to_Color, aimOut_Color, TransitionRepeatType.Once, to_OnecePassTime, to_TransitionAxis, updateType, out_DelayTime, out_Ease, outEvents);
                break;
            case TransitionMoveOutType.Foward:
                aimOut_Color = to_Color + (to_Color - start_Color);
                tweenerOut = CreateTweener(target, to_Color, aimOut_Color, TransitionRepeatType.Once, to_OnecePassTime, to_TransitionAxis, updateType, out_DelayTime, out_Ease, outEvents);
                break;
            case TransitionMoveOutType.Custom:
                aimOut_Color = out_Color;
                tweenerOut = CreateTweener(target, to_Color, aimOut_Color, TransitionRepeatType.Once, out_OnecePassTime, out_TransitionAxis, updateType, out_DelayTime, out_Ease, outEvents);
                break;
            default:
                break;
        }

        if (to_LinkOutTransition)
        {

            toEvents.onTransitionEnd.AddListener(new UnityAction(() =>
            {
                PlayOutTrans();
            }));

        }
        target.color = start_Color;
    }









    [ContextMenu("设置开始数据")]
    protected override void SetStartData()
    {
        start_Color = transform.GetComponent<SpriteRenderer>().color;
    }
    [ContextMenu("设置结束数据")]
    protected override void SetEndData()
    {
        to_Color = transform.GetComponent<SpriteRenderer>().color;
        ResetToStartData();
    }

    [ContextMenu("设置离开数据")]
    protected override void SetOutPos()
    {
        out_Color = transform.GetComponent<SpriteRenderer>().color;
        ResetToStartData();
    }

    //恢复开始状态
    public override void ResetToStartData()
    {
        transform.GetComponent<SpriteRenderer>().color = start_Color;
    }










    protected virtual Tweener OwnDo(SpriteRenderer target, Color endValue, Color fromValue, float _onecePassTime)
    {
        return target.DOColor(endValue, _onecePassTime).From(fromValue);
    }




    protected override void PlayOutTrans()
    {
        base.PlayOutTrans();
        tweenerOut.ChangeStartValue(target.color);
        tweenerOut.Restart();
    }



    //创建一个Tweener
    protected virtual Tweener CreateTweener(SpriteRenderer target, Color fromValue, Color endValue,
         TransitionRepeatType _repeatType, float _onecePassTime,
        TransitionType_Vector3 _transitionAxis, UpdateType _updateType, float _delayTime, Ease _ease, QiTransitionEvent qiEvent)
    {

        Tweener aimTweener;
        Color[] endAndFromValue = new Color[] { endValue, fromValue };

        aimTweener = OwnDo(target, endAndFromValue[0], endAndFromValue[1], _onecePassTime);

        switch (_repeatType)
        {
            case TransitionRepeatType.Once:

                aimTweener.SetLoops(1, LoopType.Restart);
                break;
            case TransitionRepeatType.Loop:
                aimTweener.SetLoops(-1, LoopType.Restart);
                break;
            case TransitionRepeatType.PingPang:
                aimTweener.SetLoops(-1, LoopType.Yoyo);
                break;
            default:
                break;
        }

        aimTweener.SetUpdate(_updateType);
        aimTweener.SetDelay(_delayTime);
        aimTweener.SetEase(_ease);
        aimTweener.SetAutoKill(false);



        aimTweener.OnStepComplete(() =>
        {
            if (_repeatType == TransitionRepeatType.Once)
            {
                Debug.Log("在终点");
                qiEvent.onTransitionEnd.Invoke();
            }

        });
        aimTweener.OnUpdate(() =>
        {
            qiEvent.onTransitionPlaying.Invoke();


        });

        aimTweener.Pause();


        return aimTweener;


    }




}

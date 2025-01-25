using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Vector3Transition : QiTransition
{
    [Header("------------------坐标设置---------------------")]
    [Header("初始数据")]
    public Vector3 start_Vector3;

    [Header("移动坐标标定位方式（世界/自身）")]
    public CoordinateType coordinateType = CoordinateType.Self;

    [Header("-------------------To---------------------")]
    // public string Intro_To_Self = "-------------------To设置---------------------";
    [Header("去的坐标")]
    public Vector3 to_Vector3;
    [Header("移动轴设定")]
    public TransitionType_Vector3 to_TransitionAxis = TransitionType_Vector3.TransAll;

    [Header("-------------------Out---------------------")]
    // public string Intro_Out_Self = "-------------------Out设置---------------------";
    [Header("离开的坐标")]
    public Vector3 out_Vector3;
    private Vector3 aimOut_Vector3;
    [Header("离开的模式，custom为自定义，back为反向移动(无视out数据)，foward为向前方移动(无视out数据)")]
    public TransitionMoveOutType transitionMoveOutType = TransitionMoveOutType.Back;
    [Header("移动轴设定（非custom无视）")]
    public TransitionType_Vector3 out_TransitionAxis = TransitionType_Vector3.TransAll;




    protected override void Ini()
    {
        tweenerTo = CreateTweener(start_Vector3, to_Vector3, coordinateType, to_RepeatType, to_OnecePassTime, to_TransitionAxis, updateType, to_DelayTime, to_Ease, toEvents);

        switch (transitionMoveOutType)
        {
            case TransitionMoveOutType.Back:
                aimOut_Vector3 = start_Vector3;
                tweenerOut = CreateTweener(to_Vector3, aimOut_Vector3, coordinateType, TransitionRepeatType.Once, to_OnecePassTime, to_TransitionAxis, updateType, out_DelayTime, out_Ease, outEvents);
                break;
            case TransitionMoveOutType.Foward:
                aimOut_Vector3 = to_Vector3 + (to_Vector3 - start_Vector3);
                tweenerOut = CreateTweener(to_Vector3, aimOut_Vector3, coordinateType, TransitionRepeatType.Once, to_OnecePassTime, to_TransitionAxis, updateType, out_DelayTime, out_Ease, outEvents);
                break;
            case TransitionMoveOutType.Custom:
                aimOut_Vector3 = out_Vector3;
                tweenerOut = CreateTweener(to_Vector3, aimOut_Vector3, coordinateType, TransitionRepeatType.Once, out_OnecePassTime, out_TransitionAxis, updateType, out_DelayTime, out_Ease, outEvents);
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
    }







    protected virtual Tweener OwnDo(Vector3 endValue, Vector3 fromValue, CoordinateType _coordinateType, float _onecePassTime)
    {
        Tweener t;
        if (_coordinateType == CoordinateType.Self)
        {
            t = transform.DOLocalMove(endValue, _onecePassTime).From(fromValue).Pause();
            transform.localPosition = start_Vector3;
        }
        else
        {
            t = transform.DOMove(endValue, _onecePassTime).From(fromValue).Pause();
            transform.position = start_Vector3;
        }
       // t.Pause();
        return t;
    }



    protected override void PlayOutTrans()
    {
        base.PlayOutTrans();
        Vector3[] aimValues;
        if (coordinateType == CoordinateType.Self)
        {
            aimValues = GetVector3Value(transform.localPosition, out_Vector3, out_TransitionAxis);
        }
        else
        {
            aimValues = GetVector3Value(transform.position, out_Vector3, out_TransitionAxis);
        }
        tweenerOut.ChangeStartValue(aimValues[0], out_OnecePassTime);
        tweenerOut.Restart(true);
    }



    //创建一个Tweener
    protected virtual Tweener CreateTweener(Vector3 fromValue, Vector3 endValue,
        CoordinateType _coordinateType, TransitionRepeatType _repeatType, float _onecePassTime,
        TransitionType_Vector3 _transitionAxis, UpdateType _updateType, float _delayTime, Ease _ease, QiTransitionEvent qiEvent)
    {

        Tweener aimTweener;
        Vector3[] endAndFromValue = GetVector3Value(endValue, fromValue, _transitionAxis);

        aimTweener = OwnDo(endAndFromValue[0], endAndFromValue[1], _coordinateType, _onecePassTime);

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

                qiEvent.onTransitionEnd.Invoke();
            }

        });
        aimTweener.OnUpdate(() =>
        {
            qiEvent.onTransitionPlaying.Invoke();


        });

        // aimTweener.Pause();


        return aimTweener;


    }


    protected Vector3[] GetVector3Value(Vector3 endValue, Vector3 fromValue, TransitionType_Vector3 type)
    {

        switch (type)
        {
            case TransitionType_Vector3.TransX:
                return new Vector3[] { new Vector3(endValue.x, fromValue.y, fromValue.z), fromValue };
            case TransitionType_Vector3.TransY:
                return new Vector3[] { new Vector3(fromValue.x, endValue.y, fromValue.z), fromValue };
            case TransitionType_Vector3.TransZ:
                return new Vector3[] { new Vector3(fromValue.x, fromValue.y, endValue.z), fromValue };
            case TransitionType_Vector3.TransXY:
                return new Vector3[] { new Vector3(endValue.x, endValue.y, fromValue.z), fromValue };
            case TransitionType_Vector3.TransXZ:
                return new Vector3[] { new Vector3(endValue.x, fromValue.y, endValue.z), fromValue };
            case TransitionType_Vector3.TransYZ:
                return new Vector3[] { new Vector3(fromValue.x, endValue.y, endValue.z), fromValue };
            case TransitionType_Vector3.TransAll:
                return new Vector3[] { endValue, fromValue };
            default:
                return new Vector3[] { endValue, fromValue };
        }

    }
}

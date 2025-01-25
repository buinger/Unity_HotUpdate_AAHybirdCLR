using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTransition : Vector3Transition
{
    [ContextMenu("设置开始数据")]
    protected override void SetStartData()
    {
        Debug.Log(transform.localScale);
        start_Vector3 = transform.localScale;
    }
    [ContextMenu("设置结束数据")]
    protected override void SetEndData()
    {
        to_Vector3 = transform.localScale;

        ResetToStartData();
    }

    [ContextMenu("设置离开数据")]
    protected override void SetOutPos()
    {
        out_Vector3 = transform.localScale;

        ResetToStartData();
    }

    //恢复开始状态
    public override void ResetToStartData()
    {
        transform.localScale = start_Vector3;
    }


    protected override Tweener OwnDo(Vector3 endValue, Vector3 fromValue, CoordinateType _coordinateType, float _onecePassTime)
    {
        return transform.DOScale(endValue, _onecePassTime).From(fromValue);


    }



}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MoveTransition : Vector3Transition
{


    [ContextMenu("设置开始数据")]
    protected override void SetStartData()
    {
        if (coordinateType == CoordinateType.World)
        {
            start_Vector3 = transform.position;
        }
        else
        {
            start_Vector3 = transform.localPosition;
        }
    }
    [ContextMenu("设置去的数据")]
    protected override void SetEndData()
    {
        if (coordinateType == CoordinateType.World)
        {
            to_Vector3 = transform.position;
        }
        else
        {
            to_Vector3 = transform.localPosition;
        }

        ResetToStartData();
    }

    [ContextMenu("设置离开数据")]
    protected override void SetOutPos()
    {
        if (coordinateType == CoordinateType.World)
        {
            out_Vector3 = transform.position;
        }
        else
        {
            out_Vector3 = transform.localPosition;
        }
        ResetToStartData();
    }

    //恢复开始状态
    public override void ResetToStartData()
    {
        if (coordinateType == CoordinateType.Self)
            transform.localPosition = start_Vector3;
        else
            transform.position = start_Vector3;
    }

    private void OnDrawGizmos()
    {
        Vector3 to_Vec3_Gizmos = to_Vector3;
        Vector3 start_Vec3_Gizmos = start_Vector3;
        switch (to_TransitionAxis)
        {
            case TransitionType_Vector3.TransX:
                to_Vector3.y = start_Vec3_Gizmos.y;
                to_Vector3.z = start_Vec3_Gizmos.z;
                break;

            case TransitionType_Vector3.TransY:
                to_Vector3.x = start_Vec3_Gizmos.x;
                to_Vector3.z = start_Vec3_Gizmos.z;
                break;
            case TransitionType_Vector3.TransZ:
                to_Vector3.x = start_Vec3_Gizmos.x;
                to_Vector3.y = start_Vec3_Gizmos.y;
                break;

            case TransitionType_Vector3.TransXY:
                to_Vector3.z = start_Vec3_Gizmos.z;
                break;
            case TransitionType_Vector3.TransXZ:
                to_Vector3.y = start_Vec3_Gizmos.y;
                break;
            case TransitionType_Vector3.TransYZ:
                to_Vector3.x = start_Vec3_Gizmos.x;
                break;
            case TransitionType_Vector3.TransAll:
                break;
            default:
                break;

        }
        if (coordinateType == CoordinateType.Self && transform.parent != null)
        {

            start_Vec3_Gizmos = transform.parent.TransformPoint(start_Vec3_Gizmos);
            to_Vec3_Gizmos = transform.parent.TransformPoint(to_Vec3_Gizmos);

        }

        Gizmos.color = Color.cyan;
        //Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.5f);
        Gizmos.DrawWireCube(start_Vec3_Gizmos, Vector3.one);
        Gizmos.DrawWireCube(to_Vec3_Gizmos, Vector3.one);
        Gizmos.DrawLine(start_Vec3_Gizmos, to_Vec3_Gizmos);
    }


}

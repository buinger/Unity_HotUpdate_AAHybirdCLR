using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RotateTransition : Vector3Transition
{
    [ContextMenu("设置开始数据")]
    protected override void SetStartData()
    {
        if (coordinateType == CoordinateType.World)
        {
            start_Vector3 = transform.eulerAngles;
        }
        else
        {
            start_Vector3 = GetInspectorRotationValueMethod(transform);
        }
    }
    [ContextMenu("设置结束数据")]
    protected override void SetEndData()
    {
        if (coordinateType == CoordinateType.World)
        {
            to_Vector3 = transform.eulerAngles;
        }
        else
        {
            to_Vector3 = GetInspectorRotationValueMethod(transform);
        }

        ResetToStartData();
    }

    [ContextMenu("设置离开数据")]
    protected override void SetOutPos()
    {
        if (coordinateType == CoordinateType.World)
        {
            out_Vector3 = transform.eulerAngles;
        }
        else
        {
            out_Vector3 = GetInspectorRotationValueMethod(transform);
        }
        ResetToStartData();
    }

    //恢复开始状态
    public override void ResetToStartData()
    {
        if (coordinateType == CoordinateType.Self)
            transform.localEulerAngles = start_Vector3;
        else
            transform.eulerAngles = start_Vector3;
    }



    protected override Tweener OwnDo(Vector3 endValue, Vector3 fromValue, CoordinateType _coordinateType, float _onecePassTime)
    {
        // if (_coordinateType == CoordinateType.Self)
        // {
        //     // 将角度值限制在 0 到 360 度之间
        //     endValue = new Vector3(endValue.x % 360, endValue.y % 360, endValue.z % 360);
        //     fromValue = new Vector3(fromValue.x % 360, fromValue.y % 360, fromValue.z % 360);

        //     // 设置初始旋转
        //     transform.localRotation = Quaternion.Euler(fromValue);

        //     // 执行四元数插值旋转
        //     return transform.DOLocalRotateQuaternion(Quaternion.Euler(endValue), _onecePassTime).Pause();
        // }
        // else
        // {
        //     // 将世界欧拉角转换为本地欧拉角
        //     Quaternion localEndValue = Quaternion.Euler(transform.InverseTransformDirection(endValue));
        //     Quaternion localFromValue = Quaternion.Euler(transform.InverseTransformDirection(fromValue));

        //     // 设置初始旋转
        //     transform.rotation = Quaternion.Euler(fromValue);

        //     // 执行四元数插值旋转
        //     return transform.DORotateQuaternion(Quaternion.Euler(endValue), _onecePassTime).Pause();
        // }
        if (_coordinateType == CoordinateType.Self)
        {
            return transform.DOLocalRotate(endValue, _onecePassTime,RotateMode.FastBeyond360).From(fromValue).Pause();
        }
        else
        {
            return transform.DORotate(endValue, _onecePassTime, RotateMode.FastBeyond360).From(fromValue).Pause();
        }
    }


    protected override void PlayOutTrans()
    {
        //base.PlayOutTrans();
        Vector3[] aimValues;
        if (coordinateType == CoordinateType.Self)
        {
            aimValues = GetVector3Value(transform.localEulerAngles, out_Vector3, out_TransitionAxis);

        }
        else
        {
            aimValues = GetVector3Value(transform.eulerAngles, out_Vector3, out_TransitionAxis);
        }
        tweenerOut.ChangeStartValue(Quaternion.Euler(aimValues[0]), out_OnecePassTime);

        tweenerOut.Restart(true);


    }



    //获取到旋转的正确数值
    public static Vector3 GetInspectorRotationValueMethod(Transform transform)
    {
        // 获取原生值
        System.Type transformType = transform.GetType();
        PropertyInfo m_propertyInfo_rotationOrder = transformType.GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
        object m_OldRotationOrder = m_propertyInfo_rotationOrder.GetValue(transform, null);
        MethodInfo m_methodInfo_GetLocalEulerAngles = transformType.GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
        object value = m_methodInfo_GetLocalEulerAngles.Invoke(transform, new object[] { m_OldRotationOrder });
        string temp = value.ToString();
        //将字符串第一个和最后一个去掉
        temp = temp.Remove(0, 1);
        temp = temp.Remove(temp.Length - 1, 1);
        //用‘，’号分割
        string[] tempVector3;
        tempVector3 = temp.Split(',');
        //将分割好的数据传给Vector3
        Vector3 vector3 = new Vector3(float.Parse(tempVector3[0]), float.Parse(tempVector3[1]), float.Parse(tempVector3[2]));
        return vector3;
    }

}

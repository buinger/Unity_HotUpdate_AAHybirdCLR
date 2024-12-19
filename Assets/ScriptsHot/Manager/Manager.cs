using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager<T> : MonoBehaviour
{
    public static T instance;
    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            Ini();
            instance = (T)(object)this;
        }
    }

    protected abstract void Ini();

}

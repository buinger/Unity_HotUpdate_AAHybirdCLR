using System.Collections;
using UnityEngine;

public abstract class ControllerBase : MonoBehaviour
{
    public abstract IEnumerator Ini();
}

public abstract class Controller<T> : ControllerBase
{
    public static T instance;
    public bool isIniOver = false;


    void Start()
    {
        StartCoroutine(Ini());
    }
    protected abstract IEnumerator IniRoutine();
    public override IEnumerator Ini()
    {
        yield return IniRoutine();
        instance = (T)(object)this;
        isIniOver = true;
    }

    public abstract void ControllerUpdate();

    void Update()
    {
        if (isIniOver)
        {
            ControllerUpdate();
        }
    }


}

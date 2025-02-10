using System.Collections;
using UnityEngine;

public abstract class ModuleBase : MonoBehaviour
{
    protected bool couldRun = false;
    public virtual IEnumerator Ini()
    {
        gameObject.SetActive(couldRun);
        yield return null;
    }
    public bool CouldRun
    {
        set
        {
            if (couldRun != value)
            {
                if (value == false)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }
            couldRun = value;
        }
        //get => couldRun;
    }
}

public abstract class Module<T> : ModuleBase
{
    public static T instance;


    public bool isIniOver = false;


    protected abstract IEnumerator IniRoutine();
    public override IEnumerator Ini()
    {
        gameObject.SetActive(couldRun);
        yield return IniRoutine();
        instance = (T)(object)this;
        isIniOver = true;
    }

    protected virtual void ModuleUpdate() { }
    protected virtual void ModuleLateUpdate() { }

    void Update()
    {
        if (isIniOver && couldRun)
        {
            ModuleUpdate();
        }
    }

    void LateUpdate()
    {
        if (isIniOver && couldRun)
        {
            ModuleLateUpdate();
        }
    }


}

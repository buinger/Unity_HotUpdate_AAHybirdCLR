using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;

public class ModuleController : Controller<ModuleController>
{
    public Transform gameCameraTrans;

    public static Camera gameCamera;
    public Transform modelFather;
    public ModuleBase[] allModules;

    public PrefabInfo aiSpeakWindowTemp;

   

    // IEnumerator ResetModelSet()
    // {
    //     yield return new WaitUntil(() => nowIpetModel.animator.isInitialized);
    //     onModelChange?.Invoke(nowIpetModel);
    // }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        gameCamera = gameCameraTrans.GetComponent<Camera>();
    }

    protected override IEnumerator IniRoutine()
    {
        yield return new WaitUntil(() => DataManager.instance.iniOver == true);
        Transform modelTrans = modelFather.GetChild(0);
  
        allModules = transform.GetComponentsInChildren<ModuleBase>();
        //初始化所有模块
        instance = this;
        foreach (var module in allModules)
        {
            module.CouldRun = true;
            yield return StartCoroutine(module.Ini());
        }

        GlobalKeyListener.Start();
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        GlobalKeyListener.Stop();
    }

    public override void ControllerUpdate()
    {

    }

}



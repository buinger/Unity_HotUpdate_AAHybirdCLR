using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Manager<GameManager>
{
    protected override void Ini()
    {
        DontDestroyOnLoad(gameObject);
        //切换场景前释放对象池
        SceneManager.sceneUnloaded += (scene) =>
        {
            GameObjectPoolTool.Release();
        };
        GlobalWindowController.Ini();
        GlobalKeyListener.RegisterHotKey_Single(VirtualKey.VK_F2, () =>
        {
            GlobalWindowController.RestoreAndBringToFront();
        });

#if !UNITY_EDITOR
        GlobalWindowController.SetTopMost();

        float biLi = 360f / 1920f;
        float heightBiLi = 1920f / 1080f;
        int screenWidth = Display.main.systemWidth;
        int targetWidth = (int)(screenWidth * biLi);
        int targetHeight = (int)(targetWidth * heightBiLi);
        Screen.SetResolution(targetWidth, targetHeight, false);
        
        StartCoroutine(SetRightBottom());
#endif

    }

    IEnumerator SetRightBottom()
    {
        yield return new WaitForEndOfFrame();
        GlobalWindowController.SetWindowToBottomRight();
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDestroy()
    {
#if !UNITY_EDITOR
        GlobalWindowController.RemoveTopMost();
#endif
        GlobalWindowController.hWnd = IntPtr.Zero;
    }

}







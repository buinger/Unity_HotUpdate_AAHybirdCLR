using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class HotUpdateStarter : MonoBehaviour
{
    public string mainScenePath; // 场景的 Addressable 地址
    // Start is called before the first frame update
    void Start()
    {
        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
        Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes(GetTargetDllPath()));
#else
        // Editor下无需加载，直接查找获得HotUpdate程序集
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif
        //跳转场景
        GetTargetDllPath();
         Addressables.LoadSceneAsync(mainScenePath, LoadSceneMode.Single).Completed += OnSceneLoaded;
    }


    // 场景加载完成后的回调
    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"场景 {mainScenePath} 加载成功！");
        }
        else
        {
            Debug.LogError($"场景 {mainScenePath} 加载失败！");
        }
    }


    string GetTargetDllPath()
    {
        //C:/Users/Administrator/AppData/LocalLow/DefaultCompany/HotUpdate
        //C:\Users\Administrator\AppData\LocalLow\DefaultCompany\HotUpdate\HotUpdateData\StandaloneWindows64
        string dllTargetPath = Application.persistentDataPath;
        dllTargetPath = dllTargetPath + "/HotUpdateData/" + GetHotDataFolderName() + "/HotUpdate.dll";
        Debug.Log(dllTargetPath);
        return dllTargetPath;
        //C:/Users/Administrator/AppData/LocalLow/DefaultCompany/HotUpdate/HotUpdateData/StandaloneWindows64/HotUpdate.dll
    }



    /// <summary>
    /// 获取运行时的构建目标名称。
    /// </summary>
    /// <returns>类似 EditorUserBuildSettings.activeBuildTarget.ToString() 的字符串名称。</returns>
    public static string GetHotDataFolderName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                return "StandaloneWindows64";
            case RuntimePlatform.WindowsEditor:
                return "StandaloneWindowsEditor";
            case RuntimePlatform.OSXPlayer:
                return "StandaloneOSX";
            case RuntimePlatform.OSXEditor:
                return "StandaloneOSXEditor";
            case RuntimePlatform.LinuxPlayer:
                return "StandaloneLinux64";
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WebGLPlayer:
                return "WebGL";
            case RuntimePlatform.PS4:
                return "PS4";
            case RuntimePlatform.XboxOne:
                return "XboxOne";
            case RuntimePlatform.Switch:
                return "Switch";
            case RuntimePlatform.tvOS:
                return "tvOS";
            case RuntimePlatform.Stadia:
                return "Stadia";
            case RuntimePlatform.CloudRendering:
                return "LinuxHeadlessSimulation";
            default:
                return "UnknownTarget";
        }
    }

}

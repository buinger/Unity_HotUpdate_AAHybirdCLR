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
    public string mainScenePath; // ������ Addressable ��ַ
    // Start is called before the first frame update
    void Start()
    {
        // Editor�����£�HotUpdate.dll.bytes�Ѿ����Զ����أ�����Ҫ���أ��ظ����ط���������⡣
#if !UNITY_EDITOR
        Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes(GetTargetDllPath()));
#else
        // Editor��������أ�ֱ�Ӳ��һ��HotUpdate����
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif
        //��ת����
        GetTargetDllPath();
         Addressables.LoadSceneAsync(mainScenePath, LoadSceneMode.Single).Completed += OnSceneLoaded;
    }


    // ����������ɺ�Ļص�
    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"���� {mainScenePath} ���سɹ���");
        }
        else
        {
            Debug.LogError($"���� {mainScenePath} ����ʧ�ܣ�");
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
    /// ��ȡ����ʱ�Ĺ���Ŀ�����ơ�
    /// </summary>
    /// <returns>���� EditorUserBuildSettings.activeBuildTarget.ToString() ���ַ������ơ�</returns>
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

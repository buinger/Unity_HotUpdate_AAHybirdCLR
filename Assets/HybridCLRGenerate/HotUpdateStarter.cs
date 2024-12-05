using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes"));
#else
        // Editor下无需加载，直接查找获得HotUpdate程序集
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif
        //跳转场景
        // Addressables.LoadSceneAsync(mainScenePath, LoadSceneMode.Single).Completed += OnSceneLoaded;
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

}

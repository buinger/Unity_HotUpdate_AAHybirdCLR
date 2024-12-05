using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class HotUpdateTool
{

    [MenuItem("资源操作/更新热更相关所有", priority = 100)]
    public static void UpdateAll()
    {
        CompileDllActiveBuildTarget();
        UpdateHotUpdateDll();
        Debug.Log("所有热更资源更新完毕");
    }

    [MenuItem("资源操作/更新热更Dll", priority = 102)]
    public static void UpdateHotUpdateDll()
    {
        CompileDllCommand.CompileDllActiveBuildTarget();
        Debug.Log("热更dll更新完成");
    }

    [MenuItem("资源操作/更新热更AA资源包", priority = 101)]
    public static bool CompileDllActiveBuildTarget()
    {
        AddressablesPlayerBuildResult result = null;
        AddressableAssetSettings.BuildPlayerContent(out result);

        if (result != null && !string.IsNullOrEmpty(result.Error))
        {
            Debug.LogError($"Failed to build Addressables content, content not included in Player Build. \"{result.Error}\"");
            return false;
        }
        else
        {
            Debug.Log("AA资源更新完成");
            return true;
        }
    }


}

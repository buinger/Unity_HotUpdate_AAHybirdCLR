using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class HotUpdateTool
{

    [MenuItem("��Դ����/�����ȸ��������", priority = 100)]
    public static void UpdateAll()
    {
        CompileDllActiveBuildTarget();
        UpdateHotUpdateDll();
        Debug.Log("�����ȸ���Դ�������");
    }

    [MenuItem("��Դ����/�����ȸ�Dll", priority = 102)]
    public static void UpdateHotUpdateDll()
    {
        CompileDllCommand.CompileDllActiveBuildTarget();
        Debug.Log("�ȸ�dll�������");
    }

    [MenuItem("��Դ����/�����ȸ�AA��Դ��", priority = 101)]
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
            Debug.Log("AA��Դ�������");
            return true;
        }
    }


}

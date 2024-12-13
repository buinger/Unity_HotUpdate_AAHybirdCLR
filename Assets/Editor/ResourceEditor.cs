using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using System.Text;
using UnityEditor.AddressableAssets.Settings;

using UnityEditor.AddressableAssets;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class ResourceEditor : MonoBehaviour
{
#if UNITY_EDITOR

    [MenuItem("资源操作/一键刷新热更预制件索引脚本")]
    public static void SetAllAllAAPrefabName()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });

        // 获取 AddressableAssetSettings，确保 Addressables 系统已经初始化
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        List<string> allPath= new List<string>();
        // 遍历所有组
        foreach (var group in settings.groups)
        {
            // 遍历该组中的所有资源条目（AssetEntry）
            foreach (var entry in group.entries)
            {
                // 打印资源的路径
                string assetPath = entry.AssetPath;
                allPath.Add(assetPath);
            }
        }

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(path);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                bool isAA = IsAssetAddressable(allPath, path);
                PrefabInfo temp = prefab.GetComponent<PrefabInfo>();
                if (temp == null)
                {
                    if (isAA == true)
                    {
                        temp = prefab.AddComponent<PrefabInfo>();
                    }
                    else
                    {
                        EditorUtility.SetDirty(prefab);
                        AssetDatabase.SaveAssets();
                        continue;
                    }
                }
                else
                {
                    if (isAA == false)
                    {
                        DestroyImmediate(temp, true);
                        EditorUtility.SetDirty(prefab);
                        AssetDatabase.SaveAssets();
                        continue;
                    }
                }

                bool ifChange = false;

                string[] strs = path.Split('/');
                string name = strs[strs.Length - 1];
                name = name.Replace(".prefab", "");
                if (name != temp.shortName)
                {
                    temp.shortName = name;
                    ifChange = true;
                }

                string aimPath = path.Replace(Application.persistentDataPath + "/", "");

                if (temp.path != aimPath)
                {
                    temp.path = aimPath;
                    ifChange = true;
                }

                if (ifChange)
                {
                    EditorUtility.SetDirty(prefab);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("预制件信息设置完毕");



        // 检查资源是否为 Addressable
        bool IsAssetAddressable(List<string> allPath, string assetPath)
        {
            return allPath.Contains(assetPath);
        }

    }

    [MenuItem("资源操作/将选中脚本转化成utf8格式 %&U")]
    public static void ConvertScriptsToUtf8()
    {
        object[] allTargetObj = Selection.objects;//这个函数可以得到你选中的对象
        foreach (Object targetObj in allTargetObj)
        {
            string path = AssetDatabase.GetAssetPath(targetObj);
            if (Path.GetFileName(path).Contains(".cs"))
            {
                ConvertFileToUtf8(path);
                Debug.Log(path + ":脚本已转换为无签名的UTF-8格式。");
            }

        }

    }

    private static void ConvertFileToUtf8(string filePath)
    {
        // 读取文件内容
        string content = File.ReadAllText(filePath, Encoding.GetEncoding("GB2312"));

        // 将内容写入为无签名的UTF-8格式
        File.WriteAllText(filePath, content, new UTF8Encoding(false));
    }

#endif

}

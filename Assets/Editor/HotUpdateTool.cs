using HybridCLR.Editor.Commands;
using System.IO;
using System;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Principal;

public class HotUpdateTool
{

    [MenuItem("资源操作/更新所有", priority = 100)]
    public static async void UpdateAll()
    {
        await UpdateHotUpdateAAbundle();
        UpdateHotUpdateDll();
        Debug.Log("所有热更资源更新完毕");
    }

    [MenuItem("资源操作/更新热更代码", priority = 102)]
    public static void UpdateHotUpdateDll()
    {
        CompileDllCommand.CompileDllActiveBuildTarget();
        string dllFilePath = GetOringnalDllPath();
        string targetDllFilePath = GetTargetDllPath();
        bool copyOk = CopyFile(dllFilePath, targetDllFilePath);
        GenerateFileHash(targetDllFilePath, "HotUpdate.hash");
        Debug.Log("热更dll更新完成");

        string GetOringnalDllPath()
        {
            //D:/UnityProjects/Unity_HotUpdate_AAHybirdCLR/Assets
            //D:\UnityProjects\Unity_HotUpdate_AAHybirdCLR\HybridCLRData\HotUpdateDlls\StandaloneWindows64\HotUpdateDll.dll
            string dllPath = Application.dataPath;
            dllPath = dllPath.Replace("Assets", "");
            dllPath = dllPath + "HybridCLRData/HotUpdateDlls/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/HotUpdate.dll";
            Debug.Log(dllPath);

            return dllPath;

            //D:/UnityProjects/Unity_HotUpdate_AAHybirdCLR/HybridCLRData/HotUpdateDlls/StandaloneWindows64/HotUpdateDll.dll
        }


        string GetTargetDllPath()
        {
            //C:/Users/Administrator/AppData/LocalLow/DefaultCompany/HotUpdate
            //C:\Users\Administrator\AppData\LocalLow\DefaultCompany\HotUpdate\HotUpdateData\StandaloneWindows64
            string dllTargetPath = Application.persistentDataPath;
            dllTargetPath = dllTargetPath + "/HotUpdateData/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/HotUpdate.dll";
            Debug.Log(dllTargetPath);
            return dllTargetPath;
            //C:/Users/Administrator/AppData/LocalLow/DefaultCompany/HotUpdate/HotUpdateData/StandaloneWindows64/HotUpdateDll.dll
        }

    }

    [MenuItem("资源操作/更新热更资源", priority = 101)]
    public static async Task UpdateHotUpdateAAbundle()
    {
        AddressablesPlayerBuildResult result = null;
        AddressableAssetSettings.BuildPlayerContent(out result);

        if (result != null && !string.IsNullOrEmpty(result.Error))
        {
            Debug.LogError($"Failed to build Addressables content, content not included in Player Build. \"{result.Error}\"");
        }
        else
        {
            List<string> usefulBundleNames = await GetAllBundleFileNamesByCatalogJson(GetCatalogPath());

            string folderPath = GetNowPlatformHotUpdateFolderPath();
            //Debug.Log(folderPath);
            string[] filePath = Directory.GetFiles(folderPath);

            foreach (var item in filePath)
            {
                if (item.Contains(".bundle"))
                {
                    string fileName = Path.GetFileName(item);
                    if (!usefulBundleNames.Contains(fileName))
                    {
                        File.Delete(item);
                    }
                }
            }

            Debug.Log("AA资源更新完成");

        }



        string GetCatalogPath()
        {
            string dllTargetPath = Application.persistentDataPath;
            dllTargetPath = dllTargetPath + "/HotUpdateData/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/catalog_0.1.json";
            return dllTargetPath;
        }
    }

    private static string GetNowPlatformHotUpdateFolderPath()
    {
        string hotUpdateFolderPath = Application.persistentDataPath;
        hotUpdateFolderPath = hotUpdateFolderPath + "/HotUpdateData/" + EditorUserBuildSettings.activeBuildTarget.ToString();
        return hotUpdateFolderPath;
    }


    public static async Task<List<string>> GetAllBundleFileNamesByCatalogJson(string filePath)
    {
        try
        {
            List<string> fileNames = new List<string>();
            // 读取 JSON 文件
            var json = await File.ReadAllTextAsync(filePath);

            // 使用 System.Text.Json 反序列化 JSON
            var catalogData = JsonConvert.DeserializeObject<AAcatalogData>(json);

            foreach (string item in catalogData.m_InternalIds)
            {
                if (item.Contains(".bundle"))
                {
                    string targetName = Path.GetFileName(item);
                    fileNames.Add(targetName);

                }
            }


            return fileNames;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading JSON: {ex.Message}");
            return null;
        }
    }


    /// <summary>
    /// 复制文件的函数。
    /// </summary>
    /// <param name="sourceFilePath">源文件的路径。</param>
    /// <param name="destinationFilePath">目标文件的路径。</param>
    /// <param name="overwrite">如果目标文件已存在，是否覆盖。</param>
    /// <returns>文件复制成功返回 true，否则返回 false。</returns>
    public static bool CopyFile(string sourceFilePath, string destinationFilePath, bool overwrite = true)
    {
        try
        {
            // 检查源文件是否存在
            if (!File.Exists(sourceFilePath))
            {
                Debug.LogError($"源文件不存在：{sourceFilePath}");
                return false;
            }

            // 检查并创建目标文件夹
            string destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            if (!Directory.Exists(destinationDirectory))
            {
                Debug.Log($"目标文件夹不存在，正在创建：{destinationDirectory}");
                Directory.CreateDirectory(destinationDirectory);
            }

            // 执行文件复制
            File.Copy(sourceFilePath, destinationFilePath, overwrite);
            Debug.Log($"文件复制成功：从 {sourceFilePath} 到 {destinationFilePath}");
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.LogError($"访问被拒绝：{ex.Message}");
        }
        catch (IOException ex)
        {
            Debug.LogError($"文件操作错误：{ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"发生错误：{ex.Message}");
        }

        return false;
    }


    /// <summary>
    /// 获取文件的哈希值并生成 HotUpdateDll.hash 文件。
    /// </summary>
    /// <param name="filePath">要计算哈希值的文件路径。</param>
    /// <returns>操作成功返回 true，否则返回 false。</returns>
    public static bool GenerateFileHash(string filePath, string txtName)
    {
        try
        {
            // 检查文件是否存在
            if (!File.Exists(filePath))
            {
                Debug.LogError($"文件不存在：{filePath}");
                return false;
            }

            // 计算文件的哈希值
            string hash = GetFileHash(filePath);

            // 定义输出文件路径
            string hashFilePath = Path.Combine(Path.GetDirectoryName(filePath), txtName);

            // 将哈希值写入文件
            File.WriteAllText(hashFilePath, hash);

            Debug.Log($"文件哈希值生成成功：{hashFilePath}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"发生错误：{ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 计算文件的哈希值（SHA256）。
    /// </summary>
    /// <param name="filePath">文件路径。</param>
    /// <returns>文件的哈希值。</returns>
    private static string GetFileHash(string filePath)
    {
        using (var sha256 = SHA256.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }



}

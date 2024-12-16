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
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

public class HotUpdateTool
{
    private static string serverUrlHead = "http://127.0.0.1:637/";

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
        ResourceEditor.SetAllAllAAPrefabName();
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

            if (usefulBundleNames != null)
            {
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


    [MenuItem("资源操作/上传所有到服务器", priority = 103)]
    public async static void UpLoadAllToServer()
    {
        string[] filePath = Directory.GetFiles(GetNowPlatformHotUpdateFolderPath());

        for (int i = 0; i < filePath.Length; i++)
        {
            if (i == 0)
            {
                await UploadFileAsync(filePath[i], true);
            }
            else
            {
                await UploadFileAsync(filePath[i]);
            }
        }

        Debug.Log($"{EditorUserBuildSettings.activeBuildTarget.ToString()}平台所有热更资源上传完毕");

        async Task UploadFileAsync(string filePath, bool clearFolder = false)
        {
            // 检查文件是否存在
            if (!System.IO.File.Exists(filePath))
            {
                Debug.LogError("文件不存在: " + filePath);
                return;
            }

            // 读取文件内容
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);

            string clearFolderTail = clearFolder ? "&clearFolder=true" : "";
            // 创建上传请求
            UnityWebRequest request = new UnityWebRequest(GetUpLoadUrl() + clearFolderTail, UnityWebRequest.kHttpVerbPOST);

            // 生成请求边界
            string boundary = "----UnityFormBoundary";
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            // 构造表单数据
            byte[] fileNameBytes = Encoding.UTF8.GetBytes($"Content-Disposition: form-data; name=\"file\"; filename=\"{Path.GetFileName(filePath)}\"\r\n");
            byte[] fileHeaderBytes = Encoding.UTF8.GetBytes("Content-Type: application/octet-stream\r\n\r\n");

            // 构造完整的 multipart 数据
            List<byte> formData = new List<byte>();
            formData.AddRange(boundaryBytes);
            formData.AddRange(fileNameBytes);
            formData.AddRange(fileHeaderBytes);
            formData.AddRange(fileData); // 添加文件数据
            formData.AddRange(Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n")); // 结束边界

            // 设置上传处理器
            request.uploadHandler = new UploadHandlerRaw(formData.ToArray());
            request.downloadHandler = new DownloadHandlerBuffer();

            // 设置 Content-Type 为 multipart/form-data，并添加文件字段
            request.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + boundary);

            // 发送请求并等待响应
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield(); // 异步等待完成
            }

            // 检查结果
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("文件上传成功: " + filePath);
            }
            else
            {
                Debug.LogError("文件上传失败: " + filePath + "-------------" + request.error);
            }
        }


    }







    private static string GetUpLoadUrl()
    {
        //private static string urlHead = "http://localhost:8080/upload/testFolder?password=123456";
        string url = serverUrlHead + "upload/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "?password=123456";
        return url;
    }

    private static string GetDownUrl()
    {
        //private static string urlHead = "http://localhost:8080/upload/testFolder?password=123456";
        string url = serverUrlHead + "download/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "?password=123456";
        return url;
    }


    private static string GetNowPlatformHotUpdateFolderPath()
    {
        // 获取目标目录路径
        string hotUpdateFolderPath = Application.persistentDataPath;
        hotUpdateFolderPath = hotUpdateFolderPath + "/HotUpdateData/" + EditorUserBuildSettings.activeBuildTarget.ToString();

        // 如果目录不存在，则创建该目录
        if (!Directory.Exists(hotUpdateFolderPath))
        {
            Directory.CreateDirectory(hotUpdateFolderPath);
        }

        // 返回文件夹路径
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
            Debug.LogError($"Error reading JSON: {ex.Message}");
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

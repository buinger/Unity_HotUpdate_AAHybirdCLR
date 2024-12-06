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

    [MenuItem("��Դ����/��������", priority = 100)]
    public static async void UpdateAll()
    {
        await UpdateHotUpdateAAbundle();
        UpdateHotUpdateDll();
        Debug.Log("�����ȸ���Դ�������");
    }

    [MenuItem("��Դ����/�����ȸ�����", priority = 102)]
    public static void UpdateHotUpdateDll()
    {
        CompileDllCommand.CompileDllActiveBuildTarget();
        string dllFilePath = GetOringnalDllPath();
        string targetDllFilePath = GetTargetDllPath();
        bool copyOk = CopyFile(dllFilePath, targetDllFilePath);
        GenerateFileHash(targetDllFilePath, "HotUpdate.hash");
        Debug.Log("�ȸ�dll�������");

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

    [MenuItem("��Դ����/�����ȸ���Դ", priority = 101)]
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

            Debug.Log("AA��Դ�������");

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
            // ��ȡ JSON �ļ�
            var json = await File.ReadAllTextAsync(filePath);

            // ʹ�� System.Text.Json �����л� JSON
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
    /// �����ļ��ĺ�����
    /// </summary>
    /// <param name="sourceFilePath">Դ�ļ���·����</param>
    /// <param name="destinationFilePath">Ŀ���ļ���·����</param>
    /// <param name="overwrite">���Ŀ���ļ��Ѵ��ڣ��Ƿ񸲸ǡ�</param>
    /// <returns>�ļ����Ƴɹ����� true�����򷵻� false��</returns>
    public static bool CopyFile(string sourceFilePath, string destinationFilePath, bool overwrite = true)
    {
        try
        {
            // ���Դ�ļ��Ƿ����
            if (!File.Exists(sourceFilePath))
            {
                Debug.LogError($"Դ�ļ������ڣ�{sourceFilePath}");
                return false;
            }

            // ��鲢����Ŀ���ļ���
            string destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            if (!Directory.Exists(destinationDirectory))
            {
                Debug.Log($"Ŀ���ļ��в����ڣ����ڴ�����{destinationDirectory}");
                Directory.CreateDirectory(destinationDirectory);
            }

            // ִ���ļ�����
            File.Copy(sourceFilePath, destinationFilePath, overwrite);
            Debug.Log($"�ļ����Ƴɹ����� {sourceFilePath} �� {destinationFilePath}");
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.LogError($"���ʱ��ܾ���{ex.Message}");
        }
        catch (IOException ex)
        {
            Debug.LogError($"�ļ���������{ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"��������{ex.Message}");
        }

        return false;
    }


    /// <summary>
    /// ��ȡ�ļ��Ĺ�ϣֵ������ HotUpdateDll.hash �ļ���
    /// </summary>
    /// <param name="filePath">Ҫ�����ϣֵ���ļ�·����</param>
    /// <returns>�����ɹ����� true�����򷵻� false��</returns>
    public static bool GenerateFileHash(string filePath, string txtName)
    {
        try
        {
            // ����ļ��Ƿ����
            if (!File.Exists(filePath))
            {
                Debug.LogError($"�ļ������ڣ�{filePath}");
                return false;
            }

            // �����ļ��Ĺ�ϣֵ
            string hash = GetFileHash(filePath);

            // ��������ļ�·��
            string hashFilePath = Path.Combine(Path.GetDirectoryName(filePath), txtName);

            // ����ϣֵд���ļ�
            File.WriteAllText(hashFilePath, hash);

            Debug.Log($"�ļ���ϣֵ���ɳɹ���{hashFilePath}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"��������{ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// �����ļ��Ĺ�ϣֵ��SHA256����
    /// </summary>
    /// <param name="filePath">�ļ�·����</param>
    /// <returns>�ļ��Ĺ�ϣֵ��</returns>
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

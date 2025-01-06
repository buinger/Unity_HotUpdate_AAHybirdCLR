using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HotUpdateStarter : MonoBehaviour
{
    public bool ifCheckUpdate = false;

    public string urlHead = "http://127.0.0.1:637";

    public string mainScenePath; // 场景的 Addressable 地址
    public Text loadingText;
    bool isUpdating = false;
    float nowUpdatePercent = 0;
    float targetUpdatepercent = 0;
    //{UnityEngine.AddressableAssets.Addressables.RuntimePath}/HotUpdateData/[BuildTarget]
    private string HotUpdateDataPath
    {
        get
        {
            return Path.Combine(Addressables.RuntimePath, "BuildTarget");
        }
    }

    public string HotUpdateDownLoadUrlHead
    {
        get
        {
            return urlHead + "/download/" + GetHotDataFolderName() + "/";
        }
    }


    // Start is called before the first frame update
    IEnumerator Start()
    {
        Debug.Log($"catalog_{Application.version}.hash");
        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
        if (ifCheckUpdate)
        {
            yield return CheckHotUpdate();
        }
#else

        // Editor下无需加载，直接查找获得HotUpdate程序集
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
        //跳转场景
        Addressables.LoadSceneAsync(mainScenePath, LoadSceneMode.Single).Completed += OnSceneLoaded;
        yield return null;
#endif

    }


    private void Update()
    {
        if (isUpdating)
        {
            loadingText.gameObject.SetActive(true);
            nowUpdatePercent = Mathf.Lerp(nowUpdatePercent, targetUpdatepercent, 0.2f);
            loadingText.text = "Ipet更新中:" + nowUpdatePercent.ToString("0") + "%";
        }
        else
        {
            loadingText.gameObject.SetActive(false);
        }
    }

    // IEnumerator ReloadCatalog()
    // {
    //     // 如果已经有一个加载操作在进行，先清理它
    //     if (loadCatalogHandle.IsValid())
    //     {
    //         Addressables.Release(loadCatalogHandle);
    //     }
    //     // 卸载之前加载的资源
    //     Addressables.ClearResourceLocators();
    //     string cataLogPath = Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.json");
    //     Addressables.LoadContentCatalogAsync(Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.json"), false).Completed += (handle) =>
    //     {
    //         Debug.Log("读取加载catalog完成");
    //     };
    //     // 异步加载新的 catalog
    //     loadCatalogHandle = Addressables.LoadContentCatalogAsync(cataLogPath);
    //     yield return loadCatalogHandle;
    //     if (loadCatalogHandle.Status == AsyncOperationStatus.Succeeded)
    //     {
    //         Debug.Log("新资aa源加载完毕");
    //     }
    //     else
    //     {
    //         Debug.LogError("aa资源加载失败");
    //     }
    // }

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
        string dllTargetPath = Application.persistentDataPath;
        dllTargetPath = Path.Combine(HotUpdateDataPath, "HotUpdate.dll");
        Debug.Log(dllTargetPath);
        return dllTargetPath;
        //C:/Users/Administrator/AppData/LocalLow/DefaultCompany/HotUpdate/HotUpdateData/StandaloneWindows64/HotUpdate.dll
    }



    IEnumerator CheckHotUpdate()
    {
        //如果热更文件夹不存在或者为空，又或者文件夹内有正在热更标志文件（说明热更曾中断），则走完整热更流程
        if (IsFolderEmpty(HotUpdateDataPath) || File.Exists(Path.Combine(HotUpdateDataPath, "Update.flag")))
        {
            yield return StartCoroutine(FullUpdateRoutine());
            yield break;
        }

        //走下去，说明热更数据完整，但有可能是旧的热更数据 
        bool isDllOld = false;
        bool isBundleOld = false;

        //确认dll是否需要更新
        string nowDllHash = ReadLocalTxt("HotUpdate.hash");
        string serverDllHash = "";
        yield return StartCoroutine(ReadNetTxt(HotUpdateDownLoadUrlHead + "HotUpdate.hash", (hash) =>
         {
             serverDllHash = hash;
         }));

        if (nowDllHash != serverDllHash)
        {
            isDllOld = true;
        }
        //确认boundle是否需要更新
        string nowBoundleHash = ReadLocalTxt($"catalog_{Application.version}.hash");
        string serverBoundleHash = "";
        yield return StartCoroutine(ReadNetTxt(HotUpdateDownLoadUrlHead + $"catalog_{Application.version}.hash", (hash) => { serverBoundleHash = hash; }));


        if (nowBoundleHash != serverBoundleHash)
        {
            isBundleOld = true;
        }
        //如果dll和bundle都是最新的，则不需要热更
        if (isDllOld == false && isBundleOld == false)
        {
            Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes(GetTargetDllPath()));
            //跳转场景
            Addressables.LoadSceneAsync(mainScenePath, LoadSceneMode.Single).Completed += OnSceneLoaded;
            yield break;
        }
        else//否者进入选择性更新
        {
            yield return StartCoroutine(SelectUpdateRoutine(isDllOld, isBundleOld));
            yield break;
        }




        IEnumerator FullUpdateRoutine()
        {
            isUpdating = true;
            DeleteAllFilesAndFolders(HotUpdateDataPath);
            CreateUpdateFlagFile(HotUpdateDataPath);

            //完整热更流程
            yield return DownloadFile(HotUpdateDownLoadUrlHead + "HotUpdate.dll", Path.Combine(HotUpdateDataPath, "HotUpdate.dll"));
            targetUpdatepercent = 20;
            yield return DownloadFile(HotUpdateDownLoadUrlHead + "HotUpdate.hash", Path.Combine(HotUpdateDataPath, "HotUpdate.hash"));
            targetUpdatepercent = 30;
            yield return DownloadFile(HotUpdateDownLoadUrlHead + $"catalog_{Application.version}.json", Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.json"));
            targetUpdatepercent = 75;
            List<string> boundleNames = GetAllBundleFileNamesByCatalogJson();
            foreach (var item in boundleNames)
            {
                yield return DownloadFile(HotUpdateDownLoadUrlHead + item, Path.Combine(HotUpdateDataPath, item));
            }
            targetUpdatepercent = 90;
            yield return DownloadFile(HotUpdateDownLoadUrlHead + $"catalog_{Application.version}.hash", Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.hash"));
            targetUpdatepercent = 99;
            //fixCatalogJson();
            File.Delete(Path.Combine(HotUpdateDataPath, "Update.flag"));

            targetUpdatepercent = 100;
            yield return new WaitForSeconds(0.5f);
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }


        IEnumerator SelectUpdateRoutine(bool dllUpdate, bool boundleUpdate)
        {
            isUpdating = true;
            CreateUpdateFlagFile(HotUpdateDataPath);
            if (dllUpdate)
            {
                yield return DownloadFile(HotUpdateDownLoadUrlHead + "HotUpdate.dll", Path.Combine(HotUpdateDataPath, "HotUpdate.dll"));
                targetUpdatepercent = 20;
                yield return DownloadFile(HotUpdateDownLoadUrlHead + "HotUpdate.hash", Path.Combine(HotUpdateDataPath, "HotUpdate.hash"));
                targetUpdatepercent = 25;
            }
            if (boundleUpdate)
            {
                yield return DownloadFile(HotUpdateDownLoadUrlHead + $"catalog_{Application.version}.json", Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.json"));
                targetUpdatepercent = 30;
                List<string> boundleNames = GetAllBundleFileNamesByCatalogJson();

                //Debug.Log(folderPath);
                string[] filePath = Directory.GetFiles(HotUpdateDataPath);
                List<string> existsBoundleName = new List<string>();
                foreach (var item in filePath)
                {
                    if (item.Contains(".bundle"))
                    {
                        string fileName = Path.GetFileName(item);
                        if (boundleNames.Contains(fileName) == false)
                        {
                            File.Delete(item);
                        }
                        else
                        {
                            existsBoundleName.Add(fileName);
                        }
                    }
                }
                targetUpdatepercent = 45;

                foreach (var item in boundleNames)
                {
                    if (!existsBoundleName.Contains(item))
                    {
                        existsBoundleName.Add(item);
                        //下载不存在的boundle,不重复下载同hash的boundle
                        yield return DownloadFile(HotUpdateDownLoadUrlHead + item, Path.Combine(HotUpdateDataPath, item));
                    }
                }
                targetUpdatepercent = 80;

                yield return DownloadFile(HotUpdateDownLoadUrlHead + $"catalog_{Application.version}.hash", Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.hash"));
                targetUpdatepercent = 85;
                //fixCatalogJson();

                targetUpdatepercent = 88;
            }
            File.Delete(Path.Combine(HotUpdateDataPath, "Update.flag"));
            targetUpdatepercent = 100;
            yield return new WaitForSeconds(0.5f);
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }



    }

    // [ContextMenu("修复catalog.json")]
    // void fixCatalogJson()
    // {
    //     string jsonStr = ReadLocalTxt($"catalog_{Application.version}.json");
    //     Debug.Log(jsonStr);

    //     AAcatalogData catalogData = JsonConvert.DeserializeObject<AAcatalogData>(jsonStr);

    //     List<string> newInternalIds = new List<string>();
    //     foreach (string item in catalogData.m_InternalIds)
    //     {

    //         if (item.Contains(".bundle"))
    //         {
    //             newInternalIds.Add(Path.Combine(HotUpdateDataPath, Path.GetFileName(item)));
    //         }
    //         else
    //         {
    //             newInternalIds.Add(item);
    //         }
    //     }
    //     catalogData.m_InternalIds = newInternalIds;
    //     string newJsonStr = JsonConvert.SerializeObject(catalogData);
    //     File.WriteAllText(Path.Combine(HotUpdateDataPath, $"catalog_{Application.version}.json"), newJsonStr);
    // }
    List<string> GetAllBundleFileNamesByCatalogJson()
    {
        List<string> fileNames = new List<string>();
        string jsonStr = ReadLocalTxt($"catalog_{Application.version}.json");
        Debug.Log(jsonStr);

        AAcatalogData catalogData = JsonConvert.DeserializeObject<AAcatalogData>(jsonStr);

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

    string ReadLocalTxt(string fileName)
    {
        string filePath = Path.Combine(HotUpdateDataPath, fileName);
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("更新失败，本地文件不存在: " + filePath);
            DeleteAllFilesAndFolders(HotUpdateDataPath);
            return string.Empty;
        }
    }

    bool IsFolderEmpty(string path)
    {
        // 检查目录是否存在
        if (!Directory.Exists(path))
        {
            //如果没有热更目录，则创建
            Directory.CreateDirectory(path);
        }

        // 获取目录下所有文件和子目录
        string[] files = Directory.GetFiles(path);
        string[] directories = Directory.GetDirectories(path);

        // 如果没有文件且没有子目录，说明文件夹为空
        return files.Length == 0 && directories.Length == 0;
    }

    void CreateUpdateFlagFile(string path)
    {
        // 检查指定的目录是否存在
        if (!Directory.Exists(path))
        {
            Debug.LogError("Directory does not exist: " + path);
            return;
        }

        // 创建 UpdateFlag 文件的完整路径
        string flagFilePath = Path.Combine(path, "Update.flag");

        // 检查文件是否已存在
        if (File.Exists(flagFilePath))
        {
            Debug.Log("UpdateFlag file already exists.");
        }
        else
        {
            try
            {
                // 创建一个空文件
                File.Create(flagFilePath).Dispose();
                Debug.Log("UpdateFlag file created successfully at: " + flagFilePath);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to create UpdateFlag file: " + ex.Message);
            }
        }
    }

    void DeleteAllFilesAndFolders(string path)
    {

        //先检查当前平台的热更目录是否有文件夹，没有则创建
        if (!Directory.Exists(path))
        {
            //如果没有热更目录，则创建
            Directory.CreateDirectory(path);
        }

        // 删除文件夹下的所有文件
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            try
            {
                File.Delete(file);
                Debug.Log($"Deleted file: {file}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to delete {file}: {ex.Message}");
                CreateUpdateFlagFile(HotUpdateDataPath);
                Destroy(this);
            }
        }

        // 删除文件夹下的所有子文件夹
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            try
            {
                DeleteAllFilesAndFolders(directory); // 递归删除子文件夹和文件
                Directory.Delete(directory);
                Debug.Log($"Deleted folder: {directory}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to delete {directory}: {ex.Message}");
                CreateUpdateFlagFile(HotUpdateDataPath);
                Destroy(this);
            }
        }

        // 删除空文件夹本身
        //try
        //{
        //    Directory.Delete(path);
        //    Debug.Log($"Deleted folder: {path}");
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError($"Failed to delete folder {path}: {ex.Message}");
        //    CreateUpdateFlagFile(HotUpdateDataPath);
        //    Destroy(this);
        //}

    }

    // 下载文件并保存到指定文件夹
    IEnumerator DownloadFile(string fileUrl, string saveFilePath)
    {
        // 创建一个 UnityWebRequest 来下载文件
        UnityWebRequest request = UnityWebRequest.Get(fileUrl);

        // 发送请求并等待响应
        yield return request.SendWebRequest();

        // 检查是否有错误
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("下载文件失败: " + fileUrl + "------------------" + request.error);
            Destroy(this);
        }
        else
        {
            // 确保目标文件夹存在
            string directory = Path.GetDirectoryName(saveFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 将下载的数据保存到本地文件
            File.WriteAllBytes(saveFilePath, request.downloadHandler.data);
            Debug.Log("文件已保存: " + saveFilePath);
        }
    }


    // 下载文本文件并读取内容
    IEnumerator ReadNetTxt(string fileUrl, Action<string> onLoadOver = null)
    {
        // 创建 UnityWebRequest 来获取文本文件
        UnityWebRequest request = UnityWebRequest.Get(fileUrl);

        // 发送请求并等待响应
        yield return request.SendWebRequest();

        // 检查请求是否成功
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("下载文件失败: " + fileUrl + "------------------" + request.error);
            Destroy(this);
        }
        else
        {
            // 获取文件内容（作为文本字符串）
            string fileContents = request.downloadHandler.text;

            onLoadOver?.Invoke(request.downloadHandler.text);
            // 输出文件内容到控制台
            Debug.Log("文件内容:\n" + fileContents);
        }
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
                return "StandaloneWindows64";
            case RuntimePlatform.OSXPlayer:
                return "StandaloneOSX";
            case RuntimePlatform.OSXEditor:
                return "StandaloneOSX";
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

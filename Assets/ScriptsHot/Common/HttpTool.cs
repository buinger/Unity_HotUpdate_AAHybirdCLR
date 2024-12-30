//using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

//using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class HttpTool : MonoBehaviour
{
    public static string loginToken = "";
    public static string projectToken = "";
    private const string urlHead = "https://";
    private const string address = "www.huihangbim.com";
    public static string URL
    {
        get { return urlHead + address; }
    }



   




    public static IEnumerator GetImage(string url, Action<Sprite> onOver = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            // 发送请求并等待响应
            yield return webRequest.SendWebRequest();
            Sprite sprite = null;
            // 检查请求错误
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(url + ":" + webRequest.error);
            }
            else
            {
                // 获取下载的Texture
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                Debug.Log(url);
                // 将Texture2D转换为Sprite
                sprite = TextureToSprite(texture);
            }
            if (onOver != null)
            {
                onOver.Invoke(sprite);
            }
        }

        // 将Texture2D转换为Sprite
        Sprite TextureToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

    }



    /// <summary>
    /// get请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="token"></param>
    /// <param name="urlTail"></param>
    /// <param name="onOver"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static IEnumerator GetRequest<T>(string token, string urlTail, Action<T> onOver, params KeyValuePair<string, string>[] values)
    {
        string targetUrl = URL + urlTail;
        if (values.Length != 0)
        {
            targetUrl += "?";
            for (int i = 0; i < values.Length; i++)
            {
                KeyValuePair<string, string> item = values[i];
                targetUrl += (item.Key + "=" + item.Value);
                if (i != values.Length - 1)
                {
                    targetUrl += "&";
                }
            }
        }


        using (UnityWebRequest request = UnityWebRequest.Get(targetUrl))
        {
            request.SetRequestHeader("authorization", token);

            // 发送请求并等待返回
            yield return request.SendWebRequest();

            // 如果发生错误，输出错误信息
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("新get请求(" + request.url + ")成功，返回结果:" + request.downloadHandler.text);
                // 请求成功时调用回调函数，传递返回的文本数据
                if (typeof(T) == typeof(string))
                {
                    onOver.Invoke((T)(object)request.downloadHandler.text);
                }
                else
                {
                    onOver.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                }
            }
            else
            {
                Debug.Log("新get请求(" + request.url + ")失败，返回结果:" + request.downloadHandler.text);
                // 在这里处理API请求失败的情况
                onOver.Invoke(default(T));
            }
        }
    }



    /// <summary>
    /// post请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="urlTail"></param>
    /// <param name="form"></param>
    /// <param name="onOver"></param>
    /// <returns></returns>
    public static IEnumerator PostRequest<T>(string token, string urlTail, Action<T> onOver, object data)
    {
        if (data is WWWForm)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(URL + urlTail, data as WWWForm))
            {
                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                if (token != "")
                {
                    // 添加请求头部信息
                    request.SetRequestHeader("Authorization", token);
                }
                // 发送请求并等待响应
                yield return request.SendWebRequest();

                // 处理响应
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("新post请求(" + request.url + ")成功，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API响应
                    onOver.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                }
                else
                {
                    Debug.Log("新post请求(" + request.url + ")失败，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API请求失败的情况

                    onOver.Invoke(default(T));
                }
            }
        }
        else
        {
            using (UnityWebRequest request = new UnityWebRequest(URL + urlTail))
            {
                request.method = "post";
                // 设置请求体内容
                string jsonStr = JsonConvert.SerializeObject(data);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.SetRequestHeader("Content-Type", "application/json");
                request.downloadHandler = new DownloadHandlerBuffer();
                if (token != "")
                {
                    // 添加请求头部信息
                    request.SetRequestHeader("Authorization", token);
                }
                // 发送请求并等待响应
                yield return request.SendWebRequest();

                // 处理响应
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("新post请求(" + request.url + ")成功，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API响应
                    onOver.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                }
                else
                {
                    Debug.Log("新post请求(" + request.url + ")失败，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API请求失败的情况

                    onOver.Invoke(default(T));
                }
            }
        }
    }
    /// <summary>
    /// post请求测试
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="urlTail"></param>
    /// <param name="form"></param>
    /// <param name="onOver"></param>
    /// <returns></returns>
    public static IEnumerator PostRequestTest<T>(string token, string urlTail, Action<T> onOver, object data)
    {
        if (data is WWWForm)
        {
            using (UnityWebRequest request = UnityWebRequest.Post("https://www.huihangbim.com" + urlTail, data as WWWForm))
            {
                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                if (token != "")
                {
                    // 添加请求头部信息
                    request.SetRequestHeader("Authorization", token);
                }
                // 发送请求并等待响应
                yield return request.SendWebRequest();

                // 处理响应
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("新post请求(" + request.url + ")成功，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API响应
                    onOver.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                }
                else
                {
                    Debug.Log("新post请求(" + request.url + ")失败，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API请求失败的情况

                    onOver.Invoke(default(T));
                }
            }
        }
        else
        {
            using (UnityWebRequest request = new UnityWebRequest("https://www.huihangbim.com" + urlTail))
            {
                request.method = "post";
                // 设置请求体内容
                string jsonStr = JsonConvert.SerializeObject(data);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.SetRequestHeader("Content-Type", "application/json");
                request.downloadHandler = new DownloadHandlerBuffer();
                if (token != "")
                {
                    // 添加请求头部信息
                    request.SetRequestHeader("Authorization", token);
                }
                // 发送请求并等待响应
                yield return request.SendWebRequest();

                // 处理响应
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("新post请求(" + request.url + ")成功，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API响应
                    onOver.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                }
                else
                {
                    Debug.Log("新post请求(" + request.url + ")失败，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API请求失败的情况

                    onOver.Invoke(default(T));
                }
            }
        }
    }
    /// <summary>
    /// post请求,带API参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="urlTail"></param>
    /// <param name="form"></param>
    /// <param name="onOver"></param>
    /// <returns></returns>
    public static IEnumerator PostRequest<T>(string token, string api, string urlTail, Action<T> onOver, object data)
    {
        if (data is WWWForm)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(URL + urlTail, data as WWWForm))
            {
                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                if (token != "")
                {
                    // 添加请求头部信息
                    request.SetRequestHeader("Authorization", token);
                }
                // 发送请求并等待响应
                yield return request.SendWebRequest();

                // 处理响应
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("新post请求(" + request.url + ")成功，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API响应
                    onOver.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                }
                else
                {
                    Debug.Log("新post请求(" + request.url + ")失败，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API请求失败的情况

                    onOver.Invoke(default(T));
                }
            }
        }
        else
        {
            using (UnityWebRequest request = new UnityWebRequest(URL + urlTail))
            {
                request.method = "post";
                // 设置请求体内容
                string jsonStr = JsonConvert.SerializeObject(data);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.SetRequestHeader("Content-Type", "application/json");
                request.downloadHandler = new DownloadHandlerBuffer();
                if (token != "")
                {
                    // 添加请求头部信息
                    request.SetRequestHeader("Authorization", token);
                }
                // 发送请求并等待响应
                yield return request.SendWebRequest();

                // 处理响应
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("新post请求(" + request.url + ")成功，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API响应
                    onOver.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                }
                else
                {
                    Debug.Log("新post请求(" + request.url + ")失败，返回结果:" + request.downloadHandler.text);
                    // 在这里处理API请求失败的情况

                    onOver.Invoke(default(T));
                }
            }
        }
    }

    ///// <summary>
    ///// post请求,多个token参数
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="urlTail"></param>
    ///// <param name="data"></param>
    ///// <param name="onOver"></param>
    ///// <returns></returns>
    //public static IEnumerator PostRequest<T>(string token, string urlTail, object data, Action<T> onOver, bool isJson = false)
    //{

    //    // 创建UnityWebRequest对象
    //    UnityWebRequest request;

    //    // 设置请求头
    //    if (!isJson)
    //    {
    //        request = UnityWebRequest.Post(URL + urlTail, (WWWForm)data);
    //        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    //    }
    //    else
    //    {
    //        string jsonStr = JsonConvert.SerializeObject(data);
    //        request = UnityWebRequest.Post(URL + urlTail, jsonStr);
    //        request.SetRequestHeader("Content-Type", "application/json");

    //    }
    //    request.SetRequestHeader("authorization", token);

    //    // 发送请求并等待响应
    //    yield return request.SendWebRequest();

    //    // 处理响应
    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log("API请求成功: " + request.downloadHandler.text);
    //        // 在这里处理API响应
    //        onOver.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
    //    }
    //    else
    //    {
    //        Debug.LogError("API请求失败: " + request.error);
    //        // 在这里处理API请求失败的情况

    //        onOver.Invoke(default(T));
    //    }
    //}

}
#region 接收其他类型
public class AnotherInfo : MonoBehaviour
{
    public AnotherInfo instance;
    static JObject jo;
    private static string jsonStr = string.Empty;

    public void Start()
    {
        instance = this;
    }
    public string GetCorouResult(IEnumerator target, int executeIndex)
    {
        for (int i = 0; i < executeIndex; i++)
        {
            target.MoveNext();
        }

        string result = target.Current != null ? target.Current.ToString() : null;
        return result;
    }
    IEnumerator GetInfo(string url)
    {
        if (!(url.Contains("http") || url.Contains("https")))
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("http://175.178.56.185/");
            builder.Append(url);
            url = builder.ToString();


        }

        UnityWebRequest webRequest = UnityWebRequest.Get(url);

        string token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiI1MTUwMTE4MzI3MzAwOTI4MDUiLCJBY2NvdW50IjoiYWRtaW4iLCJVc2VyTmFtZSI6IueuoeeQhuWRmCIsIkFkbWluaXN0cmF0b3IiOjEsIlRlbmFudElkIjoiYWRtaW4iLCJUZW5hbnREYk5hbWUiOiJodWloYW5nX2pucGYiLCJQcm9qZWN0SWQiOiIzNjQ1NzE5MTgyMTQzNjY0NzAiLCJQcm9qZWN0TmFtZSI6IkFS6K-V55So6aG555uuIiwiVGVuYW50QWNjb3VudCI6ImFkbWluIiwiSXNQcm9qZWN0QWRtaW4iOjEsIlJlbGF0aW9uSWQiOiIyNzExNDMzOTU2MTM3MzgyODYiLCJpYXQiOjE3MTM3NjkyOTgsIm5iZiI6MTcxMzc2OTI5OCwiZXhwIjoxNzEzNzc1Mjk4LCJpc3MiOiJ5aW5tYWlzb2Z0IiwiYXVkIjoieWlubWFpc29mdCJ9.h_cU-p0l3JkZX5mRw64X4GZh-PrOXffY2BwA1hhmDiM";

        webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("authorization", token);//请求头文件内容

        yield return webRequest.SendWebRequest();

        while (webRequest.result == UnityWebRequest.Result.InProgress) { }

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string result = webRequest.downloadHandler.text;

            yield return result;
        }
        else
        {

            yield return null;
        }

    }

    public T HTTPGet<T>(string url, string re = "data", string fileName = "")
    {
        jsonStr = GetCorouResult(GetInfo(url), 2);


        if (jsonStr == null) { return default(T); }

        jo = JsonConvert.DeserializeObject<JObject>(jsonStr);

        JToken joCode = jo["code"];
        JToken joData = jo["data"];
        JToken joStatus = jo["status"];

        if (joCode != null)
        {
            if (joCode.ToString().Equals("200"))
            {
                if (!fileName.Equals(""))
                {
                    //DataController.Instance.SaveJsonToLocal(fileName, joData.ToString());
                }

                try
                {
                    if (joData == null || joData.ToString() == "")
                        return default(T);

                    //StringBuilder stringBuilder = new StringBuilder(jo["data"].ToString());
                    string strResult = joData.ToString();
                    if (!re.Equals(""))
                    {
                        strResult = joData[re].ToString();
                        //stringBuilder = new StringBuilder(jo["data"][re].ToString());
                    }

                    if (typeof(T) == typeof(string))
                    {
                        return (T)Convert.ChangeType(strResult, typeof(T));
                    }
                    T data = JsonConvert.DeserializeObject<T>(strResult);
                    return data;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                    return default(T);
                }
            }
        }
        else if (joStatus != null)
        {
            if (joStatus.ToString().Equals("success"))
            {
                try
                {
                    string strResult = jo[re].ToString();
                    if (typeof(T) == typeof(string))
                    {
                        return (T)Convert.ChangeType(strResult, typeof(T));
                    }
                    T data = JsonConvert.DeserializeObject<T>(strResult);
                    return data;
                }
                catch
                {
                    return default(T);
                }
            }
        }
        else
        {
            if (!jo[re].ToString().Equals(""))
            {
                try
                {
                    string strResult = jo[re].ToString();
                    if (typeof(T) == typeof(string))
                    {
                        return (T)Convert.ChangeType(strResult, typeof(T));
                    }
                    T data = JsonConvert.DeserializeObject<T>(strResult);
                    return data;
                }
                catch
                {
                    return default(T);
                }
            }
        }
        return default(T);
    }
}
#endregion


public class UrlTail
{
    public const string login = "/api/oauth/Login";
    public const string projectGantt = "/api/extend/ProjectGantt";
    public const string currentUser = "/api/oauth/CurrentUser";
    public const string projectToken = "/api/oauth/ProjectPick";
    public const string projectModelType = "/api/BIMModel/GetModelTag";
    public const string projectModelInfo = "/api/BIMModel/GetModelList";
    public const string workerInfo = "/api/LabourDevice/QueryWorkerIdCard";

    public const string filterDisciplineList = "/api/LabourDiscipline/FilterDisciplineList";
    public const string saveDisciplineList = "/api/LabourDiscipline/SaveDisciplineList";
    public const string getArProblemList = "/api/ARInspectionServer/GetArProblemList";
    public const string getProblemListNoPage = "/api/ARInspectionProblemServer/GetProblemListNoPage";
    public const string getUserNames = "/api/permission/Users";
    public const string uploadProblem = "/api/ARInspectionServer/SaveQualityInspection";
    public const string searchList = "/api/ModelQrCode/SearchList";
    public const string getInfoById = "/api/ModelQrCode/GetInfoById";
}


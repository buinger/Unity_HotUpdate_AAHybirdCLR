using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class HttpTool : MonoBehaviour
{


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
    public static IEnumerator GetRequest<T>(string URL, string urlTail, string token, Action<T> onOver, params KeyValuePair<string, string>[] values)
    {
        string targetUrl = URL + "/" + urlTail;
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
    public static IEnumerator PostRequest<T>(string URL, string urlTail, string token, Action<T> onOver, object data)
    {
        if (data is WWWForm)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(URL + "/" + urlTail, data as WWWForm))
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
            using (UnityWebRequest request = new UnityWebRequest(URL + "/" + urlTail))
            {
                request.method = "post";
                // 设置请求体内容
                string jsonStr = JsonConvert.SerializeObject(data);
                Debug.Log(jsonStr);
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



}

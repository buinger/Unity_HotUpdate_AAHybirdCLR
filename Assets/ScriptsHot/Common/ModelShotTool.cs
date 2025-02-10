using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModelShotTool : MonoBehaviour
{
    [Range(0.05f, 1.3f)]
    public float cameraRange = 0.17f;
    public Camera renderCamera;
    public RenderTexture renderTexture; // 渲染纹理
    List<GameObject> modelsToRender = new List<GameObject>(); // 要渲染的模型
    public string savePath = "Assets/Textures/ModelShoot";

    public int nowIndex = 0;
    //{modelToRender}.png



    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        renderCamera.orthographicSize = cameraRange;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            RenderModelWithLighting(modelsToRender[nowIndex].name);
            if (nowIndex + 1 < modelsToRender.Count)
            {
                nowIndex++;
                ShowIndexGameObject(nowIndex);
            }
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        foreach (Transform child in transform)
        {
            modelsToRender.Add(child.gameObject);
        }
        ShowIndexGameObject(nowIndex);
    }



    void ShowIndexGameObject(int index)
    {
        foreach (GameObject model in modelsToRender)
        {
            model.SetActive(false);
        }
        modelsToRender[index].SetActive(true);

    }

    void RenderModelWithLighting(string pngName)
    {

        // 配置摄像机
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = new Color(0, 0, 0, 0); // 透明背景
        renderCamera.allowHDR = false; // 启用 HDR
        renderCamera.gameObject.SetActive(true);

        // 使用 RenderTexture 创建 Texture2D
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        // 激活渲染纹理并渲染摄像机
        RenderTexture.active = renderTexture;

        // 将 RenderTexture 的内容读取到 Texture2D 中
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        //AdjustBrightness(texture, 100f); // 1.2f 增加亮度，数值可根据需要调整
        //AdjustContrast(texture, 100f); // 1.3f 增加对比度，数值可根据需要调整
       // ApplyGammaCorrection(texture, 2.2f);
        texture.Apply();

        // 保存为 PNG 文件，包含透明度和光照
        byte[] bytes = texture.EncodeToPNG();

        if (Directory.Exists(savePath) == false)
        {
            Directory.CreateDirectory(savePath);
        }

        File.WriteAllBytes(Path.Combine(savePath, $"{pngName}.png"), bytes);

        // 清理设置
        RenderTexture.active = null;
        Destroy(texture);
        Debug.Log("Rendered image with lighting saved to: " + savePath);
    }

    // 应用伽马校正方法
    private void ApplyGammaCorrection(Texture2D texture, float gamma)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = Mathf.Pow(pixels[i].r, 1.0f / gamma);
            pixels[i].g = Mathf.Pow(pixels[i].g, 1.0f / gamma);
            pixels[i].b = Mathf.Pow(pixels[i].b, 1.0f / gamma);
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    private void AdjustBrightness(Texture2D texture, float brightness)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = Mathf.Clamp01(pixels[i].r * brightness);
            pixels[i].g = Mathf.Clamp01(pixels[i].g * brightness);
            pixels[i].b = Mathf.Clamp01(pixels[i].b * brightness);
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    private void AdjustContrast(Texture2D texture, float contrast)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = Mathf.Clamp01((pixels[i].r - 0.5f) * contrast + 0.5f);
            pixels[i].g = Mathf.Clamp01((pixels[i].g - 0.5f) * contrast + 0.5f);
            pixels[i].b = Mathf.Clamp01((pixels[i].b - 0.5f) * contrast + 0.5f);
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

}

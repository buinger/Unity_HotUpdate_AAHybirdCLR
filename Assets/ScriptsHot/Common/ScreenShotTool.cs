using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotTool
{


    static Texture2D texture2D;
    static RenderTexture render;
    //上传图片的压缩比例 单位： 倍
    static int pictureRatio = 2;
    static Texture2D scaleTexture;
    static Texture2D GetSreenPicture(Rect mRect, bool isCompress = true)
    {
        render = new RenderTexture((int)mRect.width, (int)mRect.height, 0);
        Debug.Log(render);
        Camera.main.targetTexture = render;  //设置目标
        Camera.main.Render();  //开始
        RenderTexture.active = render;  //激活渲染贴图读取信息
        texture2D = new Texture2D((int)Screen.width, (int)Screen.height, TextureFormat.RGBA32, false);
        scaleTexture = new Texture2D((int)(Screen.width / pictureRatio), (int)(Screen.height / pictureRatio), TextureFormat.RGBA32, true);
        texture2D.ReadPixels(mRect, 0, 0);  //读取截屏信息并存储为纹理数据
        texture2D.Apply();

        var temp = texture2D;
        if (isCompress)
        {
            pictureRatio = 2;
            for (int i = 0; i < scaleTexture.height; i++)//压缩图片
            {
                for (int j = 0; j < scaleTexture.width; j++)
                {
                    Color color = texture2D.GetPixel(j * pictureRatio, i * pictureRatio);
                    scaleTexture.SetPixel(j, i, color);

                }
            }
            scaleTexture.Apply();
            temp = scaleTexture;
        }
        else
        {
            pictureRatio = 1;
        }

        //重置相关参数，以使用camera继续在屏幕上显示
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(render);
        return temp;
    }

    /// <summary>
    /// 截屏
    /// </summary>
    public static IEnumerator ScreenShoot(Action<Sprite> onOver = null, bool save = true)
    {
        Rect mRect = new Rect(0, 0, Screen.width, Screen.height);

        yield return new WaitForEndOfFrame();

        Texture2D texture2D = GetSreenPicture(mRect);

        //  byte[] bytes = texture2D.EncodeToPNG();
        if (onOver != null)
        {
            onOver.Invoke(TextureToSprite(texture2D));
        }
        if (save)
        {
            string fileName = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            IoTools.Picture_IO.SaveImageToAlbum(texture2D, "JiePing", fileName);
        }


    }


    // 将 Texture2D 转换为 Sprite 的方法
    public static Sprite TextureToSprite(Texture2D texture)
    {
        Debug.Log(texture.width + "---" + texture.height);
        // 创建 Sprite，使用整个纹理
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTip_Window : Tip_Window
{
   public MaskableGraphic imager;

    public void SetTextAndSprite(ImageTipData imageTipData)
    {
        SetTextValue(imageTipData.text);
        SetGraphicImage(imager, imageTipData.sprite);
    }

     void SetGraphicImage(MaskableGraphic targetGraphic,Sprite sprite)
    {
        if (targetGraphic is Image)
        {
            Image image = targetGraphic as Image;
            if (sprite != null)
            {
                image.sprite = sprite;
            }
            else
            {
                Debug.LogWarning("New Sprite is not assigned.");
            }
        }
        else if (targetGraphic is RawImage)
        {
            RawImage rawImage = targetGraphic as RawImage;
            if (sprite != null)
            {
                rawImage.texture = sprite.texture;
            }
            else
            {
                Debug.LogWarning("New Texture is not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("Target Graphic is not an Image or RawImage.");
        }
    }


}

public class ImageTipData
{
    public string text;
    public Sprite sprite;

    // 构造函数，初始化text和sprite
    public ImageTipData(string text, Sprite sprite)
    {
        this.text = text;
        this.sprite = sprite;
    }
}

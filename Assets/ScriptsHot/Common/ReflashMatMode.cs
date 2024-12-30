using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflashMatMode : MonoBehaviour
{
    MeshRenderer meshRender;
    float lastAlphaValue = 1;
    // Start is called before the first frame update
    void Start()
    {
        meshRender = transform.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lastAlphaValue != meshRender.sharedMaterial.color.a)
        {
            if (meshRender.sharedMaterial.color.a == 1)
            {
                //meshRender.sharedMaterial.SetFloat("_Mode", 0);
                SetMaterialToOpaque();
            }
            else
            {
                // meshRender.sharedMaterial.SetFloat("_Mode", 3);

                SetMaterialToTransparent();
            }
            lastAlphaValue = meshRender.sharedMaterial.color.a;
        }

    }


    void SetMaterialToOpaque()
    {
        // 设置材质渲染模式为不透明
        meshRender.sharedMaterial.SetFloat("_Mode", 0); // 不透明

        // 设置混合模式为完全不透明
        meshRender.sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        meshRender.sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);

        // 启用深度写入
        meshRender.sharedMaterial.SetInt("_ZWrite", 1);

        // 禁用 alpha 测试、alpha 混合和 alpha 预乘
        meshRender.sharedMaterial.DisableKeyword("_ALPHATEST_ON");
        meshRender.sharedMaterial.DisableKeyword("_ALPHABLEND_ON");
        meshRender.sharedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        // 设置渲染队列为 Geometry
        meshRender.sharedMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
    }


    void SetMaterialToTransparent()
    {
        // 设置材质渲染模式为透明
        meshRender.sharedMaterial.SetFloat("_Mode", 3); // 透明

        // 设置混合模式为 alpha 混合
        meshRender.sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        meshRender.sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        // 禁用深度写入
        meshRender.sharedMaterial.SetInt("_ZWrite", 1);

        // 禁用 alpha 测试，启用 alpha 混合
        meshRender.sharedMaterial.DisableKeyword("_ALPHATEST_ON");
        meshRender.sharedMaterial.EnableKeyword("_ALPHABLEND_ON");
        meshRender.sharedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        // 设置渲染队列为 Transparent
        meshRender.sharedMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

}

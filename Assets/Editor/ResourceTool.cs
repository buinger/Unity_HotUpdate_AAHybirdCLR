using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTool : MonoBehaviour
{
    [MenuItem("资源操作/将选中脚本转化成utf8格式 %&U", priority = 205)]
    public static void ConvertScriptsToUtf8()
    {
        object[] allTargetObj = Selection.objects;//这个函数可以得到你选中的对象
        foreach (Object targetObj in allTargetObj)
        {
            string path = AssetDatabase.GetAssetPath(targetObj);
            if (Path.GetFileName(path).Contains(".cs"))
            {
                ConvertFileToUtf8(path);
                Debug.Log(path + ":脚本已转换为无签名的UTF-8格式。");
            }
        }
    }

    private static void ConvertFileToUtf8(string filePath)
    {
        // 读取文件内容
        string content = File.ReadAllText(filePath, Encoding.GetEncoding("GB2312"));

        // 将内容写入为无签名的UTF-8格式
        File.WriteAllText(filePath, content, new UTF8Encoding(false));
    }


    [MenuItem("资源操作/将选中图片转成精灵格式 %#Z", priority = 200)]
    static void EditTexture2()
    {
        object[] allTargetObj = Selection.objects;//这个函数可以得到你选中的对象
        foreach (Object targetObj in allTargetObj)
        {
            if (targetObj && targetObj is Texture)
            {
                string path = AssetDatabase.GetAssetPath(targetObj);
                TextureImporter texture = AssetImporter.GetAtPath(path) as TextureImporter;
                texture.textureType = TextureImporterType.Sprite;
                texture.spritePixelsPerUnit = 1;
                texture.filterMode = FilterMode.Trilinear;
                texture.mipmapEnabled = false;
                texture.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                AssetDatabase.ImportAsset(path);
            }
        }
    }
    [MenuItem("资源操作/将选中精灵生成在Canvas内 %&SPACE", priority = 201)]
    static void ImageMake()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            return;
        }
        object[] allTargetObj = Selection.objects;//这个函数可以得到你选中的对象
        foreach (Object targetObj in allTargetObj)
        {
            if (targetObj && targetObj is Texture)
            {
                string path = AssetDatabase.GetAssetPath(targetObj);
                //string rPath = System.Text.RegularExpressions.Regex.Replace(path, "Assets/Resources/", "");
                //Debug.Log(rPath);
                TextureImporter texture = AssetImporter.GetAtPath(path) as TextureImporter;
                if (texture.textureType == TextureImporterType.Sprite)
                {

                    //Sprite s=(Sprite)Resources.Load(rPath);
                    //Object pic = AssetDatabase.LoadAssetAtPath<Sprite>(path)/* Resources.Load("texture/a", typeof(Sprite))*/;
                    //Sprite s = Instantiate(pic) as Sprite;
                    Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    float w = s.rect.width;
                    float h = s.rect.height;
                    Object obj = Resources.Load("model/ImageModel");
                    GameObject gobj = (GameObject)Instantiate(obj, canvas.transform);
                    Image img = gobj.GetComponent<Image>();
                    img.rectTransform.sizeDelta = new Vector2(w, h);
                    img.sprite = s;
                    img.gameObject.name = s.name;
                }
            }
        }
    }
    [MenuItem("资源操作/复制选中resource文件的短路径 %#X", priority = 202)]
    static void CopyPath()
    {
        UnityEngine.Object targetObj = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(targetObj);
        path = System.Text.RegularExpressions.Regex.Replace(path, "Assets/Resources/", "");
        GUIUtility.systemCopyBuffer = path;

    }
    [MenuItem("资源操作/失活和激活选中gameobject &q", priority = 203)]
    public static void ActiveGameObject()
    {
        GameObject go = Selection.activeGameObject;
        if (go == null) return;

        // 检查是否是预制件实例
        if (PrefabUtility.IsPartOfPrefabInstance(go))
        {
            // 注册到Undo系统
            Undo.RecordObject(go, "Toggle Prefab Instance Active State");

            // 切换激活状态
            bool isActive = !go.activeSelf;
            go.SetActive(isActive);

            // 应用实例的修改到预制件
            PrefabUtility.RecordPrefabInstancePropertyModifications(go);
        }
        else if (PrefabUtility.IsPartOfPrefabAsset(go))
        {
            // 如果是预制件原型，提示无法直接修改
            Debug.LogWarning("无法直接修改预制件原型的激活状态。");
        }
        else
        {
            // 普通GameObject的情况
            Undo.RecordObject(go, "Toggle GameObject Active State");
            bool isActive = !go.activeSelf;
            go.SetActive(isActive);
        }

        // 标记对象为已更改
        EditorUtility.SetDirty(go);
    }

    //快捷键设置文本物体名字
    [MenuItem("资源操作/将剪切板内容黏贴到选中的Text内 &w", priority = 204)]
    public static void SetTextObjName()
    {
        GameObject go = Selection.activeGameObject;
        if (go == null) return;
        string aimContent = GetClipBoardText();
        Text aimText = go.GetComponent<Text>();
        //if (aimText == null)
        //{
        //      go.name = aimContent;
        //      return;
        //}
        if (aimText != null && aimContent != "" && aimContent != null)
        {
            //aimText.gameObject.name = aimContent;
            aimText.text = aimContent;
            //aimText.color = Color.black;
        }
    }



    public static string GetClipBoardText()
    {
        string message = GUIUtility.systemCopyBuffer;
        return message;
    }
}

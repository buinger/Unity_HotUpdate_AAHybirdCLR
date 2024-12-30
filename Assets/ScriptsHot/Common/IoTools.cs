using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
//using ICSharpCode.SharpZipLib.Zip;
using System.Security.Cryptography;
//using ICSharpCode.SharpZipLib.Core;
using System.Text;
using System.IO.Compression;

public class IoTools
{
    public static string photoPathToSend;
    public static class Model_IO
    {

        public static string modelRootPath = Path.Combine(Application.persistentDataPath, "ModelDatas");
        
        public static string GetModelDownloadPath(string modelUrlPath)
        {
            string modelPath = modelUrlPath.Replace("/glbs", "");
            modelPath = modelPath.Replace("/", "-");
            modelPath = Path.Combine(modelRootPath, modelPath);
            return modelPath;
        }

       

    }


    public static string ReadFileString(string filePath)
    {
        // ȷ���ļ�����
        if (!File.Exists(filePath))
        {
            Debug.LogError("�ļ������ڣ� " + filePath);
            return "";
        }

        // ��ȡ�ļ������ֽ�
        try
        {
            return File.ReadAllText(filePath);
        }
        catch (IOException e)
        {
            Debug.LogError("IO������ " + e.Message);
            return "";
        }
    }

    public static byte[] ReadFileBytes(string filePath)
    {
        // ȷ���ļ�����
        if (!File.Exists(filePath))
        {
            Debug.LogError("�ļ������ڣ� " + filePath);
            return null;
        }

        // ��ȡ�ļ������ֽ�
        try
        {
            return File.ReadAllBytes(filePath);
        }
        catch (IOException e)
        {
            Debug.LogError("IO������ " + e.Message);
            return null;
        }
    }

    // �ж�ĳ·���Ƿ����
    public static bool PathExists(string path)
    {
        return Directory.Exists(path);
    }

    // ɾ��ĳ���ļ���
    public static void DeleteFolder(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            //Debug.Log($"�ļ��� {path} ��ɾ��");
        }
        else
        {
            Debug.LogWarning($"�ļ��� {path} ������");
        }
    }

    // �½�ĳ���ļ���
    public static void CreateFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            // Debug.Log($"�ļ��� {path} �Ѵ���");
        }
        else
        {
            Debug.LogWarning($"�ļ��� {path} �Ѵ���");
        }
    }

    // �ж��ļ��Ƿ����
    public static bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    // ɾ��ĳ���ļ�
    public static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            //Debug.Log($"�ļ� {filePath} ��ɾ��");
        }
        else
        {
            Debug.LogWarning($"�ļ� {filePath} ������");
        }
    }


    public static void SaveByteFile(string filePath, byte[] bytes)
    {
        File.WriteAllBytes(filePath, bytes);
    }


    // �½�ĳ���ļ�
    public static void CreateFile(string filePath, string content = "")
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, content);
            Debug.Log($"�ļ� {filePath} �Ѵ���");
        }
        else
        {
            Debug.LogWarning($"�ļ� {filePath} �Ѵ���");
        }
    }

    /// <summary>
    /// ������ת��Ϊbyte����
    /// </summary>
    /// <param name="obj">��ת������</param>
    /// <returns>ת����byte����</returns>
    public static byte[] Object2Bytes(object obj)
    {
        byte[] buff;
        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter iFormatter = new BinaryFormatter();
            iFormatter.Serialize(ms, obj);
            buff = ms.GetBuffer();
        }
        return buff;
    }

    /// <summary>
    /// ��byte����ת���ɶ���
    /// </summary>
    /// <param name="buff">��ת��byte����</param>
    /// <returns>ת����ɺ�Ķ���</returns>
    public static object Bytes2Object(byte[] buff)
    {
        object obj;
        using (MemoryStream ms = new MemoryStream(buff))
        {
            IFormatter iFormatter = new BinaryFormatter();
            obj = iFormatter.Deserialize(ms);
        }
        return obj;
    }

    /// <summary>
    /// ������л���
    /// </summary>
    public static void ClearCach(string path)
    {
        Debug.LogError("��ջ��棬��ʵ��");

        //if (Directory.Exists(savePath))
        //{
        //    Directory.Delete(savePath, true);
        //}
    }


    /// <summary>
    /// �°��ѹ��ͨ���ļ�
    /// </summary>
    /// <param name="zipFilePath"></param>
    /// <param name="extractPath"></param>
    public static void ExtractZipFile(string zipFilePath, string extractPath)
    {
       
    }

    /// <summary>
    /// �ֽ�����ѹ��zip
    /// </summary>
    /// <param name="zipBytes"></param>
    /// <param name="extractPath"></param>
    public static void ExtractZipFile(byte[] zipBytes, string extractPath)
    {
        
    }

  

    public static class Json_IO
    {
        /// <summary>
        /// ����Json�ļ�������
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="JsonContent"></param>
        public static void SaveJsonToLocal(string fileName, string JsonContent)
        {
            string fullPath = Application.persistentDataPath + "/" + fileName;

            int lastIndex = fullPath.LastIndexOf('/');

            DirectoryInfo dir = new DirectoryInfo(fullPath.Substring(0, lastIndex));
            if (!dir.Exists)
                dir.Create();

            if (!fileName.EndsWith(".json"))
            {
                fullPath += ".json";
            }

            FileInfo fileInfo = new FileInfo(fullPath);
            if (fileInfo.Exists)
                fileInfo.Delete();

            StreamWriter writer = fileInfo.CreateText();
            writer.Write(JsonContent);
            writer.Flush();
            writer.Dispose();
            writer.Close();
        }

        /// <summary>
        /// ��ȡ���������ص�Json�ļ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="dataType"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static T ReadJsonFromLocal<T>(string filename, string dataType, string suffix = ".json")
        {
            var fullPath = filename;
            if (!File.Exists(filename))
            {
                fullPath = Application.persistentDataPath + "/" + filename;
            }
            if (!fullPath.EndsWith(suffix))
            {
                fullPath += suffix;
            }

            if (!File.Exists(fullPath))
                return default;
            //Debug.Log("B_"+fullPath);
            string content = File.ReadAllText(fullPath);
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(content, typeof(T));
            }

            if (!dataType.Equals("") && !dataType.Equals(string.Empty))
            {
                JObject j = (JObject)JsonConvert.DeserializeObject(content);

                content = j[dataType].ToString();
            }

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(content, typeof(T));
            }
            T data = JsonConvert.DeserializeObject<T>(content);
            return data;
        }

    }



    public static class Picture_IO
    {
        /// <summary>
        /// ���ظ��豸·���µ�ͼƬ
        /// </summary>
        /// <returns></returns>
        public static List<Texture2D> LoadAllPicture()
        {
            List<string> filePaths = new List<string>();
            List<Texture2D> textureList = new List<Texture2D>();

            string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG";
            string[] ImageType = imgtype.Split('|');

            for (int i = 0; i < ImageType.Length; i++)
            {
                //��ȡApplication.dataPath�ļ��������е�ͼƬ·��  
                string[] dirs = Directory.GetFiles((Application.dataPath + "/Picture/"), ImageType[i]);
                for (int j = 0; j < dirs.Length; j++)
                {
                    filePaths.Add(dirs[j]);
                }
            }

            for (int i = 0; i < filePaths.Count; i++)
            {

                byte[] _bytes = File.ReadAllBytes(filePaths[i]);
                Texture2D tx = new Texture2D(100, 100);
                tx.LoadImage(_bytes);
                textureList.Add(tx);
            }

            return textureList;
        }



        public static void SaveImageToAlbum(Texture2D texture2D, string fileName, string name)
        {
            //string photoAlbumPath = string.Empty;
            //string photoPath = string.Empty;
            //switch (Application.platform)
            //{
            //    case RuntimePlatform.Android:

            //        photoAlbumPath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android", StringComparison.Ordinal));
            //        photoPath = photoAlbumPath + "/DCIM//" + name;
            //        SaveImage(texture2D, fileName, photoPath + ".png", SaveImageBack);
            //        photoPathToSend = photoPath + ".png";
            //        break;

            //    case RuntimePlatform.IPhonePlayer:

            //        photoPath = Application.persistentDataPath + "/" + name;
            //        SaveImage(texture2D, fileName, photoPath + ".png", SaveImageBack);
            //        photoPathToSend = photoPath + ".png";
            //        break;

            //    default:
            //        if (!Directory.Exists(Application.persistentDataPath + "/PhotoAlbum"))
            //        {
            //            Directory.CreateDirectory(Application.persistentDataPath + "/PhotoAlbum");
            //        }
            //        photoPath = Application.persistentDataPath + "/PhotoAlbum/" + name + ".png";
            //        SaveImage4Windows(texture2D, photoPath, SaveImageBack);
            //        photoPathToSend = photoPath;
            //        break;
            //}




            void SaveImageBack(bool success, string path)
            {
                if (success)
                {
                    Debug.Log("����ɹ�");
                }
                else
                {
                    Debug.Log("����ʧ��");
                }
            }
        }

        private static void SaveImage4Windows(Texture2D texture, string path, Action<bool, string> callback)
        {
            byte[] bytes = texture.EncodeToPNG();
            try
            {
                File.WriteAllBytes(path, bytes);
                Debug.Log("File written to: " + path);
                callback?.Invoke(true, path);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save image: " + e);
                callback?.Invoke(false, path);
            }
        }
        /// <summary>
        /// ���ر���ͼƬ
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        //public static Texture2D LoadImage(string imagePath)
        //{
        //    if (File.Exists(imagePath))
        //    {
        //        return NativeGallery.LoadImageAtPath(imagePath, 512, false);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //private static void SaveImage(Texture2D picture, string albumName, string fileNameFormatted, NativeGallery.MediaSaveCallback back = null)
        //{
        //    NativeGallery.SaveImageToGallery(picture, albumName, fileNameFormatted, back);
        //}

        //public static Texture2D LoadImageFromAlbum(string fileName, string name)
        //{
        //    string photoAlbumPath = string.Empty;
        //    string photoPath = string.Empty;
        //    switch (Application.platform)
        //    {
        //        case RuntimePlatform.Android:

        //            photoAlbumPath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android", StringComparison.Ordinal));
        //            photoPath = photoAlbumPath + "/DCIM/Camera/" + name;
        //            break;

        //        case RuntimePlatform.IPhonePlayer:

        //            photoPath = Application.persistentDataPath + "/" + name;
        //            break;

        //        default:
        //            if (!Directory.Exists(Application.dataPath + "/PhotoAlbum"))
        //            {
        //                Directory.CreateDirectory(Application.dataPath + "/PhotoAlbum");
        //            }
        //            photoPath = Application.dataPath + "/PhotoAlbum/" + name;
        //            break;
        //    }

        //    return LoadImage(photoPath + ".png");

        //}

       
    }

    public static class Audio_IO
    {
        /// <summary>
        /// ����Ƶ�ļ�ת���ַ���
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static string AudioClipToByte(AudioClip clip)
        {
            if (clip == null)
                return string.Empty;

            float[] floatData = new float[clip.samples * clip.channels];
            clip.GetData(floatData, 0);
            byte[] outData = new byte[floatData.Length];
            Buffer.BlockCopy(floatData, 0, outData, 0, outData.Length);
            return Convert.ToBase64String(outData);
        }

        /// <summary>
        /// ���ַ���ת������Ƶ�ļ�
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static AudioClip StringToAudio(string content)
        {
            if (content.Equals(""))
                return null;

            byte[] bytes = Convert.FromBase64String(content);
            float[] samples = new float[bytes.Length];
            Buffer.BlockCopy(bytes, 0, samples, 0, bytes.Length);
            AudioClip clip = AudioClip.Create("RecordClip", samples.Length, 1, 16000, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }

    

}

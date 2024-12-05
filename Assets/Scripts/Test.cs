using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Test : MonoBehaviour
{

    public string prefabAddress; // 预制件的 Addressable 地址
    private GameObject loadedPrefab; // 加载的预制件实例


    //// Start is called before the first frame update
    //void Start()
    //{
    //    Addressables.LoadAssetAsync<GameObject>(prefabAddress).Completed += OnPrefabLoaded;
    //}

    //// 处理加载完成的回调
    //private void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj)
    //{
    //    if (obj.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        Debug.Log($"预制件 {prefabAddress} 加载成功！");

    //        // 实例化预制件
    //        loadedPrefab = Instantiate(obj.Result);
    //    }
    //    else
    //    {
    //        Debug.LogError($"预制件 {prefabAddress} 加载失败！");
    //    }
    //}
}

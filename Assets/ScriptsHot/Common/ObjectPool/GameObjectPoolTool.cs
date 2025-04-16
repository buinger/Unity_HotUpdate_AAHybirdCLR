using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public class GameObjectPoolTool : MonoBehaviour
{
    public static GameObject GetFromPool(bool active, string fullname)
    {
        string aimFullName = fullname;
        aimFullName = GameObjectPool.FormatPath4AALoad(aimFullName);

        GameObject aimGameObj;
        aimGameObj = GameObjectPool.OutPool(aimFullName);
        if (aimGameObj == null)
        {
            return null;
        }
        else
        {
            //aimGameObj.transform.SetParent(null);
            aimGameObj.SetActive(active);
            return aimGameObj;
        }
    }

    public static async Task<GameObject> GetFromPoolForceAsync(bool active, string fullname)
    {
        string aimFullName = fullname;
        aimFullName = GameObjectPool.FormatPath4AALoad(aimFullName);
        GameObject aimGameObj;
        aimGameObj = GameObjectPool.OutPool(aimFullName);

        if (aimGameObj == null)
        {
            Task getResource = GameObjectPool.PreLoadPrefabToPoolAsync(aimFullName);
            await getResource;
            aimGameObj = GameObjectPool.OutPool(aimFullName);
            aimGameObj.SetActive(active);
            return aimGameObj;
        }
        else
        {
            //aimGameObj.transform.SetParent(null);
            aimGameObj.SetActive(active);
            return aimGameObj;
        }
    }

    public static GameObject GetFromPoolForce(bool active, string fullname)
    {
        string aimFullName = fullname;
        aimFullName = GameObjectPool.FormatPath4AALoad(aimFullName);
        GameObject aimGameObj;
        aimGameObj = GameObjectPool.OutPool(aimFullName);

        if (aimGameObj == null)
        {
            GameObjectPool.PreLoadPrefabToPool(aimFullName);
            aimGameObj = GameObjectPool.OutPool(aimFullName);
            aimGameObj.SetActive(active);
            return aimGameObj;
        }
        else
        {
            //aimGameObj.transform.SetParent(null);
            aimGameObj.SetActive(active);
            return aimGameObj;
        }
    }



    public static void PutInPool(GameObject gameObj)
    {
        GameObjectPool.InPool(gameObj);
    }

    public static void Release(bool releaseAll = false)
    {
        if (releaseAll)
        {
            GameObjectPool.ReleaseAll();
        }
        else
        {
            GameObjectPool.ReleaseEmptyObj();
        }

        // 最后清理Unity内存
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }





    //-------------------------------Plot池

    public class GameObjectPool
    {
        //池字典

        public static Dictionary<string, List<GameObject>> poolItem = new Dictionary<string, List<GameObject>>();
        public static Dictionary<string, List<GameObject>> allItem = new Dictionary<string, List<GameObject>>();
        //加载过的资源
        public static Dictionary<string, AsyncOperationHandle<GameObject>> loadedPrefabs = new Dictionary<string, AsyncOperationHandle<GameObject>>();


        static void DestoryAllObj()
        {

            foreach (var item in allItem)
            {
                foreach (GameObject obj in item.Value)
                {
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }
            poolItem.Clear();
            allItem.Clear();
        }
        public static void ReleaseAll()
        {
            // 先销毁池内的对象
            DestoryAllObj();
            foreach (var handle in GameObjectPool.loadedPrefabs)
            {
                if (handle.Value.IsValid())
                {
                    Addressables.Release(handle.Value);
                }
            }
            GameObjectPool.loadedPrefabs.Clear();
        }


        static void ClearEmptyObj()
        {
            List<GameObject> allEmptyObj = new List<GameObject>();
            foreach (var item in poolItem)
            {
                List<GameObject> tempEmptyObj = new List<GameObject>();
                foreach (GameObject obj in item.Value)
                {
                    if (obj == null)
                    {
                        tempEmptyObj.Add(obj);
                        allEmptyObj.Add(obj);
                    }
                }
                foreach (GameObject obj in tempEmptyObj)
                {
                    item.Value.Remove(obj);
                }
            }
            List<GameObject> tempAllEmptyObj = new List<GameObject>();
            foreach (var item in allItem)
            {
                List<GameObject> tempEmptyObj = new List<GameObject>();
                foreach (GameObject obj in item.Value)
                {
                    if (obj == null)
                    {
                        tempEmptyObj.Add(obj);
                        tempAllEmptyObj.Add(obj);
                    }
                }
                foreach (GameObject obj in tempEmptyObj)
                {
                    item.Value.Remove(obj);
                }
            }
        }

        public static void ReleaseEmptyObj()
        {

            ClearEmptyObj();

            List<string> removePoolObjKey = new List<string>();
            foreach (var item in allItem)
            {
                if (item.Value.Count == 0)
                {
                    if (loadedPrefabs[item.Key].IsValid())
                    {
                        Addressables.Release(loadedPrefabs[item.Key]);
                    }
                }
            }

        }


        public static void InPool(GameObject gameObj)
        {
            //Button but = gameObj.GetComponent<Button>();
            //if (but != null)
            //{
            //    but.onClick.RemoveAllListeners();
            //}

            if (poolItem.ContainsKey(gameObj.name) == false)
            {
                poolItem.Add(gameObj.name, new List<GameObject>());
            }

            gameObj.SetActive(false);

            if (!poolItem[gameObj.name].Contains(gameObj))
            {
                poolItem[gameObj.name].Add(gameObj);
            }

        }

        public static GameObject OutPool(string assetPath)
        {
            if (poolItem.ContainsKey(assetPath) == false)
            {
                poolItem.Add(assetPath, new List<GameObject>());
            }

            if (poolItem[assetPath].Count == 0)
            {
                return null;
            }
            else
            {
                GameObject outGo = poolItem[assetPath][0];
                poolItem[assetPath].Remove(outGo);

                if (outGo != null)
                {
                    //这里有竞争条件，可能要Lock      
                    outGo.SetActive(true);
                    return outGo;
                }
                else
                {
                    return OutPool(assetPath);
                }
            }
        }


        /// <summary>
        /// 通过路径获取预制件名字
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetNameFromPrefabPath(string path)
        {
            string target = "";
            string[] strs = path.Split('/');
            if (strs.Length != 0)
            {
                string temp = strs[strs.Length - 1];
                target = FormatPath4AALoad(temp);
            }
            return target;
        }


        public static string FormatPath4AALoad(string oringnal)
        {
            string target = oringnal;

            return target;
        }


        /// <summary>
        /// 协程
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerator PreLoadPrefabToPoolIE(string path)
        {
            string assetPath = path;
            assetPath = FormatPath4AALoad(assetPath);

            Task<GameObject> loadTask = LoadGameObjectAsync(path);
            while (!loadTask.IsCompleted)
            {
                yield return null;
            }

            GameObject loadedObject = Instantiate(loadTask.Result);
            loadedObject.name = path;
            InPool(loadedObject); if (allItem.ContainsKey(assetPath) == false)
            {
                allItem.Add(assetPath, new List<GameObject>());
            }
            if (allItem[assetPath].Contains(loadedObject) == false)
            {
                allItem[assetPath].Add(loadedObject);
            }
            yield return loadedObject;

        }




        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task<GameObject> PreLoadPrefabToPoolAsync(string path)
        {
            string assetPath = path;
            assetPath = FormatPath4AALoad(assetPath);
            Task<GameObject> loadTask = LoadGameObjectAsync(path);
            await loadTask;
            GameObject loadedObject = Instantiate(loadTask.Result);
            loadedObject.name = path;
            InPool(loadedObject);
            if (allItem.ContainsKey(assetPath) == false)
            {
                allItem.Add(assetPath, new List<GameObject>());
            }
            if (allItem[assetPath].Contains(loadedObject) == false)
            {
                allItem[assetPath].Add(loadedObject);
            }
            return loadedObject;
        }


        public static GameObject PreLoadPrefabToPool(string path)
        {
            string assetPath = path;
            assetPath = FormatPath4AALoad(assetPath);
            GameObject gameObj = LoadGameObject(path);
            GameObject loadedObject = Instantiate(gameObj);
            loadedObject.name = path;
            InPool(loadedObject);
            if (allItem.ContainsKey(assetPath) == false)
            {
                allItem.Add(assetPath, new List<GameObject>());
            }
            if (allItem[assetPath].Contains(loadedObject) == false)
            {
                allItem[assetPath].Add(loadedObject);
            }
            return loadedObject;
        }


        static async Task<GameObject> LoadGameObjectAsync(string address)
        {
            if (loadedPrefabs.ContainsKey(address))
            {
                if (loadedPrefabs[address].IsValid())
                {
                    return loadedPrefabs[address].Result;
                }
                else
                {
                    loadedPrefabs.Remove(address);
                }
            }
            var handle = Addressables.LoadAssetAsync<GameObject>(address);

            // 等待异步加载完成
            await handle.Task;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                GameObject gameObj = handle.Result;
                loadedPrefabs.Add(address, handle);
                return gameObj;
            }
            else
            {
                Debug.LogError($"Failed to load addressable asset: {address}");
                return null;
            }

        }

        static GameObject LoadGameObject(string address)
        {
            if (loadedPrefabs.ContainsKey(address))
            {
                if (loadedPrefabs[address].IsValid())
                {
                    return loadedPrefabs[address].Result;
                }
                else
                {
                    loadedPrefabs.Remove(address);
                }
            }

            // 使用同步阻塞等待异步加载
            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            handle.WaitForCompletion();

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                GameObject gameObj = handle.Result;
                loadedPrefabs.Add(address, handle);

                return gameObj;
            }
            else
            {
                Debug.LogError($"Failed to load addressable asset: {address}");
                return null;
            }

        }
    }
}


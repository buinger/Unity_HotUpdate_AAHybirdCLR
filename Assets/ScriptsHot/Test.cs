using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class Test : MonoBehaviour
{
    public PrefabInfo targetGameObj;
    private GameObject loadedPrefab; // 加载的预制件实例


    // Start is called before the first frame update
    void Start()
    {
        GameObjectPoolTool.GetFromPoolForce(true, targetGameObj.path);
        
    }

}

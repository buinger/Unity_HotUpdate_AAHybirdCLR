using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class Test : MonoBehaviour
{
    public PrefabInfo targetGameObj;
    private GameObject loadedPrefab; // ���ص�Ԥ�Ƽ�ʵ��


    // Start is called before the first frame update
    void Start()
    {
        GameObjectPoolTool.GetFromPoolForce(true, targetGameObj.path);
        
    }

}

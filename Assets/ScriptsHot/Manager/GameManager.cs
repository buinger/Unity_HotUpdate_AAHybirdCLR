using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Manager<GameManager>
{
    protected override void Ini()
    {
        DontDestroyOnLoad(gameObject);
        //切换场景前释放对象池
        SceneManager.sceneUnloaded += (scene) =>
        {
            GameObjectPoolTool.Release();
        };
    }

}







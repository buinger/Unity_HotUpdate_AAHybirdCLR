using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneTool : MonoBehaviour
{
    public static void ChangeScene(string sceneName)
    { 
        SceneManager.LoadScene(sceneName);
    }

    public static void ChangeAddressableScene(string scenePath,Action onCompleted = null)
    {
        Addressables.LoadSceneAsync(scenePath, LoadSceneMode.Single).Completed += (op) =>
        {
            onCompleted?.Invoke();
        };
    }

}

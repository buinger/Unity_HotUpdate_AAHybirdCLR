using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : Manager<UiManager>
{
    [SerializeField, Header("常用窗口ui资源索引")]
    private PrefabInfo maskWin;
    [SerializeField]
    private PrefabInfo loadMaskWin;
    [SerializeField]
    private PrefabInfo tipWin;
    [SerializeField]
    private PrefabInfo viewTipWin;
    [SerializeField]
    private PrefabInfo imageTipWin;
    [SerializeField]
    private PrefabInfo confirmTipWin;



    [SerializeField, Header("常用元素ui资源索引")]
    public PrefabInfo itemUi;
    public PrefabInfo missionUi;
    public PrefabInfo rewardUi;
    public PrefabInfo heChengTiaoUi;



    List<RunTimeWindow> allUiWindows = new List<RunTimeWindow>();
    List<RunTimeElement> allUiElements = new List<RunTimeElement>();


    static MainBook firstBook;
    static MainBook nowBook;


    public static MainBook NowBook
    {
        set { nowBook = value; }
        get
        {
            if (nowBook == null)
            {
                return firstBook;
            }
            else
            {
                return nowBook;
            }
        }
    }


    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    private PointerEventData eventData;





    /// <summary>
    /// 获取ui元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public RunTimeElement GetUiElement(string path)
    {
        if (path == "")
        {
            Debug.LogError("路径为空");
            return null;
        }

        GameObject targetGameObj = GameObjectPoolTool.GetFromPoolForce(true, path);
        RunTimeElement runTimeElement = targetGameObj.GetComponent<RunTimeElement>();
        RegisterElement(runTimeElement);
        return runTimeElement;

        void RegisterElement(RunTimeElement uiElement)
        {
            if (!allUiElements.Contains(uiElement))
            {
                allUiElements.Add(uiElement);
            }
        }
    }

    /// <summary>
    /// 展示窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public RunTimeWindow ShowUiWindow(CommonWindowType type, object data = null)
    {
        string path = "";

        switch (type)
        {
            case CommonWindowType.透明遮挡:
                path = maskWin.path;
                break;
            case CommonWindowType.加载:
                path = loadMaskWin.path;
                break;
            case CommonWindowType.提示:
                path = tipWin.path;
                break;
            case CommonWindowType.无遮挡提示:
                path = viewTipWin.path;
                break;
            case CommonWindowType.图片提示:
                path = imageTipWin.path;
                break;
            case CommonWindowType.确认与取消:
                path = confirmTipWin.path;
                break;
        }


        if (path == "")
        {
            Debug.LogError(type.ToString() + "窗口:未配置窗口索引");
            return default;
        }

        GameObject targetGameObj = GameObjectPoolTool.GetFromPoolForce(true, path);
        RunTimeWindow uiWin = targetGameObj.GetComponent<RunTimeWindow>();
        RegisterWindow(uiWin);

        switch (type)
        {
            case CommonWindowType.提示:
                (uiWin as Tip_Window).SetTextValue(data.ToString());
                break;
            case CommonWindowType.无遮挡提示:
                (uiWin as ViewTip_Window).SetTextValue(data.ToString());
                break;
            case CommonWindowType.图片提示:
                (uiWin as ImageTip_Window).SetTextAndSprite(data as ImageTipData);
                break;
            case CommonWindowType.确认与取消:
                (uiWin as ConfirmTip_Window).SetAllValue(data as ConfirmTip_Window.IniData);
                break;
        }

        return uiWin;
    }





    public T ShowUiWindow<T>(string winResourcePath)
    {
        GameObject winObj = GameObjectPoolTool.GetFromPoolForce(true, winResourcePath);
        if (winObj == null)
        {
            Debug.LogError(winResourcePath + ":窗口不存在");
            return default(T);
        }
        T winT = winObj.GetComponent<T>();
        if (winT == null)
        {
            Debug.LogError(winResourcePath + ":组件不存在");
            return default(T);
        }

        RunTimeWindow uiWin = winObj.GetComponent<RunTimeWindow>();
        RegisterWindow(uiWin);
        return winT;
    }



    /// <summary>
    /// 注册窗口
    /// </summary>
    /// <param name="uiWin"></param>
    void RegisterWindow(RunTimeWindow uiWin)
    {
        if (!allUiWindows.Contains(uiWin))
        {
            allUiWindows.Add(uiWin);
        }
    }

    /// <summary>
    /// 是否在UI上
    /// </summary>
    /// <returns></returns>
    public bool IsOnUIElement(Vector3 screenPostion, Action<GameObject> onGetTargetUi = null)
    {
        if (graphicRaycaster == null)
        {
            Debug.LogError("无GraphicRaycaster,有问题");
            return false;
        }
        eventData.pressPosition = screenPostion;
        eventData.position = screenPostion;
        List<RaycastResult> list = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, list);
        foreach (var temp in list)
        {
            if (temp.gameObject.layer.Equals(5))
            {
                if (onGetTargetUi != null)
                {
                    onGetTargetUi(temp.gameObject);
                }
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// 获取鼠标ui
    /// </summary>
    /// <returns></returns>
    public GameObject GetMouseUIElement(Vector3 screenPostion)
    {
        if (graphicRaycaster == null)
        {
            Debug.LogError("无GraphicRaycaster,有问题");
            return null;
        }
        eventData.pressPosition = screenPostion;
        eventData.position = screenPostion;
        List<RaycastResult> list = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, list);
        foreach (var temp in list)
        {
            if (temp.gameObject.layer.Equals(5))
            {
                return temp.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取鼠标所有ui
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetMouseUIElements(Vector3 screenPostion)
    {
        if (graphicRaycaster == null)
        {
            Debug.LogError("无GraphicRaycaster,有问题");
            return null;
        }
        eventData.pressPosition = screenPostion;
        eventData.position = screenPostion;
        List<RaycastResult> list = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, list);
        List<GameObject> allUi = new List<GameObject>();
        foreach (var temp in list)
        {
            if (temp.gameObject.layer.Equals(5))
            {
                allUi.Add(temp.gameObject);
            }
        }
        return allUi;
    }

    /// <summary>
    /// 回收所有窗口
    /// </summary>
    public void CloseAllWindows()
    {
        foreach (var item in allUiWindows)
        {
            GameObjectPoolTool.PutInPool(item.gameObject);
        }
    }

    /// <summary>
    /// 回收所有元素
    /// </summary>
    public void CloseAllElements()
    {
        foreach (var item in allUiElements)
        {
            GameObjectPoolTool.PutInPool(item.gameObject);
        }
    }

    void UpdateUiComponet()
    {
        graphicRaycaster = NowBook.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        eventData = new PointerEventData(eventSystem);
    }
    protected override void Ini()
    {
        SceneManager.sceneLoaded += (scene, type) =>
        {
            if (LoadSceneMode.Single == type)
            {
                UpdateUiComponet();
            }
        };
        firstBook = FindObjectOfType<MainBook>();

        DontDestroyOnLoad(firstBook);
        UpdateUiComponet();
    }

    public static void DoDelayFreamDo(Action action)
    {
        instance.StartCoroutine(DelayFreamDo(action));

        static IEnumerator DelayFreamDo(Action action)
        {
            yield return new WaitForFixedUpdate();
            action?.Invoke();
        }
    }

    public static void SetTriggerEvent(Graphic triggerComponent, Action mouseFun, EventTriggerType triggerType)
    {
        EventTrigger eventTrigger = triggerComponent.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = triggerComponent.gameObject.AddComponent<EventTrigger>();
        }
        SetEvent(eventTrigger, mouseFun, triggerType);

        void SetEvent(EventTrigger et, Action mouseFun, EventTriggerType triggerType)
        {
            List<EventTrigger.Entry> entry = new List<EventTrigger.Entry>();
            foreach (var item in et.triggers)
            {
                if (item.eventID == triggerType)
                {
                    entry.Add(item);
                }
            }

            foreach (var item in entry)
            {
                et.triggers.Remove(item);
            }

            EventTrigger.Entry ete = new EventTrigger.Entry();
            ete.eventID = triggerType;
            //ete.callback.RemoveAllListeners();
            ete.callback.AddListener((baseEventData) =>
            {
                mouseFun.Invoke();
            });
            et.triggers.Add(ete);
        }
    }

}

public enum CommonWindowType
{
    透明遮挡,
    加载,
    提示,
    无遮挡提示,
    图片提示,
    确认与取消
}
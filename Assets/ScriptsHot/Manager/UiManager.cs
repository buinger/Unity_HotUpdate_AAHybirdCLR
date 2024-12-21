using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiManager : Manager<UiManager>
{
    [Header("测试用，右键脚本生成选择窗口")]
    public CommonWindowType type;

    //Canvas 的逻辑宽高
    public static Vector2 ScreenLogicalSize = new Vector2(1920, 1080);


    [SerializeField, Header("常用窗口ui资源索引")]
    private PrefabInfo InvisiableCover_Window;
    [SerializeField]
    private PrefabInfo Loading_Window;
    [SerializeField]
    private PrefabInfo Tip_Window;
    [SerializeField]
    private PrefabInfo NoCoverTip_Window;
    [SerializeField]
    private PrefabInfo PictureConfirm_Window;
    [SerializeField]
    private PrefabInfo ConfirmAndCancel_Window;

    private static GraphicRaycaster graphicRaycaster;
    private static EventSystem eventSystem;
    private static PointerEventData eventData;

    static MainBook nowBook;
    public static MainBook NowBook
    {
        set
        {
            nowBook = value;
            Canvas canvas = nowBook.GetComponent<Canvas>();
            ScreenLogicalSize = canvas.GetComponent<RectTransform>().sizeDelta;
            graphicRaycaster = NowBook.GetComponent<GraphicRaycaster>();
            eventSystem = EventSystem.current;
            eventData = new PointerEventData(eventSystem);
        }
        get
        {
            return nowBook;
        }
    }
    List<RunTimeWindow> allRunTimeWindows = new List<RunTimeWindow>();
    protected override void Ini() { }


    [ContextMenu("生成窗口")]
    private void TestCommonWindow()
    {
        ShowCommonRunTimeWindow(type, "测试文本");
    }

    /// <summary>
    /// 展示窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public RunTimeWindow ShowCommonRunTimeWindow(CommonWindowType type, object data)
    {
        string path = "";

        switch (type)
        {
            case CommonWindowType.InvisiableCover:

                path = InvisiableCover_Window.path;
                break;
            case CommonWindowType.Loading:

                path = Loading_Window.path;
                break;
            case CommonWindowType.Tip:

                path = Tip_Window.path;
                break;
            case CommonWindowType.NoCoverTip:

                path = NoCoverTip_Window.path;
                break;
            case CommonWindowType.PictureConfirm:

                path = PictureConfirm_Window.path;
                break;
            case CommonWindowType.ConfirmAndCancel:

                path = ConfirmAndCancel_Window.path;
                break;
        }

        if (path == "")
        {
            Debug.LogError(type.ToString() + "窗口:未配置窗口索引");
            return default;
        }

        GameObject targetGameObj = GameObjectPoolTool.GetFromPoolForce(true, path);
        RunTimeWindow uiWin = targetGameObj.GetComponent<RunTimeWindow>();
        RegisterRunTimeWindow(uiWin);

        switch (type)
        {
            case CommonWindowType.Tip:
                (uiWin as Tip_Window).SetTextValue(data.ToString());
                break;
            case CommonWindowType.NoCoverTip:
                (uiWin as NoCoverTip_Window).SetTextValue(data.ToString());
                break;
            case CommonWindowType.PictureConfirm:
                (uiWin as PictureConfirm_Window).SetTextAndSprite(data as ImageTipData);
                break;
            case CommonWindowType.ConfirmAndCancel:
                (uiWin as ConfirmAndCancel_Window).SetAllValue(data as ConfirmAndCancelData);
                break;
        }

        return uiWin;
    }

    /// <summary>
    /// 展示一个自定义窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="winResourcePath"></param>
    /// <returns></returns>
    public T ShowCustomRunTimeWindow<T>(string winResourcePath) where T : RunTimeWindow
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
        RegisterRunTimeWindow(uiWin);
        return winT;
    }

    /// <summary>
    /// 注册窗口
    /// </summary>
    /// <param name="uiWin"></param>
    void RegisterRunTimeWindow(RunTimeWindow uiWin)
    {
        if (!allRunTimeWindows.Contains(uiWin))
        {
            allRunTimeWindows.Add(uiWin);
        }
    }

    /// <summary>
    /// 屏幕坐标是否在UI上
    /// </summary>
    /// <returns></returns>
    public bool IsPointOnUIElement(Vector3 screenPostion, Action<GameObject> onGetTargetUi = null)
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
    /// 获取屏幕坐标上的UI元素
    /// </summary>
    /// <returns></returns>
    public GameObject GetPointUiElement(Vector3 screenPostion)
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
    /// 获取屏幕坐标穿透的所有UI元素
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetPointUiElements(Vector3 screenPostion)
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
    /// 回收所有动态窗口
    /// </summary>
    public void ReleaseAllRunTimeWindows()
    {
        foreach (var item in allRunTimeWindows)
        {
            if (item != null)
            {
                GameObjectPoolTool.PutInPool(item.gameObject);
            }
        }
        allRunTimeWindows.Clear();
    }

    /// <summary>
    /// 延迟到下一帧执行事件
    /// </summary>
    /// <param name="action"></param>
    public static void DoDelayFreamDo(Action action)
    {
        instance.StartCoroutine(DelayFreamDo(action));

        static IEnumerator DelayFreamDo(Action action)
        {
            yield return new WaitForFixedUpdate();
            action?.Invoke();
        }
    }

    /// <summary>
    /// 设置ui的按下抬起等事件
    /// </summary>
    /// <param name="triggerComponent"></param>
    /// <param name="mouseFun"></param>
    /// <param name="triggerType"></param>
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
    //透明窗口，用于阻止用户点击其他ui
    InvisiableCover,
    //加载窗口
    Loading,
    //有遮罩提示窗口
    Tip,
    //无遮罩提示窗口
    NoCoverTip,
    //图片提示窗口
    PictureConfirm,
    //确认取消窗口
    ConfirmAndCancel
}



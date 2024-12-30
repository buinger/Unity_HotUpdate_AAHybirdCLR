
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

/// <summary>
/// 情节系统基类
/// </summary>
public abstract class Plot : MonoBehaviour
{
    [Tooltip("Plot当前状态,动态显示")]
    public string nowStatus;
    [Tooltip("Plot类型,动态显示")]
    public string nowType;
    //流程当前状态
    PlotStatus plotStatus = PlotStatus.Over;
    [Tooltip("活着的子Plot")]
    [SerializeField]
    List<Plot> nowAliveChildPlots = new List<Plot>();
    [Tooltip("所有子Plot")]
    List<Plot> allChildPlots = new List<Plot>();

    [Tooltip("结束前的Plot")]
    [SerializeField]
    protected ChildPlotInformation overPlot;

    [Header("以上为plot必有字段")]
    public string tip = "以上为plot必有字段";


    //父Plot交代的事情，在Plot停止和结束时必定执行
    UnityEvent fatherEndEntrust = new UnityEvent();

    //默认设置为子Plot，只有被plotcontroller生成的才被标记为父plot
    PlotType type = PlotType.Child;

    //子Plot激活后，一直未开始，则20秒后要回收，目的是防止僵尸Plot
    private float childRecycleTime = 60;
    private float timePassSinceEnabled = 0;
    private bool playFlag = false;
    private bool iniFlag = false;

    public Action onPlotOver;

    public string PlotName
    {
        get
        {
            PrefabInfo prefabInfo = transform.GetComponent<PrefabInfo>();
            if (prefabInfo != null)
            {
                return prefabInfo.name;
            }
            else
            {
                Debug.LogError("此预制件并未配置PrefabInfo脚本或未赋值");
                return "";
            }
        }
    }

    protected string plotPath
    {
        get
        {
            PrefabInfo prefabInfo = transform.GetComponent<PrefabInfo>();
            if (prefabInfo != null)
            {
                return prefabInfo.path;
            }
            else
            {
                Debug.LogError("此预制件并未配置PrefabInfo脚本或未赋值");
                return "";
            }
        }
    }


    /// <summary>
    /// 初始化时
    /// </summary>
    void Start()
    {
        Ini(OnIniOver);
    }

    protected virtual void IniTransform()
    {
        //transform.SetParent(null);
        transform.position = Vector3.zero;
    }

    public void BindStartAction(ref Action startAction)
    {
        if (type == PlotType.Father)
        {
            startAction += (() =>
              {
                  foreach (var item in PlotController.instance.allFatherPlots)
                  {
                      if (item != this)
                      {
                          item.StopPlot();
                      }
                  }
                  if (this != PlotController.instance.playingPlot)
                  {
                      StopPlot();
                  }
                  PlotController.instance.playingPlot = this;
                  StartPlot();
              });
        }
    }


    private async void OnIniOver()
    {
        await Task.Run(() =>
        {
            //还要加个判定表格都读完的条件
            while (PlotController.instance == null)
            {
                Task.Delay(100);
            }
        });

        IniTransform();
        PlotController.instance.Register(this);
        Debug.Log(PlotName + "Ini()函数执行完毕，初始化成功");
        iniFlag = true;
    }

    //一定要在最后执行onIniOver，不然plot永远不会执行
    protected abstract void Ini(Action onIniOver);


    private void OnEnable()
    {
        timePassSinceEnabled = 0;
        playFlag = false;
    }

    //public void ChangeToChild()
    //{
    //    type = PlotType.Child;
    //}

    public void ChangeToFather()
    {
        type = PlotType.Father;
    }

    private void Update()
    {
        if (type == PlotType.Child && playFlag == false && iniFlag == true)
        {
            timePassSinceEnabled += Time.deltaTime;
            if (timePassSinceEnabled >= childRecycleTime)
            {
                timePassSinceEnabled = 0;
                GameObjectPoolTool.PutInPool(gameObject);
            }
        }
    }

    private void OnGUI()
    {
        nowStatus = plotStatus.ToString();
        nowType = type.ToString();
    }


    /// <summary>
    /// Plot整体过程
    /// </summary>
    /// <returns></returns>
    private IEnumerator Play()
    {
        playFlag = true;
        plotStatus = PlotStatus.Playing;

        //执行前先回收掉僵尸Plot
        if (PlotType.Father == type)
        {
            PlotController.instance.RecycleChildPlot();
        }

        //如果初始话未完成，阻塞协程直到完成
        while (iniFlag == false)
        {
            yield return null;
            Debug.Log(PlotName + ":阻塞中");
        }

        ResetPlot();

        yield return StartCoroutine(MainLogic());

        if (overPlot.plotModel != null)
        {
            yield return StartCoroutine(StartEndPlot());
        }

        fatherEndEntrust.Invoke();
        fatherEndEntrust.RemoveAllListeners();
        plotStatus = PlotStatus.Over;

        if (onPlotOver != null)
        {
            onPlotOver.Invoke();
        }
    }

    protected virtual IEnumerator StartEndPlot()
    {
        yield return StartNewPlot(overPlot);
    }

    /// <summary>
    /// Plot的主要流程功能,子类重写实现
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator MainLogic();


    /// <summary>
    ///  开启一个子Plot，注意：只能在MainLogic()内调用
    /// </summary>
    /// <param name="newPlot"></param>
    /// <param name="willBlock"></param>
    /// <returns></returns>
    protected IEnumerator StartNewPlot(ChildPlotInformation childPlotModel, Action<Plot> onInstatiatePlot = null)
    {
        if (childPlotModel.plotModel != null)
        {
            IEnumerator getPlotEntity = GetPlotEntity(childPlotModel);
            yield return StartCoroutine(getPlotEntity);


            GameObject newPlotGameObj = getPlotEntity.Current as GameObject;
            newPlotGameObj.transform.localScale = Vector3.one;
            Plot newPlot = newPlotGameObj.GetComponent<Plot>();
            if (onInstatiatePlot != null)
            {
                onInstatiatePlot.Invoke(newPlot);
            }

            //子Plot，强停在这里成为未执行的的僵尸Plot,难点，需要解决,（已解决，当激活后5秒内没有开始自动入池）
            nowAliveChildPlots.Add(newPlot);
            allChildPlots.Add(newPlot);
            //子Plot，强停在这里成为父List中无法移除的子Plot，不过没事，已在之后直接clear.
            newPlot.fatherEndEntrust.AddListener(() =>
            {
                if (nowAliveChildPlots.Contains(newPlot))
                {
                    nowAliveChildPlots.Remove(newPlot);
                }
            });

            newPlot.StartPlot();
            if (childPlotModel.blockable)
            {
                plotStatus = PlotStatus.Blocked;
                //子Plot，强停在这里成为无法还原父Plot的阻塞,但是不重要，因为父Plot已经是要强制停止了，没必要还原

                //委托任务结束后取消阻塞
                newPlot.fatherEndEntrust.AddListener(() =>
                {
                    plotStatus = PlotStatus.Playing;
                });
                while (plotStatus == PlotStatus.Blocked)
                {
                    yield return new WaitForEndOfFrame();
                }
                GameObjectPoolTool.PutInPool(newPlotGameObj);
            }
        }
        else
        {
            Debug.Log(transform.name + ":开启子Plot失败，模型为空");
        }
    }

    [ContextMenu("开始(仅测试)")]
    /// <summary>
    /// 开始Plot
    /// </summary>
    private void StartPlot()
    {
        //判定当前状态是否为结束，只有结束才能开始
        if (plotStatus == PlotStatus.Over)
        {
            //如果是父Plot，开始前先回收失效的子Plot
            //if (type == PlotType.Father)
            //{
            //    PlotController.instance.RecycleChildPlot();
            //}
            //触发开始流程逻辑
            StartCoroutine(Play());
        }
    }

    [ContextMenu("停止(仅测试)")]
    private void StopPlotTest()
    {
        StopPlot(false);
    }
    [ContextMenu("停止并重置(仅测试)")]
    private void StopPlotResetTest()
    {
        StopPlot(true);
    }

    /// <summary>
    /// 停止Plot
    /// </summary>
    /// <param name="ifReset"></param>
    public void StopPlot(bool ifReset = true)
    {
        if (plotStatus != PlotStatus.Over)
        {
            if (allChildPlots.Count != 0)
            {
                foreach (var item in allChildPlots)
                {
                    if (nowAliveChildPlots.Contains(item))
                    {
                        item.StopPlot();
                    }
                }
                nowAliveChildPlots.Clear();
                allChildPlots.Clear();
            }

            //结束前完成爸爸交代的事情
            fatherEndEntrust.Invoke();
            fatherEndEntrust.RemoveAllListeners();

            //停止现在的行为
            StopAllCoroutines();

            //标志已结束
            plotStatus = PlotStatus.Over;

            //还原状态到从未开始状态(也可不还原)

            //if (type == PlotType.Child)
            //{
            //    GameObjectPoolManager.PutInPool(gameObject);
            //}
        }
        if (ifReset && iniFlag)
        {
            ResetPlot();
        }
    }


    protected IEnumerator GetGameObjectEntity(bool active, string fullPath, Vector3 pos, Transform father, bool ifLocalSet)
    {
        GameObject aimGameObject = null;

        yield return StartCoroutine(GameObjectPoolTool.GameObjectPool.PreLoadPrefabToPoolIE(fullPath));

        aimGameObject = GameObjectPoolTool.GetFromPool(active, fullPath);
        aimGameObject.transform.SetParent(father);
        if (ifLocalSet)
        {
            aimGameObject.transform.localPosition = pos;
        }
        else
        {
            aimGameObject.transform.position = pos;
        }

        yield return aimGameObject;
    }

    protected async Task<GameObject> GetGameObjectEntityAsync(bool active, string fullPath, Vector3 pos, Transform father, bool ifLocalSet)
    {
        GameObject aimGameObject = GameObjectPoolTool.GetFromPool(active, fullPath);
        if (aimGameObject == null)
        {
            await GameObjectPoolTool.GameObjectPool.PreLoadPrefabToPoolAsync(fullPath);
            aimGameObject = GameObjectPoolTool.GetFromPool(active, fullPath);
        }

        aimGameObject.transform.SetParent(father);
        if (ifLocalSet)
        {
            aimGameObject.transform.localPosition = pos;
        }
        else
        {
            aimGameObject.transform.position = pos;
        }
        return aimGameObject;
    }

    protected IEnumerator GetUiEntity(bool active, string fullPath, Vector3 localPos, Transform father)
    {
        GameObject aimGameObject = null;

        yield return StartCoroutine(GameObjectPoolTool.GameObjectPool.PreLoadPrefabToPoolIE(fullPath));
        aimGameObject = GameObjectPoolTool.GetFromPool(active, fullPath);
        aimGameObject.transform.SetParent(father);
        aimGameObject.transform.localRotation = Quaternion.identity;
        aimGameObject.transform.localScale = Vector3.one;
        aimGameObject.transform.localPosition = localPos;
        yield return aimGameObject;
    }

    protected async Task<GameObject> GetUiEntityAsync(bool active, string fullPath, Vector3 localPos, Transform father)
    {
        GameObject aimGameObject = null;

        // 使用异步等待确保加载完成
        await GameObjectPoolTool.GameObjectPool.PreLoadPrefabToPoolAsync(fullPath);
        aimGameObject = GameObjectPoolTool.GetFromPool(active, fullPath);

        aimGameObject.transform.SetParent(father);
        aimGameObject.transform.localRotation = Quaternion.identity;
        aimGameObject.transform.localScale = Vector3.one;
        aimGameObject.transform.localPosition = Vector3.zero;
        return aimGameObject;
    }


    IEnumerator GetPlotEntity(ChildPlotInformation childPlotModel)
    {
        GameObject aimGameObject = null;
        string targetPlotPrefabPath = childPlotModel.plotModel.plotPath;

        Transform father = null;
        UiPlot testType = childPlotModel.plotModel as UiPlot;
        if (testType != null)
        {
            father = UiManager.NowBook.transform;
        }

        yield return StartCoroutine(GameObjectPoolTool.GameObjectPool.PreLoadPrefabToPoolIE(targetPlotPrefabPath));
        aimGameObject = GameObjectPoolTool.GetFromPool(true, targetPlotPrefabPath);
        aimGameObject.transform.SetParent(father);
        aimGameObject.transform.localPosition = Vector3.zero;
        yield return aimGameObject;
    }

    async Task<GameObject> GetPlotEntityAnsyc(ChildPlotInformation childPlotModel)
    {
        GameObject aimGameObject = null;
        string targetPlotPrefabPath = childPlotModel.plotModel.plotPath;

        await GameObjectPoolTool.GameObjectPool.PreLoadPrefabToPoolAsync(targetPlotPrefabPath);
        aimGameObject = GameObjectPoolTool.GetFromPool(true, targetPlotPrefabPath);
        aimGameObject.transform.SetParent(null);
        aimGameObject.transform.localPosition = Vector3.zero;
        return aimGameObject;
    }



    [ContextMenu("重新开始(仅测试)")]
    /// <summary>
    /// 重新开始Plot
    /// </summary>
    void RestartPlot()
    {
        StopPlot();
        StartPlot();
    }

    /// <summary>
    /// 重置Plot环境，保证Plot状态为Play之前状态
    /// </summary>
    protected abstract void ResetPlot();

    /// <summary>
    /// Plot状态
    /// </summary>
    private enum PlotStatus
    {
        //结束·未开始状态·等待开始状态 
        Over,
        //正在跑的状态
        Playing,
        //阻塞状态
        Blocked,
    }

    /// <summary>
    /// Plot状态
    /// </summary>
    protected enum PlotType
    {
        //父Plot
        Father,
        //子Plot
        Child,
    }

}



public abstract class UiPlot : Plot
{
    protected override void IniTransform()
    {
        Transform father = UiManager.NowBook.transform;

        if (father != null)
        {
            transform.SetParent(father);

            RectTransform rectTrans = transform.GetComponent<RectTransform>();

            rectTrans.sizeDelta = new Vector2(Screen.width, Screen.height);

            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogError("没有找到UiPlot容器");
            base.IniTransform();
        }
    }

}




[System.Serializable]
//子Plot模板
public class ChildPlotInformation
{
    public bool blockable;
    public Plot plotModel;
    //public Vector3 bornLocalPostion=Vector3.zero;

    // 构造函数  
    public ChildPlotInformation(bool blockable, Plot plotModel)
    {
        this.blockable = blockable;
        this.plotModel = plotModel;
    }


    public ChildPlotInformation()
    {
        this.blockable = true;
        this.plotModel = null;
    }
}
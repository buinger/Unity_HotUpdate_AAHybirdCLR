using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public abstract class ElementPanel<T> : StaticUi
{
    public Transform elementContainer;

    public Dictionary<Transform, T> elementDic = new Dictionary<Transform, T>();

    public GameObject bkg;


    protected async virtual void Awake()
    {
        if (DataManager.instance == null)
        {
            await Task.Delay(500);
        }
        IniElement();
        if (bkg == null)
        {
            bkg = elementContainer.gameObject;
        }
    }


    protected virtual void IniElement()
    {
        for (int i = 0; i < elementContainer.childCount; i++)
        {
            Transform targetTrans = elementContainer.GetChild(i);
            AddElement(targetTrans);
        }
    }

    protected virtual void AddElement(Transform trans)
    {
        T element = trans.GetComponent<T>();
        if (element != null)
        {
            if (!elementDic.ContainsKey(trans))
            {
                elementDic.Add(trans, element);
                trans.SetParent(elementContainer);
                trans.localRotation = Quaternion.identity;
                trans.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogError("添加元素失败，元素已存在：" + trans.name);
            }
        }
        else
        {
            Debug.LogError("添加元素失败，找不到元素的组件：" + trans.name);
        }

    }

    protected virtual void RemoveElement(Transform trans)
    {
        if (elementDic.ContainsKey(trans))
        {
            elementDic.Remove(trans);
            trans.SetParent(null);
            GameObjectPoolTool.PutInPool(trans.gameObject);
        }
        else
        {
            Debug.LogError("移除元素出错，不存在此元素：" + trans.name);
        }
    }



}


public abstract class ElementDragPanel<T> : ElementPanel<T>
{
    public List<MonoBehaviour> otherDragComponents = new List<MonoBehaviour>();
    public float checkRadius = 116;
    public float holdingCd = 1;
    public Transform dragFather = null;

    protected float holdingPassTime = 0;

    protected List<GameObject> elementGameObjs = new List<GameObject>();
    protected GameObject nowDragElement = null;
    protected GameObject clickDownElement = null;
    private int nowDragElementLastSiblingIndex = -1;
    protected bool holdingBreak = false;
    protected GridLayoutGroup gridLayoutGroup;

    protected virtual void OnBeginDrag(GameObject dragElement)
    {
        SwitchOtherDragComponents(false);
    }
    protected virtual void OnEndDrag(GameObject dragElement)
    {
        SwitchOtherDragComponents(true);
    }
    protected override void Awake()
    {
        base.Awake();
        if (dragFather == null)
        {
            dragFather = UiManager.NowBook.transform;
        }

        gridLayoutGroup = transform.GetComponentInChildren<GridLayoutGroup>();
        Image containerImage = elementContainer.GetComponent<Image>();
        if (containerImage == null)
        {
            containerImage = elementContainer.AddComponent<Image>();
            containerImage.color = new Color(0, 0, 0, 0);
        }
        ScrollRect scrollRect = GetComponentInChildren<ScrollRect>();
        if (scrollRect != null)
        {
            otherDragComponents.Add(scrollRect);
        }
    }


    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseLeftDown();
        }
        else if (Input.GetMouseButton(0))
        {
            OnMouseLeftHolding();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnMouseLeftUp();
        }
    }


    protected override void AddElement(Transform trans)
    {
        base.AddElement(trans);
        elementGameObjs.Add(trans.gameObject);
        // trans.SetAsLastSibling();
    }

    protected override void RemoveElement(Transform trans)
    {
        base.RemoveElement(trans);
        elementGameObjs.Remove(trans.gameObject);
    }

    protected virtual void SwitchOtherDragComponents(bool active)
    {
        foreach (var item in otherDragComponents)
        {
            item.enabled = active;
        }
    }

    protected virtual void OnMouseLeftDown()
    {
        holdingPassTime = 0;
        holdingBreak = false;
        GameObject downObj = UiManager.instance.GetMouseUIElement(Input.mousePosition);
        if (elementGameObjs.Contains(downObj))
        {
            clickDownElement = downObj;
        }
    }
    protected virtual void OnMouseLeftHolding()
    {
        if (Input.GetAxis("Mouse X") != 0)
        {
            holdingPassTime = 0;
            holdingBreak = true;
        }

        if (nowDragElement != null)
        {
            nowDragElement.transform.position = Input.mousePosition;
        }
        else
        {
            holdingPassTime += Time.deltaTime;
            if (holdingPassTime >= holdingCd)
            {
                holdingPassTime = 0;
                if (holdingBreak == false)
                {
                    GameObject uiGameObj = UiManager.instance.GetMouseUIElement(Input.mousePosition);
                    if (uiGameObj != null && elementGameObjs.Contains(uiGameObj) && uiGameObj == clickDownElement)
                    {
                        gridLayoutGroup.enabled = false;
                        nowDragElement = uiGameObj;
                        nowDragElementLastSiblingIndex = nowDragElement.transform.GetSiblingIndex();
                        nowDragElement.transform.SetParent(dragFather);
                        nowDragElement.transform.SetAsLastSibling();
                        // 强制重新构建布局
                        // LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.GetComponent<RectTransform>());

                        OnBeginDrag(nowDragElement);
                    }
                }
            }
        }
    }
    protected virtual void OnMouseLeftUp()
    {
        if (nowDragElement != null && elementGameObjs.Contains(nowDragElement))
        {
            List<GameObject> allUis = UiManager.instance.GetMouseUIElements(Input.mousePosition);
            nowDragElement.transform.SetParent(elementContainer);
            if (allUis.Count <= 1)
            {
                Debug.Log("拖拽到空白处");
                if (nowDragElementLastSiblingIndex >= 0)
                {
                    nowDragElement.transform.SetSiblingIndex(nowDragElementLastSiblingIndex);
                }
                else
                {
                    nowDragElement.transform.SetAsLastSibling();
                }
            }
            else
            {
                Transform nearestTouchElement = null;
                nearestTouchElement = GetClosestElement(nowDragElement.transform);
                float dis = Vector3.Distance(nearestTouchElement.position, nowDragElement.transform.position);
                //Debug.Log(dis);
                if (nearestTouchElement != null && dis < checkRadius)
                {
                    int targetIndex = nearestTouchElement.GetSiblingIndex();

                    if (nearestTouchElement.position.x > nowDragElement.transform.position.x)
                    {
                        nowDragElement.transform.SetSiblingIndex(targetIndex);
                    }
                    else
                    {
                        nowDragElement.transform.SetSiblingIndex(targetIndex + 1);
                    }
                }
                else
                {
                    List<GameObject> gameos = UiManager.instance.GetMouseUIElements(Input.mousePosition);


                    if ((!gameos.Contains(bkg)) && nowDragElementLastSiblingIndex >= 0)
                    {

                        nowDragElement.transform.SetSiblingIndex(nowDragElementLastSiblingIndex);

                    }
                    else
                    {
                        nowDragElement.transform.SetAsLastSibling();
                    }
                }

            }
            gridLayoutGroup.enabled = true;
            // 强制重新构建布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.GetComponent<RectTransform>());
            OnEndDrag(nowDragElement);
            nowDragElement = null;
        }
        else if (holdingBreak == false && clickDownElement != null && holdingPassTime <= holdingCd / 2)
        {
            Debug.Log("点击了" + clickDownElement.name);
        }
        nowDragElementLastSiblingIndex = -1;
        holdingPassTime = 0;
        holdingBreak = false;
        clickDownElement = null;

    }



    protected Transform GetClosestElement(Transform targetTrans)
    {
        GameObject closestElement = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject element in elementGameObjs)
        {
            if (element != nowDragElement.gameObject && element.activeSelf == true)
            {
                float distance = Vector3.Distance(element.transform.position, targetTrans.position);

                if (distance == 0)
                {
                    Debug.Log(element.name + "---" + targetTrans.name);
                }

                // 找到距离最小的元素
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestElement = element;
                }
            }

        }
        if (closestElement != null)
        {
            return closestElement.transform;
        }
        else
        {
            return null;
        }
    }


}

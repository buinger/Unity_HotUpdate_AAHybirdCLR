using UnityEngine;
using UnityEngine.UI;

public class StaticUi_ImageDragPanel : ElementDragPanel<Image>
{
    protected override void Awake()
    {
        base.Awake();
        foreach (var item in elementGameObjs)
        {
            Transform targetTrans = item.transform;
            targetTrans.GetComponent<Image>().color = new Color(Random.Range(0.01f, 1f), Random.Range(0.01f, 1f), Random.Range(0.01f, 1f), 1);
        }
    }

}

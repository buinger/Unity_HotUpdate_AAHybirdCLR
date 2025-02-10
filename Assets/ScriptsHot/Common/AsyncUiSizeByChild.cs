using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncUiSizeByChild : MonoBehaviour
{
    public Vector2 offset = new Vector2(0, 0);
    public float minX = 0;
    public float minY = 0;
    public bool asyncX = true;
    public bool asyncY = true;
    RectTransform self;
    RectTransform child;
    // Start is called before the first frame update
    void Start()
    {
        self = transform.GetComponent<RectTransform>();
        child = transform.GetChild(0).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 size = new Vector2(asyncX ? child.sizeDelta.x : self.sizeDelta.x, asyncY ? child.sizeDelta.y : self.sizeDelta.y);
        if (size.x < minX)
        {
            size.x = minX;
        }
        if (size.y < minY)
        {
            size.y = minY;
        }
        self.sizeDelta = size + offset;
    }
}

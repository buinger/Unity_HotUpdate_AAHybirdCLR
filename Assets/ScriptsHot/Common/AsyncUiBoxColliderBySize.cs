using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncUiBoxColliderBySize : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private RectTransform rectTransform;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rectTransform = GetComponent<RectTransform>();
        boxCollider.size = rectTransform.rect.size;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (boxCollider.size != rectTransform.rect.size)
        {
            boxCollider.size = rectTransform.rect.size;
        }
    }
}

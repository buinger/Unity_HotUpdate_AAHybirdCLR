using UnityEngine;

public abstract class WorldCanvas : StaticUi
{
    protected virtual void Update()
    {
        transform.localPosition = Vector3.zero;
        transform.LookAt(Camera.main.transform.position);
    }
}
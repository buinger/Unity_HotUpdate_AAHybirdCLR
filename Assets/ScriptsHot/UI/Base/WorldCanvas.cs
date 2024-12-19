using UnityEngine;

public abstract class WorldCanvas : StaticUi
{
    protected virtual void Update()
    {
        transform.localPosition = Vector3.zero;
        transform.LookAt(-CameraController.instance.mainCamera.transform.position);
    }
}
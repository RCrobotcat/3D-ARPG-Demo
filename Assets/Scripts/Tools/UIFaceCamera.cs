using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    private Camera mainCamera; // 主摄像机

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 让按钮UI的正面始终面向摄像机
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }
}

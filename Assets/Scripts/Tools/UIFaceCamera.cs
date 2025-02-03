using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    private Camera mainCamera; // �������

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // �ð�ťUI������ʼ�����������
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }
}

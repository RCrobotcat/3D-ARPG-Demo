using UnityEngine;

public class RemoteEnemyControl : MonoBehaviour
{
    Vector3 lastPos;
    [HideInInspector] public Animator animator;
    float speed;

    void Awake()
    {
        animator = GetComponent<Animator>();
        lastPos = transform.position;
    }

    void Update()
    {
        // 计算速度
        speed = (transform.position - lastPos).magnitude / Time.deltaTime * 1.5f;

        animator.SetFloat("Speed", speed);
        lastPos = transform.position;
    }
}

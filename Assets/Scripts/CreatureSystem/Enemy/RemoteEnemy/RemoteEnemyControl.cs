using UnityEngine;

// 远程怪物控制器
public class RemoteEnemyControl : MonoBehaviour
{
    private Vector3 lastPos;
    [HideInInspector] public Animator animator;
    private float speed;

    RemoteEnemy remoteEnemy;

    void Awake()
    {
        animator = GetComponent<Animator>();
        remoteEnemy = GetComponent<RemoteEnemy>();
        lastPos = transform.position;
    }

    void Update()
    {
        speed = (transform.position - lastPos).magnitude / Time.deltaTime;
        animator.SetFloat("Speed", speed);

        lastPos = transform.position;
    }
}

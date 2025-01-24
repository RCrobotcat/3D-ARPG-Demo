using System.Collections;
using UnityEngine;

public class WanderBehaviors : MonoBehaviour
{
    public Vector2 targetChangeRange = new Vector2(2, 6); // 随机时间的范围 Range of random time
    public float wanderRadius = 2f; // 漫游半径 Wander radius
    public float targetHeight = 0.5f;

    [HideInInspector] public Vector3 targetPosition; // 目标位置 Target position
    SteeringBehaviors steeringBehaviors;
    Rigidbody rb;

    void Awake()
    {
        steeringBehaviors = GetComponent<SteeringBehaviors>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine(targetPositionChange());
    }

    /// <summary>
    /// 定期更换目标位置 Regularly change the target position
    /// </summary>
    IEnumerator targetPositionChange()
    {
        while (true)
        {
            float theta = Random.value * 2 * Mathf.PI;
            Vector3 wanderTarget = new Vector3(
                wanderRadius * Mathf.Cos(theta),
                0,
                wanderRadius * Mathf.Sin(theta));
            wanderTarget.Normalize();
            wanderTarget *= wanderRadius;

            targetPosition = transform.position + wanderTarget;
            targetPosition.y = targetHeight;

            yield return new WaitForSeconds(Random.Range(targetChangeRange.x, targetChangeRange.y));
        }
    }

    /// <summary>
    /// 获取Wander行为的加速度 Get the acceleration of the Wander behavior
    /// </summary>
    public Vector3 GetSteering()
    {
        Debug.DrawLine(transform.position, targetPosition, Color.gray);

        return steeringBehaviors.Seek(targetPosition);
    }
}
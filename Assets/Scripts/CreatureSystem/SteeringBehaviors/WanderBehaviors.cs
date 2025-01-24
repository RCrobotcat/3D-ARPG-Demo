using System.Collections;
using UnityEngine;

public class WanderBehaviors : MonoBehaviour
{
    public Vector2 targetChangeRange = new Vector2(2, 6); // ���ʱ��ķ�Χ Range of random time
    public float wanderRadius = 2f; // ���ΰ뾶 Wander radius
    public float targetHeight = 0.5f;

    [HideInInspector] public Vector3 targetPosition; // Ŀ��λ�� Target position
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
    /// ���ڸ���Ŀ��λ�� Regularly change the target position
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
    /// ��ȡWander��Ϊ�ļ��ٶ� Get the acceleration of the Wander behavior
    /// </summary>
    public Vector3 GetSteering()
    {
        Debug.DrawLine(transform.position, targetPosition, Color.gray);

        return steeringBehaviors.Seek(targetPosition);
    }
}
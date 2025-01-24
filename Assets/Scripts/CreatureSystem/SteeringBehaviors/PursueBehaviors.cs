using UnityEngine;

// ׷����Ϊ Pursue Behaviors
public class PursueBehaviors : MonoBehaviour
{
    public float maxPredictionTime = 1f;

    Rigidbody rb;
    SteeringBehaviors steeringBehaviors;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        steeringBehaviors = GetComponent<SteeringBehaviors>();
    }

    public Vector3 GetSteering(Rigidbody target)
    {
        Vector3 displacement = target.position - transform.position;
        float distance = displacement.magnitude;

        float speed = rb.velocity.magnitude;
        float prediction; // �����׷��ʱ�� Estimated chase time
        if (speed <= distance / maxPredictionTime)
        {
            prediction = maxPredictionTime;
        }
        else
        {
            prediction = distance / speed;
        }

        Vector3 explicitTarget = target.position + target.velocity * prediction;

        return steeringBehaviors.Seek(explicitTarget);
    }
}

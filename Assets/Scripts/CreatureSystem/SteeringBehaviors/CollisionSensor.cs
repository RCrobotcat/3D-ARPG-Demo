using UnityEngine;

// ��ײ���, ֮�������Ϊ Collision detection and obstacle avoidance behavior
public class CollisionSensor : MonoBehaviour
{
    public float rayStart = 0.5f; // ������ʼλ�� ray start position
    public float rayLength = 10f; // ���߳��� ray length
    public int rayCount = 36; // �������� ray count
    public LayerMask collisionLayers; // ��ײ�� collision layer

    /// <summary>
    /// ͨ������ķ��򣬻�ȡһ��û����ײ�ķ���
    /// Get a direction without collision by passing in the direction
    /// </summary>
    public bool GetCollisionFreeDirection(Vector3 desiredDirection, out Vector3 outDirection)
    {
        desiredDirection.Normalize();
        outDirection = desiredDirection;

        if (desiredDirection == Vector3.zero) return false;

        Vector3 bestDirection = Vector3.zero;

        // ����BestDirection Calculate Best Direction
        Vector3 bestDirection_right = GetBestDirectionHalf(1, desiredDirection);
        Vector3 bestDirection_left = GetBestDirectionHalf(-1, desiredDirection);

        // ѡ����ѷ��� Choose the best direction
        if (Vector3.Dot(transform.forward, bestDirection_left) > Vector3.Dot(transform.forward, bestDirection_right))
        {
            bestDirection = bestDirection_left;
        }
        else
        {
            bestDirection = bestDirection_right;
        }

        // ����Ƿ��ҵ���Ч����
        if (bestDirection != desiredDirection)
        {
            outDirection = bestDirection;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// ��ȡ������ϰ����� Get the best direction without obstacles
    /// sign == 1: �Ұ�� Right half
    /// sign == -1: ���� Left half
    /// </summary>
    Vector3 GetBestDirectionHalf(int sign, Vector3 desiredDirection)
    {
        // ����BestDirection Calculate Best Direction
        Vector3 result = Vector3.zero;
        for (int i = 0; i < rayCount / 2; i++)
        {
            float angle = sign * (360f / rayCount) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * desiredDirection;

            // ����������� Emit multiple rays
            RaycastHit hit;
            bool collision = Physics.Raycast(transform.position + direction * rayStart, direction, out hit, rayLength, collisionLayers);

            if (collision)
            {
                Debug.DrawRay(transform.position + direction * rayStart, direction * hit.distance, Color.red);
            }
            else // û�з�����ײ No collision
            {
                Debug.DrawRay(transform.position + direction * rayStart, direction * rayLength, Color.green);
                result = direction;
                break;
            }
        }
        return result;
    }
}

using UnityEngine;

// 碰撞检测, 之后避障行为 Collision detection and obstacle avoidance behavior
public class CollisionSensor : MonoBehaviour
{
    public float rayStart = 0.5f; // 射线起始位置 ray start position
    public float rayLength = 10f; // 射线长度 ray length
    public int rayCount = 36; // 射线数量 ray count
    public LayerMask collisionLayers; // 碰撞层 collision layer

    /// <summary>
    /// 通过传入的方向，获取一个没有碰撞的方向
    /// Get a direction without collision by passing in the direction
    /// </summary>
    public bool GetCollisionFreeDirection(Vector3 desiredDirection, out Vector3 outDirection)
    {
        desiredDirection.Normalize();
        outDirection = desiredDirection;

        if (desiredDirection == Vector3.zero) return false;

        Vector3 bestDirection = Vector3.zero;

        // 计算BestDirection Calculate Best Direction
        Vector3 bestDirection_right = GetBestDirectionHalf(1, desiredDirection);
        Vector3 bestDirection_left = GetBestDirectionHalf(-1, desiredDirection);

        // 选择最佳方向 Choose the best direction
        if (Vector3.Dot(transform.forward, bestDirection_left) > Vector3.Dot(transform.forward, bestDirection_right))
        {
            bestDirection = bestDirection_left;
        }
        else
        {
            bestDirection = bestDirection_right;
        }

        // 检查是否找到有效方向
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
    /// 获取最佳无障碍方向 Get the best direction without obstacles
    /// sign == 1: 右半边 Right half
    /// sign == -1: 左半边 Left half
    /// </summary>
    Vector3 GetBestDirectionHalf(int sign, Vector3 desiredDirection)
    {
        // 计算BestDirection Calculate Best Direction
        Vector3 result = Vector3.zero;
        for (int i = 0; i < rayCount / 2; i++)
        {
            float angle = sign * (360f / rayCount) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * desiredDirection;

            // 发射多条射线 Emit multiple rays
            RaycastHit hit;
            bool collision = Physics.Raycast(transform.position + direction * rayStart, direction, out hit, rayLength, collisionLayers);

            if (collision)
            {
                Debug.DrawRay(transform.position + direction * rayStart, direction * hit.distance, Color.red);
            }
            else // 没有发生碰撞 No collision
            {
                Debug.DrawRay(transform.position + direction * rayStart, direction * rayLength, Color.green);
                result = direction;
                break;
            }
        }
        return result;
    }
}

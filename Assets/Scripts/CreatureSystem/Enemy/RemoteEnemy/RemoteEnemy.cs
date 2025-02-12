using AIActor_RC;
using RCProtocol;
using UnityEngine;

public class RemoteEnemy : AIActor
{
    public Vector3 CurrentPos { get; set; }
    public Vector3 TargetPos { get; set; }
    public Vector3 CurrentDir { get; set; }
    public Vector3 TargetDir { get; set; }
    public long LastUpdateTime { get; set; }
    public GameObject go { get; set; }

    // 插值参数
    private float interpolationDuration = 0.1f; // 插值持续时间
    private float interpolationProgress = 0f;

    /// <summary>
    /// 更新玩家状态，并重置插值进度
    /// </summary>
    public void UpdateState(Vector3 newPos, Vector3 newDir, long timestamp)
    {
        CurrentPos = TargetPos;
        CurrentDir = TargetDir;
        TargetPos = newPos;
        TargetDir = newDir;
        LastUpdateTime = timestamp;
        interpolationProgress = 0f;
    }

    /// <summary>
    /// 插值更新位置和方向
    /// </summary>
    public void Interpolate(float deltaTime)
    {
        interpolationProgress += deltaTime;
        float t = Mathf.Clamp01(interpolationProgress / interpolationDuration);
        CurrentPos = Vector3.Lerp(CurrentPos, TargetPos, t);
        CurrentDir = Vector3.Lerp(CurrentDir, TargetDir, t);
    }
}

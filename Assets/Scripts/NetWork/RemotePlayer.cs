using UnityEngine;

/// <summary>
/// 其他玩家类
/// </summary>
public class RemotePlayer
{
    public int RoleID { get; private set; }
    public string Account { get; private set; }
    public Vector3 CurrentPos { get; private set; }
    public Vector3 TargetPos { get; private set; }
    public Vector3 CurrentDir { get; private set; }
    public Vector3 TargetDir { get; private set; }
    public long LastUpdateTime { get; private set; }
    public GameObject GameObject { get; private set; }

    // 插值参数
    private float interpolationDuration = 0.1f; // 插值持续时间
    private float interpolationProgress = 0f;

    public RemotePlayer(int roleID, string account, Vector3 initialPos, Vector3 initialDir, long timestamp, GameObject gameObject)
    {
        RoleID = roleID;
        Account = account;
        CurrentPos = initialPos;
        TargetPos = initialPos;
        CurrentDir = initialDir;
        TargetDir = initialDir;
        LastUpdateTime = timestamp;
        GameObject = gameObject;
    }

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
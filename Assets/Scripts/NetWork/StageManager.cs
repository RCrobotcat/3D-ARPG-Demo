using RCProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    public GameObject rolePrefab;

    AsyncOperation ao;
    private List<NetMsg> pendingInstantiateMsgs = new List<NetMsg>();

    protected override void Awake()
    {
        NetManager.Instance.RegisterNtfHandler(CMD.NtfEnterStage, ntfEnterStage);
        NetManager.Instance.RegisterNtfHandler(CMD.InstantiateRole, InstantiateRole);

        DontDestroyOnLoad(this);

        base.Awake();
    }

    void ntfEnterStage(NetMsg msg)
    {
        ao = SceneManager.LoadSceneAsync(msg.ntfEnterStage.stageName);
        ao.completed += OnSceneLoaded;
    }

    /// <summary>
    /// 生成玩家角色
    /// </summary>
    void InstantiateRole(NetMsg msg)
    {
        if (ao != null && ao.isDone)
        {
            Vector3 pos = new Vector3(msg.instantiateRole.PosX, 0, msg.instantiateRole.PosZ);
            Instantiate(rolePrefab, pos, Quaternion.identity);
        }
        else
        {
            pendingInstantiateMsgs.Add(msg);
        }
    }
    void OnSceneLoaded(AsyncOperation obj)
    {
        foreach (var msg in pendingInstantiateMsgs)
        {
            Vector3 pos = new Vector3(msg.instantiateRole.PosX, 0, msg.instantiateRole.PosZ);
            Instantiate(rolePrefab, pos, Quaternion.identity);
        }
        pendingInstantiateMsgs.Clear();
    }

}

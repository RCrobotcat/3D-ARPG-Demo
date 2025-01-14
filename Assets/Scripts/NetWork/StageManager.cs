using RCProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    public GameObject rolePrefab;

    AsyncOperation ao;
    private List<NetMsg> pendingInstantiateMsgs = new List<NetMsg>();

    readonly Dictionary<int, GameObject> roleID2Entities = new Dictionary<int, GameObject>(); // ������Ϸʵ��

    protected override void Awake()
    {
        NetManager.Instance.StartConnectToGame();

        NetManager.Instance.RegisterNtfHandler(CMD.NtfEnterStage, ntfEnterStage);
        NetManager.Instance.RegisterNtfHandler(CMD.InstantiateRole, InstantiateRole);
        NetManager.Instance.RegisterNtfHandler(CMD.SyncMovePos, SyncMovePos);
        NetManager.Instance.RegisterNtfHandler(CMD.RemoveEntity, RemoveEntity);

        DontDestroyOnLoad(this);

        base.Awake();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    /// <summary>
    /// ���볡��
    /// </summary>
    void ntfEnterStage(NetMsg msg)
    {
        ao = SceneManager.LoadSceneAsync(msg.ntfEnterStage.stageName);
        ao.completed += OnSceneLoaded;
    }

    /// <summary>
    /// ������ҽ�ɫ
    /// </summary>
    void InstantiateRole(NetMsg msg)
    {
        pendingInstantiateMsgs.Add(msg);
    }
    void OnSceneLoaded(AsyncOperation obj)
    {
        foreach (var msg in pendingInstantiateMsgs)
        {
            Vector3 pos = new Vector3(msg.instantiateRole.PosX, 0, msg.instantiateRole.PosZ);
            GameObject go = Instantiate(rolePrefab, pos, Quaternion.identity);
            NetManager.Instance.roldID = msg.instantiateRole.roleID;
            go.GetComponent<Character>().roleID = msg.instantiateRole.roleID;
            if (NetManager.Instance.isGameConnected())
            {
                SendAffirmEnterStage(msg);
            }
        }
        pendingInstantiateMsgs.Clear();
    }

    /// <summary>
    /// ����ȷ�Ͻ��볡����Ϣ
    /// </summary>
    /// <param name="msg"></param>
    void SendAffirmEnterStage(NetMsg msg)
    {
        // ����ȷ�Ͻ��볡����Ϣ
        NetMsg netMsg = new NetMsg
        {
            cmd = CMD.AffirmEnterStage,
            affirmEnterStage = new AffirmEnterStage
            {
                mode = EnterStageMode.Login,
                stageName = SceneManager.GetActiveScene().name,
                roleID = msg.instantiateRole.roleID,
                account = msg.instantiateRole.account,

                playerState = PlayerStateEnum.Online,
                driverEnum = EntityDriverEnum.Client,

                PosX = msg.instantiateRole.PosX,
                PosZ = msg.instantiateRole.PosZ
            }
        };
        NetManager.Instance.SendMsg(netMsg);
    }

    /// <summary>
    /// ����ʵ���Լ����ƶ�λ����Ϣ�ص�
    /// </summary>
    void SyncMovePos(NetMsg msg)
    {
        if (msg.syncMovePos.roleID != NetManager.Instance.roldID)
        {
            if (!roleID2Entities.ContainsKey(msg.syncMovePos.roleID))
            {
                Vector3 pos = new Vector3(msg.syncMovePos.PosX, 0, msg.syncMovePos.PosZ);
                GameObject go = Instantiate(rolePrefab, pos, Quaternion.identity);
                go.GetComponent<Character>().roleID = msg.syncMovePos.roleID;
                roleID2Entities.Add(msg.syncMovePos.roleID, go);
            }
            else
            {
                roleID2Entities[msg.syncMovePos.roleID].transform.position = new Vector3(msg.syncMovePos.PosX, 0, msg.syncMovePos.PosZ);
            }
        }
    }
    /// <summary>
    /// ����ͬ���ƶ�λ����Ϣ
    /// </summary>
    public void SendSyncMovePos(Vector3 targetPos)
    {
        NetMsg netMsg = new NetMsg
        {
            cmd = CMD.SyncMovePos,
            syncMovePos = new SyncMovePos
            {
                roleID = NetManager.Instance.roldID,
                account = NetManager.Instance.account,
                PosX = targetPos.x,
                PosZ = targetPos.z
            }
        };
        NetManager.Instance.SendMsg(netMsg);
    }

    /// <summary>
    /// �Ƴ�ʵ��
    /// </summary>
    void RemoveEntity(NetMsg msg)
    {
        if (roleID2Entities.ContainsKey(msg.removeEntity.roleID))
        {
            Destroy(roleID2Entities[msg.removeEntity.roleID]);
            roleID2Entities.Remove(msg.removeEntity.roleID);
        }
    }
}

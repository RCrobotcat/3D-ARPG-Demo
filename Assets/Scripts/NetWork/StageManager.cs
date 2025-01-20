using RCProtocol;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    public GameObject rolePrefab;
    public GameObject rolePrefab_remote;

    AsyncOperation ao;
    private List<NetMsg> pendingInstantiateMsgs = new List<NetMsg>();

    private Dictionary<int, RemotePlayer> remotePlayers = new Dictionary<int, RemotePlayer>(); // 其他玩家实体

    protected override void Awake()
    {
        NetManager.Instance.StartConnectToGame();

        NetManager.Instance.RegisterNtfHandler(CMD.NtfEnterStage, ntfEnterStage);

        NetManager.Instance.RegisterNtfHandler(CMD.InstantiateRole, InstantiateRole);
        NetManager.Instance.RegisterNtfHandler(CMD.SwitchWeapon, SwitchWeapon);

        NetManager.Instance.RegisterNtfHandler(CMD.SyncMovePos, SyncMovePos);
        NetManager.Instance.RegisterNtfHandler(CMD.SyncAnimationState, SyncAnimationState);

        NetManager.Instance.RegisterNtfHandler(CMD.RemoveEntity, RemoveEntity);

        DontDestroyOnLoad(this);

        base.Awake();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        float deltaTime = Time.deltaTime;
        foreach (var player in remotePlayers.Values)
        {
            player.Interpolate(deltaTime);
            // 更新实体的显示
            UpdatePlayerEntity(player);
        }
    }

    /// <summary>
    /// 进入场景
    /// </summary>
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
        pendingInstantiateMsgs.Add(msg);
    }
    void OnSceneLoaded(AsyncOperation obj)
    {
        foreach (var msg in pendingInstantiateMsgs)
        {
            Vector3 pos = new Vector3(msg.instantiateRole.PosX, 0, msg.instantiateRole.PosZ);
            GameObject go = Instantiate(rolePrefab, pos, Quaternion.identity);
            Character character = go.GetComponent<Character>();
            character.roleID = msg.instantiateRole.roleID;

            // 设置本地玩家的角色ID
            NetManager.Instance.roleID = msg.instantiateRole.roleID;

            if (NetManager.Instance.isGameConnected())
            {
                SendAffirmEnterStage(msg);
            }
        }
        pendingInstantiateMsgs.Clear();
    }
    /// <summary>
    /// 发送确认进入场景消息
    /// </summary>
    /// <param name="msg"></param>
    void SendAffirmEnterStage(NetMsg msg)
    {
        // 发送确认进入场景消息
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
    /// 切换武器
    /// </summary>
    void SwitchWeapon(NetMsg msg)
    {
        if (msg.switchWeapon.roleID == NetManager.Instance.roleID)
            return;

        RemotePlayer role = remotePlayers[msg.switchWeapon.roleID];
        if (msg.switchWeapon.weaponName != "")
        {
            ItemData_SO weapon = GameManager.Instance.GetWeaponByName(msg.switchWeapon.weaponName);
            role.gameObject.GetComponent<Character_remote>().SwitchWeapon(weapon);
        }
        else if (msg.switchWeapon.weaponName == "")
        {
            role.gameObject.GetComponent<Character_remote>().UnEquipWeapon();
        }
    }
    /// <summary>
    /// 发送切换武器消息
    /// </summary>
    public void SendSwitchWeapon(ItemData_SO weapon)
    {
        string weaponName = weapon.itemName;
        NetMsg netMsg = new NetMsg
        {
            cmd = CMD.SwitchWeapon,
            switchWeapon = new SwitchWeapon
            {
                roleID = NetManager.Instance.roleID,
                account = NetManager.Instance.account,
                weaponName = weaponName
            }
        };

        NetManager.Instance.SendMsg(netMsg);
    }
    /// <summary>
    /// 卸下武器
    /// </summary>
    public void SendUnEquipWeapon()
    {
        NetMsg msg = new NetMsg
        {
            cmd = CMD.UnEquipWeapon,
            unEquipWeapon = new UnEquipWeapon
            {
                roleID = NetManager.Instance.roleID,
                account = NetManager.Instance.account
            }
        };
        NetManager.Instance.SendMsg(msg);
    }

    /// <summary>
    /// 所有实体以及其移动位置消息回调
    /// </summary>
    void SyncMovePos(NetMsg msg)
    {
        if (msg.syncMovePos.roleID == NetManager.Instance.roleID)
            return;

        // 处理其他玩家的位置同步
        OnReceiveSyncMovePos(msg.syncMovePos);
    }
    /// <summary>
    /// 接收服务器同步消息
    /// </summary>
    public void OnReceiveSyncMovePos(SyncMovePos syncMovePos)
    {
        if (syncMovePos.roleID == NetManager.Instance.roleID)
            return;

        if (!remotePlayers.ContainsKey(syncMovePos.roleID))
        {
            // 创建新的远程玩家实体
            GameObject go = Instantiate(rolePrefab_remote, new Vector3(syncMovePos.PosX, 0, syncMovePos.PosZ), Quaternion.identity);
            Character_remote character = go.GetComponent<Character_remote>();
            character.roleID = syncMovePos.roleID;

            RemotePlayer newPlayer = new RemotePlayer(
                syncMovePos.roleID,
                syncMovePos.account,
                new Vector3(syncMovePos.PosX, 0, syncMovePos.PosZ),
                new Vector3(syncMovePos.dirX, syncMovePos.dirY, syncMovePos.dirZ),
                syncMovePos.timestamp,
                go
            );
            remotePlayers.Add(syncMovePos.roleID, newPlayer);
        }
        else
        {
            // 更新现有玩家的状态
            RemotePlayer existingPlayer = remotePlayers[syncMovePos.roleID];
            existingPlayer.UpdateState(
                new Vector3(syncMovePos.PosX, existingPlayer.CurrentPos.y, syncMovePos.PosZ),
                new Vector3(syncMovePos.dirX, syncMovePos.dirY, syncMovePos.dirZ),
                syncMovePos.timestamp
            );
        }
    }
    /// <summary>
    /// 更新实体的显示
    /// </summary>
    private void UpdatePlayerEntity(RemotePlayer player)
    {
        if (player.gameObject != null)
        {
            player.gameObject.transform.position = player.CurrentPos;
            player.gameObject.transform.forward = player.CurrentDir;
        }
    }

    /// <summary>
    /// 同步动画状态
    /// </summary>
    void SyncAnimationState(NetMsg msg)
    {
        if (msg.syncAnimationState.roleID == NetManager.Instance.roleID)
            return;

        // 处理其他玩家的动画状态同步
        SyncAnimationState syncAnimationState = msg.syncAnimationState;
        Character_remote character = remotePlayers[syncAnimationState.roleID].gameObject.GetComponent<Character_remote>();
        if (syncAnimationState.animationStateEnum == AnimationStateEnum.Roll)
        {
            character.movementSM.ChangeState(character.remoteRollState);
            character.remoteRollState.IsRoll = true;
        }
        else
        {
            character.movementSM.ChangeState(character.remoteComboState);
            character.remoteComboState.animationState = syncAnimationState.animationStateEnum;
        }
    }

    /// <summary>
    /// 发送同步移动位置消息
    /// </summary>
    public void SendSyncMovePos(Vector3 targetPos, Vector3 targetDir)
    {
        NetMsg netMsg = new NetMsg
        {
            cmd = CMD.SyncMovePos,
            syncMovePos = new SyncMovePos
            {
                roleID = NetManager.Instance.roleID,
                account = NetManager.Instance.account,
                PosX = targetPos.x,
                PosZ = targetPos.z,
                dirX = targetDir.x,
                dirY = targetDir.y,
                dirZ = targetDir.z,
                timestamp = DateTime.UtcNow.Ticks
            }
        };
        NetManager.Instance.SendMsg(netMsg);
    }

    /// <summary>
    /// 发送同步动画状态
    /// </summary>
    public void SendSyncAnimationState(AnimationStateEnum animState)
    {
        NetMsg netMsg = new NetMsg
        {
            cmd = CMD.SyncAnimationState,
            syncAnimationState = new SyncAnimationState
            {
                roleID = NetManager.Instance.roleID,
                account = NetManager.Instance.account,
                animationStateEnum = animState
            }
        };
        NetManager.Instance.SendMsg(netMsg);
    }

    /// <summary>
    /// 移除实体
    /// </summary>
    void RemoveEntity(NetMsg msg)
    {
        if (remotePlayers.ContainsKey(msg.removeEntity.roleID))
        {
            RemotePlayer player = remotePlayers[msg.removeEntity.roleID];
            remotePlayers.Remove(msg.removeEntity.roleID);
            if (player.gameObject != null)
            {
                Destroy(player.gameObject);
            }
        }
    }
}

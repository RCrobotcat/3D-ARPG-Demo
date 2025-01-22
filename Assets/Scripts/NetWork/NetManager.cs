using PEUtils;
using RC_IOCPNet;
using RCProtocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class NetManager : Singleton<NetManager>
{
    IOCPNet<LoginToken, NetMsg> loginNet;
    IOCPNet<GameToken, NetMsg> gameNet;

    public int roleID; // ��ǰ��ҵĽ�ɫID
    public string account; // ��ǰ�˺�

    readonly ConcurrentQueue<NetMsg> netMsgQueue = new();
    readonly Dictionary<CMD, Action<NetMsg>> ntfHandlers = new(); // Notification Handlers (֪ͨ��Ϣ�Ĵ�����)
    readonly Dictionary<CMD, Action<NetMsg>> respHandlers = new(); // Response Handlers (��Ӧ��Ϣ�Ĵ�����)

    protected override void Awake()
    {
        LogConfig cfg = new()
        {
            enableLog = true,
            logPrefix = "",
            enableTime = true,
            logSeparate = ">",
            enableThreadID = true, // Enable thread ID
            enableTrace = true, // Enable stack trace(��ջ����)
            enableSave = true,
            enableCover = true, // Enable overwrite
            saveName = "ARPGClientLog.txt",
            loggerEnum = LoggerType.Unity
        };
        PELog.InitSettings(cfg);

        IOCPTool.LogFunc = PELog.Log;
        IOCPTool.WarnFunc = PELog.Warn;
        IOCPTool.ErrorFunc = PELog.Error;
        IOCPTool.ColorLogFunc = (color, msg) => { PELog.ColorLog((LogColor)color, msg); };

        RegisterNtfHandler(CMD.OnClient2LoginConnected, OnClient2LoginConnected);
        RegisterNtfHandler(CMD.OnClient2LoginDisConnected, OnClient2LoginDisConnected);

        RegisterNtfHandler(CMD.OnClient2GameConnected, OnClient2GameConnected);
        RegisterNtfHandler(CMD.OnClient2GameDisConnected, OnClient2GameDisConnected);

        // this.Log("Init NetManager Done.");

        DontDestroyOnLoad(this);

        base.Awake();
    }

    void Update()
    {
        while (!netMsgQueue.IsEmpty)
        {
            if (netMsgQueue.TryDequeue(out NetMsg msg))
            {
                // this.Log($"[Token] => Received netMsg's Command: {msg.cmd}");
                // �����л�Ӧ����Ϣ
                if (respHandlers.TryGetValue(msg.cmd, out Action<NetMsg> respHandler))
                {
                    respHandler.Invoke(msg);
                    respHandlers.Remove(msg.cmd);
                }
                else // �����޻�Ӧ��Ϣ, ����֪ͨ����Ϣ(������ת֪ͨ��)
                {
                    if (ntfHandlers.TryGetValue(msg.cmd, out Action<NetMsg> ntfHandler))
                    {
                        ntfHandler.Invoke(msg);
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        SendExitMsg();

        ActiveCloseLoginConnection();
        ActiveCloseGameConnection();
    }

    void SendExitMsg()
    {
        NetMsg netMsg = new NetMsg
        {
            cmd = CMD.ExitGame,
            exitGame = new ExitGame
            {
                roleID = roleID,
                account = account
            }
        };
        SendMsg(netMsg);
    }

    public void OnClient2LoginConnected(NetMsg msg)
    {
        this.LogGreen("Connected to Login Server.");
    }
    public void OnClient2LoginDisConnected(NetMsg msg)
    {
        this.LogYellow("Disconnected from Login Server.");
    }

    void OnClient2GameConnected(NetMsg msg)
    {
        this.LogGreen("Connected to Game Server.");
    }
    void OnClient2GameDisConnected(NetMsg msg)
    {
        this.LogYellow("Disconnected from Game Server.");
    }

    /// <summary>
    /// Connect to Login Server.(���ӵ�¼������)
    /// </summary>
    public void StartConnectToLogin()
    {
        loginNet = new IOCPNet<LoginToken, NetMsg>();
        loginNet.StartAsClient("127.0.0.1", 18000);
    }
    /// <summary>
    /// Connect to Game Server.(������Ϸͬ��������)
    /// </summary>
    public void StartConnectToGame()
    {
        gameNet = new IOCPNet<GameToken, NetMsg>();
        gameNet.StartAsClient("127.0.0.1", 19000);
    }
    public bool isGameConnected()
    {
        return gameNet != null && gameNet.token != null && gameNet.token.IsConnected;
    }

    /// <summary>
    /// Active Close Login Connection.(�����رյ�¼����)
    /// </summary>
    public void ActiveCloseLoginConnection()
    {
        loginNet?.ClosetClient();
    }
    /// <summary>
    /// Active Close Game Connection.(�����ر���Ϸ����)
    /// </summary>
    public void ActiveCloseGameConnection()
    {
        gameNet?.ClosetClient();
    }

    public void AddMsgPacks(NetMsg msg)
    {
        netMsgQueue.Enqueue(msg);
    }

    public void RegisterNtfHandler(CMD cmd, Action<NetMsg> handlerCb)
    {
        if (!ntfHandlers.ContainsKey(cmd))
        {
            ntfHandlers.Add(cmd, handlerCb);
        }
    }

    /// <summary>
    /// �޻ذ���Ϣ��callback����Ϊnull
    /// </summary>
    public void SendMsg(NetMsg msg, Action<NetMsg> callback = null)
    {
        // callback��Ϊ��, ˵�����л�Ӧ����Ϣ
        if (callback != null)
        {
            CMD responseCommand = msg.cmd + 1; // CMD.RespAccountLogin
            if (!respHandlers.ContainsKey(responseCommand))
            {
                respHandlers.Add(responseCommand, callback);
            }
            else
            {
                // �Ѿ����͹�����, �ظ�����Ӧ��Ϣ
                this.Warn($"responseCommand of the command: [{msg.cmd}] already exists in response cache!");
            }
        }

        switch (msg.cmd)
        {
            case CMD.ReqRoleToken:
                loginNet?.token?.SendMsg(msg);
                break;
            case CMD.ReqAccountLogin:
                loginNet?.token?.SendMsg(msg);
                break;
            case CMD.ReqRoleEnter:
                loginNet?.token?.SendMsg(msg);
                break;
            case CMD.AffirmEnterStage:
                gameNet?.token?.SendMsg(msg);
                break;
            case CMD.SyncMovePos:
                gameNet?.token?.SendMsg(msg);
                break;
            case CMD.SyncAnimationState:
                gameNet?.token?.SendMsg(msg);
                break;
            case CMD.SwitchWeapon:
                gameNet?.token?.SendMsg(msg);
                break;
            case CMD.UnEquipWeapon:
                gameNet?.token?.SendMsg(msg);
                break;
            case CMD.ExitGame:
                gameNet?.token?.SendMsg(msg);
                break;
            default:
                // battleNet?.token?.SendMsg(msg);
                break;
        }
    }

    /* Tool Funtions */
    public bool IsLoginConnected()
    {
        if (loginNet != null && loginNet.token != null)
        {
            return loginNet.token.IsConnected;
        }
        else return false;
    }
}

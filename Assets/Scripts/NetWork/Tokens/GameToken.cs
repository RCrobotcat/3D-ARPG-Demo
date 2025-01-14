using RC_IOCPNet;
using RCProtocol;

public class GameToken : IOCPToken<NetMsg>
{
    protected override void OnConnected(bool result)
    {
        if (result)
        {
            NetManager.Instance.AddMsgPacks(new NetMsg(CMD.OnClient2GameConnected));
        }
        else
        {
            this.LogRed("Game Connection Failed!");
        }
    }

    protected override void OnDisConnected()
    {
        NetManager.Instance.AddMsgPacks(new NetMsg(CMD.OnClient2GameDisConnected));
    }

    protected override void OnReceiveMsg(NetMsg msg)
    {
        NetManager.Instance.AddMsgPacks(msg);
    }
}
using RC_IOCPNet;
using RCProtocol;

public class LoginToken : IOCPToken<NetMsg>
{
    protected override void OnConnected(bool result)
    {
        if (result)
        {
            NetManager.Instance.AddMsgPacks(new NetMsg(CMD.OnClient2LoginConnected));
        }
        else
        {
            this.LogRed("Login Connection Failed!");
        }
    }

    protected override void OnDisConnected()
    {
        NetManager.Instance.AddMsgPacks(new NetMsg(CMD.OnClient2LoginDisConnected));
    }

    protected override void OnReceiveMsg(NetMsg msg)
    {
        NetManager.Instance.AddMsgPacks(msg);
    }
}

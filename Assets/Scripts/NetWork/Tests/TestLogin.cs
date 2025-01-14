using RCProtocol;
using UnityEngine;
using UnityEngine.UI;

public class TestLogin : MonoBehaviour
{
    public Button btnLogin;
    public InputField playerInput;

    void Awake()
    {
        NetManager.Instance.StartConnectToLogin();

        btnLogin.onClick.AddListener(Login);
    }

    void Login()
    {
        NetMsg loginMsg = new NetMsg()
        {
            cmd = CMD.ReqAccountLogin,
            reqAccountLogin = new ReqAccountLogin()
            {
                account = playerInput.text,
                password = "123456"
            }
        };

        NetManager.Instance.SendMsg(loginMsg, (response) =>
        {
            switch (response.errorCode)
            {
                case ErrorCode.None:
                    this.LogGreen($"Login Successful.");
                    NetManager.Instance.account = loginMsg.reqAccountLogin.account;
                    break;
                case ErrorCode.acct_online_login:
                    this.LogYellow($"�˺��ѵ�¼Login");
                    break;
                case ErrorCode.acct_online_data:
                    this.LogYellow($"�˺��ѵ�¼Data");
                    break;
                case ErrorCode.acct_l2d_offline:
                    this.LogYellow($"��ǰ��������(Offline)����ѡ���������");
                    break;
                default:
                    this.Error($"errorCode:{response.errorCode} δ����");
                    break;
            }
        });
    }
}

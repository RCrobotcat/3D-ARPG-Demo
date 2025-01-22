using RCProtocol;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public Button btnLogin;
    public InputField AccountInput;
    public InputField PasswordInput;

    void Awake()
    {
        NetManager.Instance.StartConnectToLogin();

        btnLogin.onClick.AddListener(LoginBtnEvent);
    }

    void LoginBtnEvent()
    {
        NetMsg loginMsg = new NetMsg()
        {
            cmd = CMD.ReqAccountLogin,
            reqAccountLogin = new ReqAccountLogin()
            {
                account = AccountInput.text,
                password = PasswordInput.text
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
                    this.LogYellow($"账号已登录Login");
                    break;
                case ErrorCode.acct_online_data:
                    this.LogYellow($"账号已登录Data");
                    break;
                case ErrorCode.acct_l2d_offline:
                    this.LogYellow($"当前区服离线(Offline)，请选择其大区。");
                    break;
                default:
                    this.Error($"errorCode:{response.errorCode} 未处理。");
                    break;
            }
        });
    }
}

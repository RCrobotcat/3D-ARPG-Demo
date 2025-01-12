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

    private void OnApplicationQuit()
    {
        NetManager.Instance.ActiveCloseLoginConnection();
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
            if (response.errorCode == ErrorCode.None)
            {
                this.LogGreen("Login Success!");
            }
            else
            {
                this.Warn("Response: " + response.errorCode);
            }
        });
    }
}

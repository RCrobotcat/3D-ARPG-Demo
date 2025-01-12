using RCProtocol;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    protected override void Awake()
    {
        NetManager.Instance.RegisterNtfHandler(CMD.NtfEnterStage, ntfEnterStage);

        base.Awake();
    }

    void ntfEnterStage(NetMsg msg)
    {
        SceneManager.LoadSceneAsync(msg.ntfEnterStage.stageName);
    }
}

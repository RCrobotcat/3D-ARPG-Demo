using Cinemachine;

public class CameraManager : Singleton<CameraManager>
{
    CinemachineFreeLook freeLookCam;
    public CinemachineFreeLook FreeLookCam { get => freeLookCam; }

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (FindObjectOfType<Character>() == null)
            return;

        if (freeLookCam == null)
        {
            freeLookCam = FindObjectOfType<CinemachineFreeLook>();
        }

        if (Character.Instance != null && freeLookCam != null)
        {
            freeLookCam.Follow = Character.Instance.followPoint;
            freeLookCam.LookAt = Character.Instance.lookAtPoint;
        }
    }
}

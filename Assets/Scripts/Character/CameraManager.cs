using Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    CinemachineFreeLook freeLookCam;
    public CinemachineFreeLook FreeLookCam { get => freeLookCam; }

    CinemachineFreeLook targetLockCam;

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
            // freeLookCam = FindObjectOfType<CinemachineFreeLook>();
            freeLookCam = GameObject.Find("FreeLook Camera_player").GetComponent<CinemachineFreeLook>();
        }

        if (targetLockCam == null)
        {
            targetLockCam = GameObject.Find("FreeLook Camera_locking").GetComponent<CinemachineFreeLook>();
        }

        if (Character.Instance != null && freeLookCam != null)
        {
            freeLookCam.Follow = Character.Instance.followPoint;
            freeLookCam.LookAt = Character.Instance.lookAtPoint;
        }

        if (Character.Instance != null && targetLockCam != null)
        {
            targetLockCam.Follow = Character.Instance.followPoint;
            targetLockCam.LookAt = Character.Instance.GetComponent<TargetLockOn>().EnemyTarget_Locator;
        }
    }
}
using AIActor_RC;
using UnityEngine;

public class TargetLockOn : MonoBehaviour
{
    Transform currentTarget;
    // Animator anim;

    [SerializeField] LayerMask targetLayers;
    [SerializeField] Transform enemyTarget_Locator;

    [Tooltip("StateDrivenMethod for Switching Cameras")] [SerializeField]
    Animator cinemachineAnimator;

    [Header("Settings")] [SerializeField] bool zeroVert_Look;
    [SerializeField] float noticeZone = 10f;
    [SerializeField] float lookAtSmoothing = 2;

    [Tooltip("Angle_Degree")] [SerializeField]
    float maxNoticeAngle = 60;

    [SerializeField] float crossHair_Scale = 0.1f;

    Transform cam;
    [HideInInspector] public bool enemyLocked;
    float currentYOffset;
    Vector3 pos;

    CharacterActor characterActor;

    // [SerializeField] CameraFollow camFollow;
    [SerializeField] Transform lockOnCanvas;
    // DefMovement defMovement;

    float rotX, rotY;
    [SerializeField] Vector2 clampAxis = new Vector2(60, 60);
    [SerializeField] float follow_smoothing = 5;
    [SerializeField] float rotate_Smoothing = 5;
    [SerializeField] float senstivity = 60;

    void Start()
    {
        characterActor = GetComponent<CharacterActor>();

        // defMovement = GetComponent<DefMovement>();
        // anim = GetComponent<Animator>();
        cam = Camera.main.transform;
        lockOnCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        // camFollow.lockedTarget = enemyLocked;
        // defMovement.lockMovement = enemyLocked;

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (currentTarget)
            {
                // If there is already a target, Reset.
                ResetTarget();
                return;
            }

            currentTarget = ScanNearBy();
            if (currentTarget)
            {
                FoundTarget();
                characterActor.lockTarget = currentTarget.GetComponent<Actor>();
            }
            else ResetTarget();
        }

        if (enemyLocked)
        {
            if (!TargetOnRange())
                ResetTarget();
            LookAtTarget();
        }
        
        if(Input.GetKeyDown(KeyCode.S))
            ResetTarget();
    }

    void FoundTarget()
    {
        lockOnCanvas.gameObject.SetActive(true);
        // anim.SetLayerWeight(1, 1);
        cinemachineAnimator.Play("TargetCamera");
        enemyLocked = true;

        float h = currentTarget.GetComponent<BoxCollider>().bounds.size.y * currentTarget.localScale.y;
        currentYOffset = h - (h / 2) / 2;
    }

    void ResetTarget()
    {
        lockOnCanvas.gameObject.SetActive(false);
        currentTarget = null;
        currentYOffset = 0;
        enemyLocked = false;
        // anim.SetLayerWeight(1, 0);
        cinemachineAnimator.Play("FollowCamera");
    }

    private Transform ScanNearBy()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle;
        Transform closestTarget = null;
        if (nearbyTargets.Length <= 0) return null;

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            Vector3 dir = nearbyTargets[i].transform.position - cam.position;
            dir.y = 0;
            float _angle = Vector3.Angle(cam.forward, dir);

            if (_angle < closestAngle)
            {
                closestTarget = nearbyTargets[i].transform;
                closestAngle = _angle;
            }
        }

        if (!closestTarget) return null;
        float h1 = closestTarget.GetComponent<BoxCollider>().bounds.size.y;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = half_h;
        if (zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
        Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
        if (Blocked(tarPos)) return null;
        return closestTarget;
    }

    bool Blocked(Vector3 t)
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit))
        {
            if (!hit.transform.CompareTag("Enemy")) return true;
        }

        return false;
    }

    bool TargetOnRange()
    {
        float dis = (transform.position - pos).magnitude;
        if (dis / 2 > noticeZone) return false;
        else return true;
    }

    private void LookAtTarget()
    {
        if (currentTarget == null)
        {
            ResetTarget();
            return;
        }

        pos = currentTarget.position + new Vector3(0, currentYOffset / 4, 0);
        lockOnCanvas.position = pos;
        lockOnCanvas.localScale = Vector3.one * ((cam.position - pos).magnitude * crossHair_Scale);

        enemyTarget_Locator.position = pos;
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing * 2);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noticeZone);
    }
}
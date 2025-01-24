using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CharacterController : Singleton<CharacterController>
{
    [Header("Basics")]
    [HideInInspector] public NavMeshAgent agent;
    public float stopDistance = 0.5f;
    public float runningSpeedRate = 1.5f;

    [Header("Roll")]
    public float rollRange = 5f;
    public float rollTime = 0.3f;
    public float rollStaminaChange = -1.8f;
    [HideInInspector] public bool isRolling = false;

    Animator animator;
    float horizontal, vertical;

    [HideInInspector] public bool isSprint = false;
    float originalSpeed;
    public bool restoringStamina; // 是否正在恢复精力(用尽精力之后)

    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        Cursor.visible = false;

        InputManager.Instance.onRoll += () =>
        {
            if (CharacterNumController.Instance.mModel.PlayerStamina.Value < 1.8f)
                return;
            if (!isRolling && agent.velocity.magnitude > 0.1f)
            {
                StartCoroutine(Roll());
            }
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        /* horizontal = Input.GetAxis("Horizontal");
         vertical = Input.GetAxis("Vertical");*/
        horizontal = InputManager.Instance.inputMove.x;
        vertical = InputManager.Instance.inputMove.y;
        animator.SetFloat("Speed", agent.velocity.magnitude);

        SetAnimationState();

        if (!isRolling)
        {
            Vector3 inputDirection = new Vector3(horizontal, 0, vertical);
            if (inputDirection == Vector3.zero)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                Vector3 CamRelativeMove = ConvertToCameraSpace(inputDirection);
                MovePlayer(CamRelativeMove);
            }

            Running();
        }

        // Rolling();
    }

    /// <summary>
    /// 跑步
    /// </summary>
    void Running()
    {
        if (horizontal == 0 && vertical == 0)
        {
            if (isSprint)
            {
                agent.speed = originalSpeed;
                isSprint = false;
            }
            return;
        }

        if (CharacterNumController.Instance.mModel.PlayerStamina.Value <= 0)
        {
            agent.speed = originalSpeed;
            isSprint = false;
            restoringStamina = true;
        }
        else
        {
            if (!restoringStamina)
            {
                if (InputManager.Instance.inputSprint && !isSprint)
                {
                    originalSpeed = agent.speed;
                    agent.speed *= runningSpeedRate;
                    isSprint = true;
                }
                else if (!InputManager.Instance.inputSprint && isSprint)
                {
                    agent.speed = originalSpeed;
                    isSprint = false;
                }
            }
        }
    }


    void SetAnimationState()
    {
        animator.SetBool("Running", isSprint);
    }

    public void MovePlayer(Vector3 inputDirection)
    {
        Vector3 targetPosition = transform.position + inputDirection;
        MoveToTarget(targetPosition);
    }
    public void MoveToTarget(Vector3 target)
    {
        // StopAllCoroutines();
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }
    private Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 CamForwardZProduct = vectorToRotate.z * camForward;
        Vector3 CamRightXProduct = vectorToRotate.x * camRight;

        Vector3 vectorRotateToCameraSpace = CamForwardZProduct + CamRightXProduct;
        return vectorRotateToCameraSpace;
    }

    /// <summary>
    /// 翻滚
    /// </summary>
    /*void Rolling()
    {
        if (CharacterNumController.Instance.mModel.PlayerStamina.Value < 1.8f)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && !isRolling && agent.velocity.magnitude > 0.1f)
        {
            StartCoroutine(Roll());
        }
    }*/
    IEnumerator Roll()
    {
        isRolling = true;
        agent.isStopped = true;

        animator.SetTrigger("Roll");
        float startTime = Time.time;

        while (Time.time < startTime + rollTime)
        {
            agent.velocity = transform.forward * rollRange;
            yield return null;
        }

        agent.isStopped = false;
        agent.speed = originalSpeed;
        isRolling = false;
        isSprint = false;
    }
    public void RollingStaminaChangeAnimationEvent()
    {
        CharacterNumController.Instance.StaminaChange(rollStaminaChange);
    }

    /// <summary>
    /// 变化精力条接口
    /// </summary>
    public void StaminChange(float val)
    {
        CharacterNumController.Instance.StaminaChange(val);
    }
}

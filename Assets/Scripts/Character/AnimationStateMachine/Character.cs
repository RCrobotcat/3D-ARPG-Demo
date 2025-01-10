using UnityEngine;
using UnityEngine.AI;

public class Character : Singleton<Character>
{
    [Header("Basics")]
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    public float stopDistance = 0.5f;
    public float moveSpeed = 5f;

    [Header("Roll")]
    public float rollRange = 5f;
    public float rollTime = 0.3f;
    public float rollStaminaChange = -1.8f;
    [HideInInspector] public bool isRolling = false;

    [Header("Sprint")]
    public float runSpeed = 7.5f;
    [HideInInspector] public bool isSprint = false;
    [HideInInspector] public bool restoringStamina; // 是否正在恢复精力(用尽精力之后)

    public StateMachine movementSM; // 玩家动作状态机
    public StandingState standingState; // 站立状态
    public SprintState sprintState; // 冲刺状态
    public RollState rollState; // 翻滚状态

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        movementSM = new StateMachine();
        standingState = new StandingState(this, movementSM);
        sprintState = new SprintState(this, movementSM);
        rollState = new RollState(this, movementSM);

        movementSM.Initialize(standingState);
    }

    private void Update()
    {
        movementSM.currentState.HandleInput();

        movementSM.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        movementSM.currentState.PhysicsUpdate();
    }

    #region Animation Events

    public void RollingStaminaChangeAnimationEvent()
    {
        CharacterNumController.Instance.StaminaChange(rollStaminaChange);
    }
    #endregion
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : Singleton<Character>
{
    [Header("Basics")]
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    public float stopDistance = 0.5f;
    public float moveSpeed = 5f;
    public Transform lookAtPoint;
    public Transform followPoint;

    [Header("Roll")]
    public float rollRange = 5f;
    public float rollTime = 0.3f;
    public float rollStaminaChange = -1.5f;
    [HideInInspector] public bool isRolling = false;

    [Header("Sprint")]
    public float runSpeed = 7.5f;
    [HideInInspector] public bool isSprint = false;
    [HideInInspector] public bool restoringStamina; // �Ƿ����ڻָ�����(�þ�����֮��)

    [Header("Attack")]
    public List<AttackSO> combo; // �������
    public float attackStaminaChange = -1.0f;

    public StateMachine movementSM; // ��Ҷ���״̬��
    public StandingState standingState; // վ��״̬
    public SprintState sprintState; // ���״̬
    public RollState rollState; // ����״̬
    public ComboState comboState; // ����״̬

    [HideInInspector] public int roleID; // ��ɫID

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        movementSM = new StateMachine();
        standingState = new StandingState(this, movementSM);
        sprintState = new SprintState(this, movementSM);
        rollState = new RollState(this, movementSM);
        comboState = new ComboState(this, movementSM);

        movementSM.Initialize(standingState);
    }

    private void Update()
    {
        if (roleID != NetManager.Instance.roldID)
            return;
        movementSM.currentState.HandleInput();

        movementSM.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        if (roleID != NetManager.Instance.roldID)
            return;
        movementSM.currentState.PhysicsUpdate();
    }

    #region Animation Events

    public void RollingStaminaChangeAnimationEvent()
    {
        if (roleID != NetManager.Instance.roldID)
            return;
        CharacterNumController.Instance.StaminaChange(rollStaminaChange);
    }

    public void SlashingStaminaChangeAnimationEvent()
    {
        if (roleID != NetManager.Instance.roldID)
            return;

        if (CharacterNumController.Instance.mModel.PlayerStamina.Value < 1.0f)
            return;

        CharacterNumController.Instance.StaminaChange(attackStaminaChange);
    }
    #endregion
}
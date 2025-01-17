using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character_remote : MonoBehaviour
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

    [Header("Sprint")]
    public float runSpeed = 7.5f;
    [HideInInspector] public bool isSprint = false;
    [HideInInspector] public bool restoringStamina; // �Ƿ����ڻָ�����(�þ�����֮��)

    [Header("Attack")]
    public List<AttackSO> combo; // �������
    public float attackStaminaChange = -1.0f;

    public StateMachine_remote movementSM; // ��Ҷ���״̬��
    public RemoteStandingState remoteStandingState;
    public RemoteComboState remoteComboState;
    public RemoteRollState remoteRollState;

    [HideInInspector] public int roleID; // ��ɫID

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        movementSM = new StateMachine_remote();

        remoteStandingState = new RemoteStandingState(this, movementSM);
        remoteComboState = new RemoteComboState(this, movementSM);
        remoteRollState = new RemoteRollState(this, movementSM);

        movementSM.Initialize(remoteStandingState);
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
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
}
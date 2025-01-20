using RCProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

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
    [HideInInspector] public List<AttackSO> combo; // �������
    [HideInInspector] public AttackSO currentCombo; // ��ǰ����
    public List<AttackSO> originalCombo; // ԭʼ�������
    public float attackStaminaChange = -1.0f;

    [Header("Weapon & Armor")]
    public Transform weaponTrans;
    public Transform vfxTrans_left; // ��Чλ��
    public Transform vfxTrans_right; // ��Чλ��
    public Transform armorTrans;

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

    #region Animation Events
    public void RollingStaminaChangeAnimationEvent()
    {
        if (roleID != NetManager.Instance.roleID)
            return;
        CharacterNumController.Instance.StaminaChange(rollStaminaChange);
    }

    public void SlashingStaminaChangeAnimationEvent()
    {
        if (roleID != NetManager.Instance.roleID)
            return;

        if (CharacterNumController.Instance.mModel.PlayerStamina.Value < 1.0f)
            return;

        CharacterNumController.Instance.StaminaChange(attackStaminaChange);
    }

    public void SendRemoteAnimationState()
    {
        ComboStateEnum comboStateEnum = comboState.GetComboState();

        switch (comboStateEnum)
        {
            case ComboStateEnum.Combo_1:
                StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.Combo_1);
                break;
            case ComboStateEnum.Combo_2:
                StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.Combo_2);
                break;
            case ComboStateEnum.Combo_3:
                StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.Combo_3);
                break;
            case ComboStateEnum.Combo_4:
                StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.Combo_4);
                break;
            case ComboStateEnum.Combo_5:
                StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.Combo_5);
                break;
            default:
                break;
        }
    }

    public void PlayVFX_1()
    {
        List<VisualEffect> effects = currentCombo.attackVFXs;

        if (weaponTrans.childCount >= 1)
        {
            if (effects.Count == 0)
            {
                return;
            }
            else if (effects.Count >= 1)
            {
                if (currentCombo.vfxType == VFXType.Left)
                {
                    effects[0].Spawn(vfxTrans_left, vfxTrans_left.position, effects[0].transform.rotation);
                }
                else if (currentCombo.vfxType == VFXType.Right)
                {
                    effects[0].Spawn(vfxTrans_right, vfxTrans_right.position, effects[0].transform.rotation);
                }
            }
        }
    }
    public void PlayVFX_2()
    {
        List<VisualEffect> effects = currentCombo.attackVFXs;

        if (weaponTrans.childCount >= 1)
        {
            if (effects.Count <= 1)
            {
                return;
            }
            else if (effects.Count >= 2)
            {
                effects[1].Spawn(vfxTrans_right, vfxTrans_right.position, effects[1].transform.rotation);
            }
        }
    }
    #endregion
}
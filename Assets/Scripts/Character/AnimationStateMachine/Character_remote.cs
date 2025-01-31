using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

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
    [HideInInspector] public List<AttackSO> combo; // �������
    [HideInInspector] public AttackSO currentCombo; // ��ǰ����
    public List<AttackSO> originalCombo; // ԭʼ�������
    public float attackStaminaChange = -1.0f;

    [Header("Weapon & Armor")]
    public Transform weaponTrans;
    public Transform vfxTrans_left; // ��Чλ��
    public Transform vfxTrans_right; // ��Чλ��
    public Transform armorTrans;

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

        combo = originalCombo;
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

    #region Tool Functions
    public void SwitchWeapon(ItemData_SO weapon)
    {
        Instantiate(weapon.WeaponPrefab, weaponTrans);

        combo = weapon.weaponAttackCombo;
    }
    public void UnEquipWeapon()
    {
        foreach (Transform child in weaponTrans)
        {
            Destroy(child.gameObject);
        }

        combo = originalCombo;
    }
    #endregion

    #region Animation Events
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
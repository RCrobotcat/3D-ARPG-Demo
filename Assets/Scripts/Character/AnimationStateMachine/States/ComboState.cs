using RCProtocol;
using UnityEngine;

public enum ComboStateEnum
{
    Combo_1,
    Combo_2,
    Combo_3,
    Combo_4,
    Combo_5,
}

public class ComboState : State
{
    ComboStateEnum comboStateEnum = ComboStateEnum.Combo_1;

    float lastClickedTime;
    float lastComboEnd;
    int comboCounter; // 连击计数器

    bool inputLeftKey = false;
    bool openInventory = false;

    bool isDead = false;
    public bool IsDead { get => isDead; set => isDead = value; }

    public ComboState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        comboCounter = 0;
        inputLeftKey = false;
        openInventory = false;
        isDead = false;

        comboStateEnum = ComboStateEnum.Combo_1;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        inputLeftKey = InputManager.Instance.inputSlash;
        openInventory = InputManager.Instance.inputOpenInventory;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isDead) return;

        if (inputLeftKey && !openInventory)
        {
            Attack();
        }

        if (openInventory)
        {
            StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.None);
            EndCombo();
            stateMachine.ChangeState(character.standingState);
        }

        ExitAttack();

        if (character.isDead)
        {
            isDead = true;
            character.animator.SetTrigger("Dead");
        }
        else if (!character.isDead)
        {
            isDead = false;
        }
    }

    public override void Exit()
    {
        base.Exit();

        inputLeftKey = false;
        openInventory = false;
        StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.None);
        EndCombo();

        character.isSprint = false;
        character.sprintState.Sprint = false;

        character.agent.isStopped = false;
        character.animator.SetBool("Running", false);

        character.isAttacking = false;
    }

    #region Tool Functions
    /// <summary>
    /// 攻击
    /// </summary>
    void Attack()
    {
        if (Time.time - lastComboEnd > 0.5f && comboCounter < character.combo.Count && CharacterNumController.Instance.mModel.PlayerStamina.Value >= 1f)
        {
            InputManager.Instance.inputSlash = false;
            character.CancelInvoke("EndCombo");

            if (Time.time - lastClickedTime >= 0.55f)
            {
                // 设置当前使用的动画控制器
                character.animator.runtimeAnimatorController = character.combo[comboCounter].animatorOV;
                character.currentCombo = character.combo[comboCounter];
                comboStateEnum = (ComboStateEnum)comboCounter;

                character.animator.Play("NormalAttack", 0, 0);
                character.isAttacking = true;

                // TODO: damage, vfx, etc.

                comboCounter++;
                lastClickedTime = Time.time;

                if (comboCounter >= character.combo.Count)
                {
                    comboCounter = 0;
                    character.isAttacking = false;
                }
            }
        }

        if (CharacterNumController.Instance.mModel.PlayerStamina.Value < 1f)
        {
            // character.Invoke("EndCombo", 0.8f);
            StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.None);
            EndCombo();
            stateMachine.ChangeState(character.standingState);
        }
    }


    /// <summary>
    /// 退出攻击
    /// </summary>
    void ExitAttack()
    {
        if (character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f
            && character.animator.GetCurrentAnimatorStateInfo(0).IsTag("NormalAttack"))
        {
            // character.Invoke("EndCombo", 0.8f);
            StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.None);
            EndCombo();
            stateMachine.ChangeState(character.standingState);
        }
    }

    /// <summary>
    /// 退出连击
    /// </summary>
    void EndCombo()
    {
        character.isAttacking = false;
        comboCounter = 0;
        lastComboEnd = Time.time;
    }

    /// <summary>
    /// 获取连击计数器
    /// </summary>
    public ComboStateEnum GetComboState()
    {
        return comboStateEnum;
    }
    #endregion
}

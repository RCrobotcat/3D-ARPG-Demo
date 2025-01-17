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
    int comboCounter; // ����������

    bool inputLeftKey = false;

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

        comboStateEnum = ComboStateEnum.Combo_1;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        inputLeftKey = InputManager.Instance.inputSlash;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (inputLeftKey)
        {
            Attack();
        }

        ExitAttack();
    }

    public override void Exit()
    {
        base.Exit();

        inputLeftKey = false;
        StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.None);
        EndCombo();

        character.isSprint = false;
        character.sprintState.Sprint = false;

        character.agent.isStopped = false;
        character.animator.SetBool("Running", false);
    }

    #region Tool Functions
    /// <summary>
    /// ����
    /// </summary>
    void Attack()
    {
        if (Time.time - lastComboEnd > 0.5f && comboCounter < character.combo.Count && CharacterNumController.Instance.mModel.PlayerStamina.Value >= 1f)
        {
            InputManager.Instance.inputSlash = false;
            character.CancelInvoke("EndCombo");

            if (Time.time - lastClickedTime >= 0.55f)
            {
                // ���õ�ǰʹ�õĶ���������
                character.animator.runtimeAnimatorController = character.combo[comboCounter].animatorOV;
                comboStateEnum = (ComboStateEnum)comboCounter;

                character.animator.Play("NormalAttack", 0, 0);

                // TODO: damage, vfx, etc.

                comboCounter++;
                lastClickedTime = Time.time;

                if (comboCounter >= character.combo.Count)
                {
                    comboCounter = 0;
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
    /// �˳�����
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
    /// �˳�����
    /// </summary>
    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }

    /// <summary>
    /// ��ȡ����������
    /// </summary>
    public ComboStateEnum GetComboState()
    {
        return comboStateEnum;
    }
    #endregion
}

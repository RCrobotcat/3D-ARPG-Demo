using RCProtocol;

public class RemoteComboState : State_remote
{
    public AnimationStateEnum animationState = AnimationStateEnum.None;
    private AnimationStateEnum lastAnimationState = AnimationStateEnum.None; // 上一个动画状态

    public RemoteComboState(Character_remote _character, StateMachine_remote _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        animationState = AnimationStateEnum.None;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 如果动画状态发生变化，才进行攻击处理
        if (animationState != lastAnimationState)
        {
            Attack();
            lastAnimationState = animationState; // 更新当前的动画状态
        }

        if (animationState == AnimationStateEnum.None && lastAnimationState == AnimationStateEnum.None)
        {
            stateMachine.ChangeState(character.remoteStandingState);
        }

        ExitAttack();
    }

    public override void Exit()
    {
        base.Exit();

        character.animator.runtimeAnimatorController = character.combo[0].animatorOV;
        animationState = AnimationStateEnum.None;
        lastAnimationState = AnimationStateEnum.None;
    }

    #region Tool Functions
    /// <summary>
    /// 攻击
    /// </summary>
    void Attack()
    {
        switch (animationState)
        {
            case AnimationStateEnum.None:
                stateMachine.ChangeState(character.remoteStandingState);
                break;
            case AnimationStateEnum.Combo_1:
                character.animator.runtimeAnimatorController = character.combo[0].animatorOV;
                character.currentCombo = character.combo[0];
                character.animator.Play("NormalAttack", 0, 0);
                break;
            case AnimationStateEnum.Combo_2:
                character.animator.runtimeAnimatorController = character.combo[1].animatorOV;
                character.currentCombo = character.combo[1];
                character.animator.Play("NormalAttack", 0, 0);
                break;
            case AnimationStateEnum.Combo_3:
                character.animator.runtimeAnimatorController = character.combo[2].animatorOV;
                character.currentCombo = character.combo[2];
                character.animator.Play("NormalAttack", 0, 0);
                break;
            case AnimationStateEnum.Combo_4:
                character.animator.runtimeAnimatorController = character.combo[3].animatorOV;
                character.currentCombo = character.combo[3];
                character.animator.Play("NormalAttack", 0, 0);
                break;
            case AnimationStateEnum.Combo_5:
                character.animator.runtimeAnimatorController = character.combo[4].animatorOV;
                character.currentCombo = character.combo[4];
                character.animator.Play("NormalAttack", 0, 0);
                break;
            default:
                break;
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
            stateMachine.ChangeState(character.remoteStandingState);
        }
    }
    #endregion
}

public class RemoteRollState : State_remote
{
    bool isRoll = false;
    public bool IsRoll { get => isRoll; set => isRoll = value; }

    public RemoteRollState(Character_remote _character, StateMachine_remote _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        isRoll = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (IsRoll)
        {
            Rolling();
        }
    }

    #region Tool Functions
    public void Rolling()
    {
        character.animator.SetTrigger("Roll");
        isRoll = false;
    }
    #endregion
}


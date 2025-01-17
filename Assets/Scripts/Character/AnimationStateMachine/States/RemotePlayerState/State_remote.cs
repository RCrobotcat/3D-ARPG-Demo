using UnityEngine;

public class State_remote
{
    public Character_remote character;
    public StateMachine_remote stateMachine;

    protected Vector3 inputDirection;
    protected Vector2 input; // ÕÊº“ ‰»Î

    public State_remote(Character_remote _character, StateMachine_remote _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public virtual void Enter()
    {
        //StateUI.instance.SetStateText(this.ToString());
        Debug.Log("Enter State: " + this.ToString());
    }

    public virtual void HandleInput() { }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void Exit() { }
}
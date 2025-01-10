using UnityEngine;

public class State
{
    public Character character;
    public StateMachine stateMachine;

    protected Vector3 inputDirection;
    protected Vector2 input; // ÕÊº“ ‰»Î


    public State(Character _character, StateMachine _stateMachine)
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
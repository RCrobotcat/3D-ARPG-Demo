using UnityEngine;

public class RemoteStandingState : State_remote
{
    Vector3 lastPos; // 上一帧的位置

    public RemoteStandingState(Character_remote _character, StateMachine_remote _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        input = Vector2.zero;

        character.agent.speed = character.moveSpeed;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (lastPos != character.transform.position)
        {
            character.animator.SetFloat("Speed", 1.0f);
            lastPos = character.transform.position;
        }
        else
        {
            character.animator.SetFloat("Speed", 0.0f);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}

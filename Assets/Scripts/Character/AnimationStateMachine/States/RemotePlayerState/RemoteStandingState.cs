using UnityEngine;
using UnityEngine.AI;

public class RemoteStandingState : State
{
    bool sprint;
    bool rolling;
    bool slash;

    Vector3 lastPos; // 上一帧的位置

    public RemoteStandingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        sprint = false;
        rolling = false;

        input = Vector2.zero;

        character.agent.speed = character.moveSpeed;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // character.animator.SetFloat("Speed", character.agent.velocity.magnitude);

        if (sprint)
        {
            stateMachine.ChangeState(character.sprintState);
        }
        if (rolling)
        {
            slash = false;
            stateMachine.ChangeState(character.rollState);
        }

        if (slash && !rolling)
        {
            character.agent.isStopped = true;
            stateMachine.ChangeState(character.comboState);
        }
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
        slash = false;
    }
}

using UnityEngine;

public class StandingState : State
{
    bool sprint;
    bool rolling;
    bool slash;

    public StandingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
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

    public override void HandleInput()
    {
        base.HandleInput();

        input = InputManager.Instance.inputMove;
        inputDirection = new Vector3(input.x, 0, input.y);

        if (InputManager.Instance.inputSprint && character.agent.velocity.magnitude > 0.1f)
        {
            sprint = true;
        }

        if (InputManager.Instance.inputRoll && character.agent.velocity.magnitude > 0.1f)
        {
            rolling = true;
        }

        slash = InputManager.Instance.inputSlash;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        character.animator.SetFloat("Speed", character.agent.velocity.magnitude);

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

        if (inputDirection == Vector3.zero)
        {
            character.agent.isStopped = true;
        }
        else
        {
            character.agent.isStopped = false;
            Vector3 CamRelativeMove = ConvertToCameraSpace(inputDirection);
            MovePlayer(CamRelativeMove);
        }
    }
    public override void Exit()
    {
        base.Exit();
        slash = false;
    }

    #region Tool Functions
    public void MovePlayer(Vector3 inputDirection)
    {
        if (character.agent.isStopped)
        {
            character.agent.isStopped = false;
        }
        Vector3 targetPosition = character.transform.position + inputDirection;

        Vector3 direction = (targetPosition - character.transform.position).normalized;
        StageManager.Instance.SendSyncMovePos(character.transform.position, direction);

        MoveToTarget(targetPosition);
    }
    public void MoveToTarget(Vector3 target)
    {
        // StopAllCoroutines();
        character.agent.stoppingDistance = character.stopDistance;
        character.agent.isStopped = false;
        character.agent.destination = target;
    }
    private Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 CamForwardZProduct = vectorToRotate.z * camForward;
        Vector3 CamRightXProduct = vectorToRotate.x * camRight;

        Vector3 vectorRotateToCameraSpace = CamForwardZProduct + CamRightXProduct;
        return vectorRotateToCameraSpace;
    }
    #endregion
}
using UnityEngine;

public class SprintState : State
{
    bool sprint;
    bool rolling;

    public SprintState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        sprint = false;
        input = Vector2.zero;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        input = InputManager.Instance.inputMove;
        inputDirection = new Vector3(input.x, 0, input.y);

        rolling = InputManager.Instance.inputRoll;

        if (input == Vector2.zero)
        {
            sprint = false;
            character.isSprint = false;
        }

        if (CharacterNumController.Instance.mModel.PlayerStamina.Value <= 0)
        {
            sprint = false;
            character.isSprint = false;
            character.restoringStamina = true;
        }
        else
        {
            if (!character.restoringStamina)
            {
                if (InputManager.Instance.inputSprint && !sprint)
                {
                    sprint = true;
                    character.isSprint = true;
                }
                else if (!InputManager.Instance.inputSprint && sprint)
                {
                    sprint = false;
                    character.isSprint = false;
                }
            }
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (sprint)
        {
            character.animator.SetBool("Running", true);
        }
        else
        {
            character.animator.SetBool("Running", false);
            stateMachine.ChangeState(character.standingState);
        }

        if (rolling)
        {
            stateMachine.ChangeState(character.rollState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (sprint)
        {
            character.agent.speed = character.runSpeed;
        }
        else if (!sprint)
        {
            character.agent.speed = character.moveSpeed;
        }

        if (inputDirection == Vector3.zero)
        {
            character.agent.isStopped = true;
        }
        else if (inputDirection != Vector3.zero)
        {
            character.agent.isStopped = false;
            Vector3 CamRelativeMove = ConvertToCameraSpace(inputDirection);
            MovePlayer(CamRelativeMove);
        }
    }

    public override void Exit()
    {
        base.Exit();
        sprint = false;
        character.isSprint = false;
    }

    #region Tool Functions
    public void MovePlayer(Vector3 inputDirection)
    {
        Vector3 targetPosition = character.transform.position + inputDirection;
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
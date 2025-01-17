using System.Collections;
using UnityEngine;
using RCProtocol;

public class RollState : State
{
    bool rolling;

    public RollState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        rolling = false;
        input = Vector2.zero;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        input = InputManager.Instance.inputMove;
        inputDirection = new Vector3(input.x, 0, input.y);

        rolling = InputManager.Instance.inputRoll && character.agent.velocity.magnitude > 0.1f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!rolling && !InputManager.Instance.inputSlash)
        {
            stateMachine.ChangeState(character.standingState);
        }
        else
        {
            Rolling();
        }
    }

    public override void Exit()
    {
        base.Exit();

        rolling = false;
    }

    #region Tool Functions
    public void Rolling()
    {
        if (CharacterNumController.Instance.mModel.PlayerStamina.Value < 1.8f)
            return;
        if (!character.isRolling && character.agent.velocity.magnitude > 0.1f)
        {
            character.StartCoroutine(Roll());
            StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.Roll);
        }
    }
    IEnumerator Roll()
    {
        character.isRolling = true;
        character.agent.isStopped = true;

        character.animator.SetTrigger("Roll");
        float startTime = Time.time;

        while (Time.time < startTime + character.rollTime)
        {
            character.agent.velocity = character.transform.forward * character.rollRange;
            yield return null;
        }

        character.agent.isStopped = false;
        character.agent.speed = character.moveSpeed;
        character.isRolling = false;
        rolling = false;
        StageManager.Instance.SendSyncAnimationState(AnimationStateEnum.None);
        character.isSprint = false;
    }
    #endregion
}

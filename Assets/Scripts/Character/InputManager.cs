using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InputManager : Singleton<InputManager>
{
#if ENABLE_INPUT_SYSTEM
    public PlayerInput playerInput;
#endif

    public Vector2 inputMove = Vector2.zero;
    public bool inputSprint = false;

    public bool inputRoll = false;
    public Action onRoll;

    public bool inputSlash = false;
    public Action onSlash;

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputAction.CallbackContext value)
    {
        // �洢�����ֵ(����ΪWASD�����ֵ)
        MoveInput(value.ReadValue<Vector2>());
    }

    public void OnSrpint(InputAction.CallbackContext value)
    {
        // �����Ǽ����shift�Ƿ���
        SprintInput(value.performed);
    }

    public void OnRoll(InputAction.CallbackContext value)
    {
        // �����Ǽ��space�Ƿ���
        if (value.performed)
        {
            RollInput(true);
            onRoll?.Invoke();
        }
        else if (value.canceled)
        {
            RollInput(false);
        }
    }

    public void OnSlash(InputAction.CallbackContext value)
    {
        // �����Ǽ���������Ƿ���
        if (value.performed)
        {
            SlashInput(true);
            onSlash?.Invoke();
        }
        else if (value.canceled)
        {
            SlashInput(false);
        }
    }
#endif

    public void MoveInput(Vector2 newMoveDirection)
    {
        inputMove = newMoveDirection;
    }

    public void SprintInput(bool newSprint)
    {
        inputSprint = newSprint;
    }

    public void RollInput(bool newRoll)
    {
        inputRoll = newRoll;
    }

    public void SlashInput(bool newSlash)
    {
        inputSlash = newSlash;
    }
}

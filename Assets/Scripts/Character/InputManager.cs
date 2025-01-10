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
        // 存储输入的值(这里为WASD输入的值)
        MoveInput(value.ReadValue<Vector2>());
    }

    public void OnSrpint(InputAction.CallbackContext value)
    {
        // 这里是检测左shift是否按下
        SprintInput(value.performed);
    }

    public void OnRoll(InputAction.CallbackContext value)
    {
        // 这里是检测space是否按下
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
        // 这里是检测鼠标左键是否按下
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

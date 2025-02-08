using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
#if ENABLE_INPUT_SYSTEM
    public PlayerInput playerInput;
#endif

    public Vector2 inputMove = Vector2.zero;
    public bool inputSprint = false;

    public bool inputRoll = false;
    public Action onRoll;

    public bool inputSlash;
    public Action onSlash;

    public bool inputOpenInventory;

    public bool inputOpenQuest;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
    }


#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputAction.CallbackContext value)
    {
        MoveInput(value.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext value)
    {
        SprintInput(value.performed);
    }

    public void OnRoll(InputAction.CallbackContext value)
    {
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
        if (value.performed)
        {
            inputSlash = true;
            onSlash?.Invoke();
        }
        else
        {
            inputSlash = false;
        }
    }

    public void OnOpenInventory(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            OpenInventoryInput();
        }
    }

    public void OnOpenQuest(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            OpenQuestInput();
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
        inputSlash = false;
        inputRoll = newRoll;
    }

    public void OpenInventoryInput()
    {
        inputOpenInventory = !inputOpenInventory;
    }

    public void OpenQuestInput()
    {
        inputOpenQuest = !inputOpenQuest;
    }
}

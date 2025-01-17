public class StateMachine_remote
{
    public State_remote currentState;

    public void Initialize(State_remote startingState)
    {
        currentState = startingState;
        startingState.Enter();
    }

    public void ChangeState(State_remote newState)
    {
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }
}
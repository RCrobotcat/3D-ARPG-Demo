using QFramework;

public class PlayerStaminaChangeCommand : AbstractCommand
{
    float _staminaChange;

    public PlayerStaminaChangeCommand(float staminaChange)
    {
        _staminaChange = staminaChange;
    }

    protected override void OnExecute()
    {
        var characterNumModel = this.GetModel<ICharacterNumModel>();
        characterNumModel.PlayerStaminaChange(_staminaChange);
    }
}
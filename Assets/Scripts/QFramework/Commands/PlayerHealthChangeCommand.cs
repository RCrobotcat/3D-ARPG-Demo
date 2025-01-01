using QFramework;

public class PlayerHealthChangeCommand : AbstractCommand
{
    float _healthChange;

    public PlayerHealthChangeCommand(float healthChange)
    {
        _healthChange = healthChange;
    }

    protected override void OnExecute()
    {
        var characterNumModel = this.GetModel<ICharacterNumModel>();
        characterNumModel.PlayerHealthChange(_healthChange);
    }
}
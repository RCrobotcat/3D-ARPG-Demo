using QFramework;

public interface ICharacterNumSystem : ISystem { }

public class CharacterNumSystem : AbstractSystem, ICharacterNumSystem
{
    protected override void OnInit()
    {
        var mModel = this.GetModel<ICharacterNumModel>();

        mModel.PlayerStamina.Register(stamina =>
        {
            if (CharacterController.Instance != null)
            {
                if (CharacterController.Instance.restoringStamina && stamina >= 1.5f)
                {
                    CharacterController.Instance.restoringStamina = false;
                }
            }
        });
    }
}
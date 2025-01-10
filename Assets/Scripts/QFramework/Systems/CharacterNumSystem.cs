using QFramework;

public interface ICharacterNumSystem : ISystem { }

public class CharacterNumSystem : AbstractSystem, ICharacterNumSystem
{
    protected override void OnInit()
    {
        var mModel = this.GetModel<ICharacterNumModel>();

        mModel.PlayerStamina.Register(stamina =>
        {
            if (Character.Instance != null)
            {
                if (Character.Instance.restoringStamina && stamina >= 1.5f)
                {
                    Character.Instance.restoringStamina = false;
                }
            }
        });
    }
}
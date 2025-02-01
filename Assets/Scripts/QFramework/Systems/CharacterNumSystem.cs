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

        mModel.PlayerHealth.Register(health =>
        {
            if (Character.Instance != null)
            {
                if (health <= 0 && !Character.Instance.isDead)
                {
                    GameOver.Instance.OpenRestartPanel();
                    Character.Instance.isDead = true;
                }
                else
                {
                    Character.Instance.isDead = false;
                }
            }
        });
    }
}
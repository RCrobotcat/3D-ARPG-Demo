using QFramework;

public class CharacterApp : Architecture<CharacterApp>
{
    protected override void Init()
    {
        this.RegisterModel<ICharacterNumModel>(new CharacterNumModel());
        this.RegisterSystem<ICharacterNumSystem>(new CharacterNumSystem());
        this.RegisterUtility<Istorage>(new storage());
    }
}

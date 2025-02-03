using AIActor_RC;

public enum DrivenType
{
    Player,
    RemotePlayer,
}

public class CharacterActor : Actor
{
    int roleID; // 角色ID
    public int RoleID { get => roleID; }
    public DrivenType drivenType; // 驱动类型

    public Actor attackTarget; // 攻击目标

    protected override void Start()
    {
        base.Start();

        if (drivenType == DrivenType.Player)
        {
            roleID = GetComponent<Character>().roleID;
        }
        else if (drivenType == DrivenType.RemotePlayer)
        {
            roleID = GetComponent<Character_remote>().roleID;
        }
    }
}

using System.Collections.Generic;
using AIActor_RC;
using UnityEngine;

public enum DrivenType
{
    Player,
    RemotePlayer,
}

public class CharacterActor : Actor
{
    int roleID; // 角色ID

    public int RoleID
    {
        get => roleID;
    }

    public DrivenType drivenType; // 驱动类型

    public Actor lockTarget; // 锁定的目标

    public float viewRadius = 20f; // 视野半径 View radius

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

    public Actor GetNearestAttackPlayerTargetInView()
    {
        ActorTypeFilter filter = (actor) => actor is AIActor;

        List<Actor> actors = GetActorsInView(filter);

        if (actors.Count == 0) return null;

        actors.Sort((actorA, actorB) =>
        {
            float distanceA = Vector3.Distance(transform.position, actorA.transform.position);
            float distanceB = Vector3.Distance(transform.position, actorB.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        return actors[0];
    }

    public List<Actor> GetActorsInView(ActorTypeFilter filter = null)
    {
        if (ActorManager.Instance != null)
        {
            return ActorManager.Instance.GetActorsWithinRange(this, transform.position, viewRadius, filter);
        }

        return new List<Actor>();
    }
}
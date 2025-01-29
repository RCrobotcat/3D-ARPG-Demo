using RCProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AIActor_RC
{
    /// <summary>
    /// Actor类型过滤器
    /// Actor type filter
    /// </summary>
    public delegate bool ActorTypeFilter(Actor actor);

    public class ActorManager : Singleton<ActorManager>
    {
        List<Actor> actors = new List<Actor>();

        public List<GameObject> monster_all = new List<GameObject>(); // 当前场景中的怪物列表
        public List<GameObject> monsters_all_remote = new List<GameObject>(); // 当前场景中的怪物(远程)列表

        Dictionary<int, RemoteEnemy> monsters_remote = new Dictionary<int, RemoteEnemy>(); // 远程怪物列表

        protected override void Awake()
        {
            base.Awake();

            NetManager.Instance.RegisterNtfHandler(CMD.SyncMonsterMovePos, SyncMonsterMovePos);

            NetManager.Instance.RegisterNtfHandler(CMD.CreateMonsters, CreateMonsters); // 第一次进入游戏的玩家创建怪物

            DontDestroyOnLoad(this);
        }

        void Update()
        {
            float deltaTime = Time.deltaTime;
            foreach (var monster in monsters_remote.Values)
            {
                monster.Interpolate(deltaTime);
                // 更新实体的显示
                UpdateMonsterEntity(monster);
            }
        }
        /// <summary>
        /// 更新怪物位置同步状态
        /// </summary>
        /// <param name="monster"></param>
        void UpdateMonsterEntity(RemoteEnemy monster)
        {
            if (monster.go != null)
            {
                monster.go.transform.position = monster.CurrentPos;
                monster.go.transform.forward = monster.CurrentDir;
            }
        }

        /// <summary>
        /// 注册Actor
        /// Register Actor
        /// </summary>
        public void RegisterActor(Actor actor)
        {
            if (!actors.Contains(actor))
                actors.Add(actor);
            Debug.Log($"Registered Actor, Actors Count: {actors.Count}");
        }

        /// <summary>
        /// 注销Actor
        /// Unregister Actor
        /// </summary>
        public void UnregisterActor(Actor actor)
        {
            if (actors.Contains(actor))
                actors.Remove(actor);
            Debug.Log($"Unregistered Actor, Actors Count: {actors.Count}");
        }

        /// <summary>
        /// 获取范围内的Actors
        /// Get Actors within range
        /// </summary>
        public List<Actor> GetActorsWithinRange(Actor mySelf, Vector3 position, float range)
        {
            List<Actor> nearbyActors = new List<Actor>();
            foreach (var actor in actors)
            {
                if (actor == mySelf)
                    continue;
                if (Vector3.Distance(actor.transform.position, position) <= range)
                {
                    nearbyActors.Add(actor);
                }
            }

            return nearbyActors;
        }

        /// <summary>
        /// 获取范围内的Actors
        /// Get Actors within range
        /// </summary>
        public List<Actor> GetActorsWithinRange(Actor mySelf, Vector3 position, float range, ActorTypeFilter filter = null)
        {
            List<Actor> nearbyActors = new List<Actor>();
            foreach (var actor in actors)
            {
                if (actor == mySelf)
                    continue;
                if (Vector3.Distance(actor.transform.position, position) <= range)
                {
                    if (filter == null || filter(actor))
                        nearbyActors.Add(actor);
                }
            }

            return nearbyActors;
        }

        /// <summary>
        /// 根据类型获取所有Actors
        /// Get all Actors by type
        /// </summary>
        public List<T> GetAllActorsByType<T>() where T : Actor
        {
            List<T> typeActors = new List<T>();
            foreach (var actor in actors)
            {
                if (actor is T)
                    typeActors.Add((T)actor);
            }
            return typeActors;
        }

        /// <summary>
        /// 同步怪物移动位置(远程)回调
        /// </summary>
        void SyncMonsterMovePos(NetMsg msg)
        {
            if (SceneManager.GetActiveScene().name != "TestScene")
                return;

            SyncMonsterMovePos syncMonsterMovePos = msg.syncMonsterMovePos;
            if (!monsters_remote.ContainsKey(syncMonsterMovePos.monsterID))
            {
                GameObject monster = GetMonsterGoByType_remote(syncMonsterMovePos.monsterType);
                GameObject go = Instantiate(monster, new Vector3(syncMonsterMovePos.PosX, 0, syncMonsterMovePos.PosZ), Quaternion.identity);

                RemoteEnemy remoteEnemy = go.AddComponent<RemoteEnemy>();
                remoteEnemy.monsterID = syncMonsterMovePos.monsterID;
                remoteEnemy.CurrentPos = new Vector3(syncMonsterMovePos.PosX, 0, syncMonsterMovePos.PosZ);
                remoteEnemy.TargetPos = new Vector3(syncMonsterMovePos.PosX, 0, syncMonsterMovePos.PosZ);
                remoteEnemy.CurrentDir = new Vector3(syncMonsterMovePos.dirX, syncMonsterMovePos.dirY, syncMonsterMovePos.dirZ);
                remoteEnemy.TargetDir = new Vector3(syncMonsterMovePos.dirX, syncMonsterMovePos.dirY, syncMonsterMovePos.dirZ);
                remoteEnemy.LastUpdateTime = syncMonsterMovePos.timestamp;
                remoteEnemy.go = go;

                monsters_remote.Add(syncMonsterMovePos.monsterID, remoteEnemy);
            }
            else
            {
                RemoteEnemy remoteEnemy = monsters_remote[syncMonsterMovePos.monsterID];
                remoteEnemy.UpdateState(
                    new Vector3(syncMonsterMovePos.PosX, remoteEnemy.CurrentPos.y, syncMonsterMovePos.PosZ),
                    new Vector3(syncMonsterMovePos.dirX, syncMonsterMovePos.dirY, syncMonsterMovePos.dirZ),
                    syncMonsterMovePos.timestamp
                );
            }
        }
        /// <summary>
        /// 根据怪物类型获取怪物GameObject
        /// </summary>
        GameObject GetMonsterGoByType_remote(MonstersEnum monsterType)
        {
            foreach (var monster in monsters_all_remote)
            {
                if (monster.GetComponent<AIActor>().monsterType == monsterType)
                {
                    return monster;
                }
            }
            return null;
        }

        /// <summary>
        /// 创建怪物
        /// </summary>
        void CreateMonsters(NetMsg msg)
        {
            CreateMonsters createMonsters = msg.createMonsters;
            GameObject monster = GetMosnterByType(createMonsters.monsterType);
            monster.GetComponent<AIActor>().monsterID = createMonsters.monsterID;
            GameObject go = Instantiate(monster, new Vector3(createMonsters.PosX, 0, createMonsters.PosZ), Quaternion.identity);
        }
        GameObject GetMosnterByType(MonstersEnum monsterType)
        {
            foreach (var monster in monster_all)
            {
                if (monster.GetComponent<AIActor>().monsterType == monsterType)
                {
                    return monster;
                }
            }
            return null;
        }
    }
}
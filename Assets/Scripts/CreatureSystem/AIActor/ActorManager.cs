using RCProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AIActor_RC
{
    /// <summary>
    /// Actor���͹�����
    /// Actor type filter
    /// </summary>
    public delegate bool ActorTypeFilter(Actor actor);

    public class ActorManager : Singleton<ActorManager>
    {
        List<Actor> actors = new List<Actor>();

        public List<GameObject> monster_all = new List<GameObject>(); // ��ǰ�����еĹ����б�
        public List<GameObject> monsters_all_remote = new List<GameObject>(); // ��ǰ�����еĹ���(Զ��)�б�
        Dictionary<int, GameObject> monsters_remote = new Dictionary<int, GameObject>(); // Զ�̹����б�

        protected override void Awake()
        {
            base.Awake();

            NetManager.Instance.RegisterNtfHandler(CMD.SyncMonsterMovePos, SyncMonsterMovePos);

            NetManager.Instance.RegisterNtfHandler(CMD.CreateMonsters, CreateMonsters); // ��һ�ν�����Ϸ����Ҵ�������

            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// ע��Actor
        /// Register Actor
        /// </summary>
        public void RegisterActor(Actor actor)
        {
            if (!actors.Contains(actor))
                actors.Add(actor);
            Debug.Log($"Registered Actor, Actors Count: {actors.Count}");
        }

        /// <summary>
        /// ע��Actor
        /// Unregister Actor
        /// </summary>
        public void UnregisterActor(Actor actor)
        {
            if (actors.Contains(actor))
                actors.Remove(actor);
            Debug.Log($"Unregistered Actor, Actors Count: {actors.Count}");
        }

        /// <summary>
        /// ��ȡ��Χ�ڵ�Actors
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
        /// ��ȡ��Χ�ڵ�Actors
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
        /// �������ͻ�ȡ����Actors
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
        /// ͬ�������ƶ�λ��(Զ��)�ص�
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
                monsters_remote.Add(syncMonsterMovePos.monsterID, go);
            }
            else
            {
                GameObject go = monsters_remote[syncMonsterMovePos.monsterID];
                go.transform.position = new Vector3(syncMonsterMovePos.PosX, 0, syncMonsterMovePos.PosZ);
                go.transform.forward = new Vector3(syncMonsterMovePos.dirX, syncMonsterMovePos.dirY, syncMonsterMovePos.dirZ);
            }
        }
        /// <summary>
        /// ���ݹ������ͻ�ȡ����GameObject
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
        /// ��������
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
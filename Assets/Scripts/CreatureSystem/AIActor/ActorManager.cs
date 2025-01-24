using System.Collections.Generic;
using UnityEngine;

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

        protected override void Awake()
        {
            base.Awake();

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
    }
}
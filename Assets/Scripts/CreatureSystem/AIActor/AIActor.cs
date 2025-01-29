using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Trees;
using RCProtocol;

namespace AIActor_RC
{
    public class AIActor : Actor
    {
        [Range(0.1f, 100f)]
        public float viewRadius = 10f; // 视野半径 View radius

        [Range(0.1f, 100f)]
        public float attackRadius = 1f; // 攻击半径 Attack radius 

        protected SteeringBehaviors steeringBehaviors;
        protected WanderBehaviors wanderBehaviors;
        protected PursueBehaviors pursueBehaviors;
        protected CollisionSensor collisionSensor;

        [SerializeField]
        protected BehaviorTree brain; // AI行为树 AI Behavior tree

        public int monsterID;
        public MonstersEnum monsterType;

        protected override void Start()
        {
            base.Start();
            steeringBehaviors = GetComponent<SteeringBehaviors>();
            wanderBehaviors = GetComponent<WanderBehaviors>();
            pursueBehaviors = GetComponent<PursueBehaviors>();
            collisionSensor = GetComponent<CollisionSensor>();
        }

        protected virtual void Update() { }
        protected virtual void FixedUpdate() { }

        /// <summary>
        /// 获取视野内的角色 
        /// Get actors in view
        /// </summary>
        public List<Actor> GetActorsInView()
        {
            if (ActorManager.Instance != null)
            {
                return ActorManager.Instance.GetActorsWithinRange(this, transform.position, viewRadius);
            }

            return new List<Actor>();
        }

        /// <summary>
        /// 获取视野内的角色 
        /// Get actors in view
        /// </summary>
        public List<Actor> GetActorsInView(ActorTypeFilter filter = null)
        {
            if (ActorManager.Instance != null)
            {
                return ActorManager.Instance.GetActorsWithinRange(this, transform.position, viewRadius, filter);
            }

            return new List<Actor>();
        }

        /*private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, viewRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }*/
    }
}

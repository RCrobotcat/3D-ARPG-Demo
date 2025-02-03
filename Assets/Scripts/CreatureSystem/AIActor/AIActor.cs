using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Trees;
using RCProtocol;
using UnityEngine.UI;
using DG.Tweening;

namespace AIActor_RC
{
    public class AIActor : Actor
    {
        [Range(0.1f, 100f)]
        public float viewRadius = 10f; // ��Ұ�뾶 View radius

        [Range(0.1f, 100f)]
        public float attackRadius = 1f; // �����뾶 Attack radius 

        protected SteeringBehaviors steeringBehaviors;
        protected WanderBehaviors wanderBehaviors;
        protected PursueBehaviors pursueBehaviors;
        protected CollisionSensor collisionSensor;

        [SerializeField]
        protected BehaviorTree brain; // AI��Ϊ�� AI Behavior tree

        [HideInInspector] public int monsterID;
        public MonstersEnum monsterType;

        public float maxHealth = 20f; // �������ֵ
        float currentHealth; // ��ǰ����ֵ
        public float CurrentHealth { get => currentHealth; }

        public Transform healthBar;
        Image healthSlider;
        Text healthText;

        protected bool isDead; // �Ƿ�����
        bool isDeadAnimated = false; // �Ƿ��Ѿ�������������

        protected override void Start()
        {
            base.Start();
            steeringBehaviors = GetComponent<SteeringBehaviors>();
            wanderBehaviors = GetComponent<WanderBehaviors>();
            pursueBehaviors = GetComponent<PursueBehaviors>();
            collisionSensor = GetComponent<CollisionSensor>();

            currentHealth = maxHealth;
            healthSlider = healthBar.GetChild(0).GetComponent<Image>();
            healthText = healthBar.GetChild(1).GetComponent<Text>();
            UpdateHealthBar();
        }

        protected virtual void Update()
        {
            if (isDead && !isDeadAnimated)
            {
                animator.SetTrigger("Dead");
                isDeadAnimated = true;
                healthBar.gameObject.SetActive(false);
                // Destroy(gameObject, 2f);
            }
        }
        protected virtual void FixedUpdate() { }

        /// <summary>
        /// ��ȡ��Ұ�ڵĽ�ɫ 
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
        /// ��ȡ��Ұ�ڵĽ�ɫ 
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


        /// <summary>
        /// �ܵ�����
        /// </summary>
        public void BeAttack(float damage)
        {
            float health = currentHealth - damage;
            currentHealth = Mathf.Clamp(health, 0, maxHealth);
            animator.SetTrigger("BeHit");
            // Debug.Log($"Monster {monsterType} be attacked, health: {currentHealth} -> {health}");
            UpdateHealthBar();
        }
        void UpdateHealthBar()
        {
            float sliderPercentage = currentHealth / maxHealth;
            healthSlider.DOFillAmount(sliderPercentage, 0.3f);
            healthText.text = $"{currentHealth}/{maxHealth}";

            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
            }
        }
    }
}

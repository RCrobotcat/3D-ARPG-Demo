using UnityEngine;


namespace AIActor_RC
{
    public class Actor : MonoBehaviour
    {
        [HideInInspector] public Animator animator;
        protected Rigidbody rb;

        protected virtual void Start() { }

        protected virtual void OnEnable()
        {
            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody>();

            if (ActorManager.Instance != null)
            {
                ActorManager.Instance.RegisterActor(this);
            }
            else
            {
                Debug.LogError("ActorManager.Instance is null!");
            }
        }

        protected virtual void OnDisable()
        {
            if (ActorManager.Instance != null)
            {
                ActorManager.Instance.UnregisterActor(this);
            }
            else
            {
                Debug.LogError("ActorManager.Instance is null!");
            }
        }

        public Rigidbody GetRigidbody()
        {
            return rb;
        }
    }
}

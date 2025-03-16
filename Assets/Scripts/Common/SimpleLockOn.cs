using System.Collections;
using UnityEngine;

// Object look at the given target
// default target is Camera.main
public class SimpleLockOn : MonoBehaviour
{
    [SerializeField] Transform target;

    void OnEnable()
    {
        if (target == null) target = Camera.main.transform;
        StartCoroutine(LookAtTarget());
    }

    private IEnumerator LookAtTarget()
    {
        while (this.gameObject.activeInHierarchy)
        {
            Vector3 _dir = target.position - transform.position;
            //_dir.y = 0;
            transform.rotation = Quaternion.LookRotation(_dir);
            yield return null;
        }
    }
}
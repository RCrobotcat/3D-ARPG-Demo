using System.Collections;
using UnityEngine;

public class ObjectPoolTools : MonoBehaviour
{
    public bool needAutoRecycle; // 是否需要自动回收粒子
    public float maxDuration = 0.5f;

    void OnEnable()
    {
        if (needAutoRecycle)
            StartCoroutine(DelayedRecycle());
    }

    // 定时销毁粒子 Destroy particles after a certain time
    IEnumerator DelayedRecycle()
    {
        yield return new WaitForSeconds(maxDuration);

        // Destroy(gameObject);
        gameObject.Recycle();
    }
}

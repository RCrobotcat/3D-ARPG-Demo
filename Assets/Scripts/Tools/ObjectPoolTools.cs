using System.Collections;
using UnityEngine;

public class ObjectPoolTools : MonoBehaviour
{
    public bool needAutoRecycle; // �Ƿ���Ҫ�Զ���������
    public float maxDuration = 0.5f;

    void OnEnable()
    {
        if (needAutoRecycle)
            StartCoroutine(DelayedRecycle());
    }

    // ��ʱ�������� Destroy particles after a certain time
    IEnumerator DelayedRecycle()
    {
        yield return new WaitForSeconds(maxDuration);

        // Destroy(gameObject);
        gameObject.Recycle();
    }
}

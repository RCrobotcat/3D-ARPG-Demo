using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static bool init;

    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                }
                else
                {
                    init = true;
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (init) return;

        if (instance == null)
        {
            instance = (T)this;
        }
        else
        {
            Debug.LogWarning("An instance of " + typeof(T) + " already exists in the scene.");
        }
    }
}
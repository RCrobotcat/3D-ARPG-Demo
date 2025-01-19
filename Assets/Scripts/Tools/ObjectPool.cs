using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectPool : MonoBehaviour
{
    static ObjectPool _instance; // Singleton

    [System.Serializable]
    public class StartUpPool
    {
        public int size;
        public GameObject prefab;
    }

    public StartUpPool[] startUpPools; // List of object pools that can be set in the Inspector
    bool startUpPoolsCreated;

    // Key: prefab, Value: list of spawned objects
    Dictionary<GameObject, List<GameObject>> pooledGameObjects = new Dictionary<GameObject, List<GameObject>>();

    // Key: spawned object, Value: prefab of the spawned object
    // Active object container
    Dictionary<GameObject, GameObject> spawnedGameObjects = new Dictionary<GameObject, GameObject>();

    void Awake()
    {
        _instance = this;
        CreateStartUpPools();
    }

    public static ObjectPool Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            _instance = FindObjectOfType<ObjectPool>();
            if (_instance != null)
                return _instance;

            var obj = new GameObject("ObjectPool");
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            _instance = obj.AddComponent<ObjectPool>();
            return _instance;
        }
    }

    public static void CreatePool(GameObject prefab, int initialPoolSize)
    {
        if (prefab != null && !Instance.pooledGameObjects.ContainsKey(prefab))
        {
            var list = new List<GameObject>();
            Instance.pooledGameObjects.Add(prefab, list);

            // If initial pool size is greater than 0, spawn objects and add them to the object pool
            if (initialPoolSize > 0)
            {
                bool active = prefab.activeSelf;
                prefab.SetActive(false);
                Transform parent = Instance.transform;
                while (list.Count < initialPoolSize)
                {
                    var obj = (GameObject)Instantiate(prefab);
                    obj.transform.SetParent(parent);
                    list.Add(obj);
                }
                prefab.SetActive(active);
            }
        }
    }

    // Create an object pool
    public static void CreateStartUpPools()
    {
        if (!Instance.startUpPoolsCreated)
        {
            Instance.startUpPoolsCreated = true;
            var pools = Instance.startUpPools;
            if (pools != null && pools.Length > 0)
            {
                for (int i = 0; i < pools.Length; i++)
                {
                    CreatePool(pools[i].prefab, pools[i].size);
                }
            }
        }
    }

    // Create a game object using the object pool
    public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        List<GameObject> list;
        Transform trans;
        GameObject obj;

        if (Instance.pooledGameObjects.TryGetValue(prefab, out list))
        {
            obj = null;
            if (list.Count > 0)
            {
                while (obj == null && list.Count > 0)
                {
                    obj = list[0];
                    list.RemoveAt(0);
                }
                if (obj != null)
                {
                    trans = obj.transform;
                    trans.parent = parent;
                    trans.localPosition = position;
                    trans.localRotation = rotation;
                    obj.SetActive(true);
                    Instance.spawnedGameObjects.Add(obj, prefab);
                    return obj;
                }
            }

            // If there are no objects in the object pool, spawn a new object
            obj = (GameObject)Instantiate(prefab);
            trans = obj.transform;
            trans.parent = parent;
            trans.localPosition = position;
            trans.localRotation = rotation;
            Instance.spawnedGameObjects.Add(obj, prefab);
            return obj;
        }
        else // if the prefab is not in the object pool, spawn a new object directly without object pool management
        {
            obj = (GameObject)Instantiate(prefab);
            trans = obj.GetComponent<Transform>();
            trans.parent = parent;
            trans.localPosition = position;
            trans.localRotation = rotation;
            return obj;
        }
    }

    // Object pool recycling
    static void Recycle(GameObject obj, GameObject prefab)
    {
        Instance.pooledGameObjects[prefab].Add(obj);
        Instance.spawnedGameObjects.Remove(obj);
        obj.transform.parent = Instance.transform;
        obj.SetActive(false);
    }

    public static void Recycle(GameObject obj)
    {
        GameObject prefab;
        if (Instance.spawnedGameObjects.TryGetValue(obj, out prefab))
        {
            Recycle(obj, prefab);
        }
        else
        {
            Destroy(obj);
        }
    }

    public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
    {
        return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
    }

    public static void Recycle<T>(T obj) where T : Component
    {
        Recycle(obj.gameObject);
    }
}

public static class ObjectPoolExtensions
{
    public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
    {
        return ObjectPool.Spawn(prefab, parent, position, rotation);
    }

    public static void Recycle<T>(this T obj) where T : Component
    {
        ObjectPool.Recycle(obj);
    }

    public static void Recycle(this GameObject obj)
    {
        ObjectPool.Recycle(obj);
    }
}
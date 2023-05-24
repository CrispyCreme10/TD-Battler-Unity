using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TDBattler.Runtime
{
    public class ObjectPooler : MonoBehaviour
    {    
        public static ObjectPooler Instance { get; private set; }
        private Dictionary<string, ObjectPool> _pools;
        private GameObject _poolContainer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            _pools = new Dictionary<string, ObjectPool>();
            _poolContainer = new GameObject($"Pools");
        }

        public void CreatePool(string name, GameObject prefab, int poolSize = 5, ObjectPoolOptions opts = null)
        {
            if (!_pools.ContainsKey(name))
            {
                _pools[name] = new ObjectPool(_poolContainer.transform, name, prefab, poolSize, opts);
                InitPool(_pools[name]);
            }
        }

        private void InitPool(ObjectPool pool)
        {
            for (int i = 0; i < pool.PoolSize; i++)
            {
                var instance = CreateInstance(pool.ObjectPrefab, pool.PoolContainer.transform);
                pool.InitObject(instance, i);
                pool.Objects.Add(instance);
            }
            Debug.Log($"Pool: {pool.PoolName} created!");
        }

        private GameObject CreateInstance(GameObject prefab, Transform parent)
        {
            return Instantiate(prefab, parent);
        }

        public GameObject GetInstanceFromPool(string name)
        {
            if (_pools.TryGetValue(name, out ObjectPool pool))
            {
                return pool.GetObjectFromPool();
            }

            return null;
        }

        public void ReturnToPool(GameObject instance)
        {
            instance.SetActive(false);
        }

        public IEnumerator ReturnToPoolWithDelay(GameObject instance, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToPool(instance);
        }
    }

    public class ObjectPool
    {
        public string PoolName { get; private set; }
        public int PoolSize { get; private set; }
        public int ObjectIndex { get; private set; }
        public List<GameObject> Objects { get; private set; }
        public GameObject ObjectPrefab { get; private set; }
        public GameObject PoolContainer { get; private set; }
        private ObjectPoolOptions _options;

        public ObjectPool(Transform parentTransform, string name, GameObject prefab, int poolSize = 5, ObjectPoolOptions opts = null)
        {
            PoolName = name;
            PoolSize = poolSize;
            ObjectIndex = 0;
            Objects = new List<GameObject>();
            ObjectPrefab = prefab;
            PoolContainer = new GameObject($"{name} - Pool");
            PoolContainer.transform.parent = parentTransform;
            _options = opts;
        }

        public void InitObject(GameObject newInstance, int i)
        {
            if (i >= 0)
            {
                newInstance.name += $" {i}";
            }
            newInstance.transform.position = _options.Position ?? newInstance.transform.position;
            newInstance.SetActive(false);
        }
    
        public GameObject GetObjectFromPool()
        {
            var obj = Objects[ObjectIndex];
            obj.SetActive(true);
            IncrementIndex();
            return obj;  
        }

        private void IncrementIndex()
        {
            if (ObjectIndex >= Objects.Count - 1)
            {
                ObjectIndex = 0;
                return;
            }

            ObjectIndex++;
        }
    }

    public class ObjectPoolOptions
    {
        public Vector3? Position { get; set; }

        public ObjectPoolOptions(Vector3? position)
        {
            Position = position;
        }
    }
}
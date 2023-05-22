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

            }
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
            instance.SetActive(false);
        }
    }

    public class ObjectPool : MonoBehaviour
    {
        public string PoolName { get; private set; }
        public int PoolSize { get; private set; }
        public int ObjectIndex { get; private set; }
        public List<GameObject> Objects { get; private set; }
        public GameObject ObjectPrefab { get; private set; }
        private GameObject _poolContainer;
        private ObjectPoolOptions _options;

        public ObjectPool(Transform parentTransform, string name, GameObject prefab, int poolSize = 5, ObjectPoolOptions opts = null)
        {
            PoolName = name;
            PoolSize = poolSize;
            ObjectIndex = 0;
            Objects = new List<GameObject>();
            ObjectPrefab = prefab;
            _poolContainer = new GameObject($"{name} - Pool");
            _poolContainer.transform.parent = parentTransform;
            _options = opts;

            CreatePool();
        }

        private void CreatePool()
        {
            for (int i = 0; i < PoolSize; i++)
            {
                Objects.Add(CreateInstance(i));
            }
            Debug.Log($"Pool: {PoolName} created!");
        }

        private GameObject CreateInstance(int i)
        {
            GameObject newInstance = Instantiate(ObjectPrefab, _poolContainer.transform);
            if (i >= 0)
            {
                newInstance.name += $" {i}";
            }
            newInstance.transform.position = _options.Position ?? newInstance.transform.position;
            newInstance.SetActive(false);
            return newInstance;
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
            if (ObjectIndex == Objects.Count)
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
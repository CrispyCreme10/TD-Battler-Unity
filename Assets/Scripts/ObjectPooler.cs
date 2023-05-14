using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;
    
    public static ObjectPooler Instance { get; private set; }
    private List<GameObject> _pool;
    private GameObject _poolContainer;
    private Vector3 _startingPosition;

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

        _pool = new List<GameObject>();
        _poolContainer = new GameObject($"Pool - {prefab.name}");
        Waypoint waypoint = GameObject.Find("Spawner").GetComponent<Waypoint>();
        _startingPosition = waypoint.GetWaypointPosition(0);
        CreatePooler();
    }

    private void CreatePooler()
    {
        for (int i = 0; i < poolSize; i++)
        {
            _pool.Add(CreateInstance());
        }
    }

    private GameObject CreateInstance()
    {
        GameObject newInstance = Instantiate(prefab, _poolContainer.transform);
        newInstance.transform.position = _startingPosition;
        newInstance.SetActive(false);
        return newInstance;
    }

    public GameObject GetInstanceFromPool()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].activeInHierarchy)
            {
                return _pool[i];
            }
        }

        return CreateInstance();
    }

    public void ReturnToPool(GameObject instance)
    {
        instance.SetActive(false);
        instance.transform.position = _startingPosition;
    }

    public static IEnumerator ReturnToPoolWithDelay(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }
}

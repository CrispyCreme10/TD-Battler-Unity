using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Spawner spawner;
    [SerializeField] private Tower[] towerPrefabs;

    private Transform _towersContainer;
    private Towerpoint _towerpoint;
    private List<PointData> _pointData;
    private int RandomTowerIndex => (int)Mathf.Round(Random.value) * (towerPrefabs.Length - 1);
    private int _spawnTowerCount;

    private void Start()
    {
        _towerpoint = GameObject.Find("Spawner").GetComponent<Towerpoint>();
        _towersContainer = GameObject.Find("Towers").transform;
        _pointData = Enumerable.Range(0, _towerpoint.Points.Length - 1).Select(i => new PointData(i)).ToList();
        _spawnTowerCount = 0;
    }

    public void SpawnRandomFirstTower()
    {
        int randomTowerPointIndex = GetRandomAvailableTowerPointIndex();
        SpawnTower(RandomTowerIndex, randomTowerPointIndex);
    }

    public void SpawnRandomTowerAtPointOfMergeLevel(int towerPointIndex, int mergeLevel)
    {
        SpawnTower(RandomTowerIndex, towerPointIndex, mergeLevel);
    }

    public int DespawnTower(GameObject tower)
    {
        var pd = _pointData.SingleOrDefault(pd => pd.TowerObject == tower);
        int index = pd.PointIndex;
        Debug.Log(pd.TowerObject);
        Destroy(tower);
        Debug.Log(pd.TowerObject);
        return index;
    }

    private void SpawnTower(int towerIndex, int towerPointIndex, int mergeLevel = 0)
    {
        Tower instance = Instantiate(towerPrefabs[towerIndex], _towerpoint.Points[towerPointIndex], Quaternion.identity, _towersContainer);
        UpdatePointData(instance.gameObject, towerPointIndex);
        instance.name += " " + ++_spawnTowerCount;
        if (mergeLevel > 0)
        {
            instance.SetMergeLevel(mergeLevel);
        }
        instance.UpdateEnemies(spawner.Enemies);
    }

    private int GetRandomAvailableTowerPointIndex()
    {
        List<int> availablePoints = _pointData.Where(pd => pd.TowerObject == null).Select(pd => pd.PointIndex).ToList();
        int availableIndex = Random.Range(0, availablePoints.Count());
        return availablePoints[availableIndex];
    }

    private void UpdatePointData(GameObject towerObject, int pointIndex)
    {
        _pointData.SingleOrDefault(pd => pd.PointIndex == pointIndex).TowerObject = towerObject;
    }
}

public class PointData
{
    public GameObject TowerObject { get; set; }
    public int PointIndex { get; private set; }

    public PointData(int index)
    {
        PointIndex = index;
    }
}
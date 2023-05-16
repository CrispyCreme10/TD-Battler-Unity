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

    private List<int> _availableTowerPointIndices;
    private List<int> _occupiedTowerPointIndices;

    private void Start()
    {
        _towerpoint = GameObject.Find("Spawner").GetComponent<Towerpoint>();
        _towersContainer = GameObject.Find("Towers").transform;
        _availableTowerPointIndices = Enumerable.Range(0, _towerpoint.Points.Length - 1).ToList();
        _occupiedTowerPointIndices = new List<int>();
    }

    public void SpawnTower()
    {
        int towerIndex = (int)Mathf.Round(Random.value) * (towerPrefabs.Length - 1);
        int availableIndex = Random.Range(0, _availableTowerPointIndices.Count());
        int towerPointIndex = _availableTowerPointIndices[availableIndex];
        _occupiedTowerPointIndices.Add(towerPointIndex);
        _availableTowerPointIndices.Remove(towerPointIndex);
        Tower instance = Instantiate(towerPrefabs[towerIndex], _towerpoint.Points[towerPointIndex], Quaternion.identity, _towersContainer);
        instance.name += $" {_occupiedTowerPointIndices.Count() - 1}";
        instance.UpdateEnemies(spawner.Enemies);
    }

    private void AddAvailablePoint()
    {

    }
}

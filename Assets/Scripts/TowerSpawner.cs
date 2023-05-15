using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private Tower[] towerPrefabs;

    private Transform _towersContainer;
    private Towerpoint _towerpoint;

    private List<int> _availableTowerPoints;

    private void Start()
    {
        _towerpoint = GameObject.Find("Spawner").GetComponent<Towerpoint>();
        _towersContainer = GameObject.Find("Towers").transform;
        _availableTowerPoints = Enumerable.Range(0, _towerpoint.Points.Length).ToList();
    }

    public void SpawnTower()
    {
        int towerIndex = (int)Mathf.Round(Random.value) * (towerPrefabs.Length - 1);
        // int towerPointIndex = Random.Range(0, _availableTowerPoints.Count());
        // _availableTowerPoints.Remove
        // Instantiate(towerPrefabs[towerIndex], _towerpoint.Points[towerPointIndex], Quaternion.identity, _towersContainer);

        // need to get random spawn location and only select valid locations afterwards
    }
}

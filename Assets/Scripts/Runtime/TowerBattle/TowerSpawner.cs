using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class TowerSpawner : MonoBehaviour
    {
        public static System.Action<Tower> OnTowerSpawn;
        public static System.Action<Tower> OnTowerDespawn;
        public static System.Action<int, int> OnTowerEnergyIncrease;

        [Header("References")]
        [SerializeField] private Towerpoint towerpoint;
        [SerializeField] private EnemySpawner enemySpawner;

        private Transform _towersContainer;
        private List<PointData> _pointData;
        private PlayerTowers _playerSelectedTowers => Singleton.Instance.PlayerManager.SelectedTowers;
        private int RandomTowerIndex => (int)Mathf.Round(UnityEngine.Random.Range(0, _playerSelectedTowers.Towers.Count));
        private int _spawnTowerCount;
        private List<TowerScriptableObject> _towerData;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            _towersContainer = new GameObject("Towers").transform;
            _pointData = Enumerable.Range(0, towerpoint.Points.Length).Select(i => new PointData(i)).ToList();
            _spawnTowerCount = 0;

            _towerData = new List<TowerScriptableObject>();
            foreach (var prefab in _playerSelectedTowers.Towers)
            {
                _towerData.Add(prefab.GetComponent<Tower>().TowerData);
            }
        }

        public GameObject SpawnRandomFirstTower(Func<int, int> GetEnergyLevel)
        {
            int randomTowerPointIndex = GetRandomAvailableTowerPointIndex();
            if (randomTowerPointIndex >= 0)
            {
                // determine energyLevel
                int energyLevel = GetEnergyLevel(randomTowerPointIndex);
                return SpawnTower(RandomTowerIndex, randomTowerPointIndex, energyLevel);
            }

            return null;
        }

        public GameObject SpawnRandomTowerAtPointOfMergeLevel(int towerPointIndex, int energyLevel, int mergeLevel)
        {
            return SpawnTower(RandomTowerIndex, towerPointIndex, energyLevel, mergeLevel);
        }

        private GameObject SpawnTower(int towerIndex, int towerPointIndex, int energyLevel, int mergeLevel = 1)
        {
            GameObject instanceGO = Instantiate(_playerSelectedTowers.Towers[towerIndex], towerpoint.Points[towerPointIndex], Quaternion.identity, _towersContainer);
            Tower instance = instanceGO.GetComponent<Tower>();
            UpdatePointData(instance, towerPointIndex);
            instance.name += " " + ++_spawnTowerCount;
            if (mergeLevel > 0)
            {
                instance.SetMergeLevel(mergeLevel);
            }
            if (energyLevel > 0)
            {
                instance.SetEnergyLevel(energyLevel);
            }
            instance.UpdateEnemies(enemySpawner.Enemies);
            OnTowerSpawn?.Invoke(instance);

            return instanceGO;
        }

        public int DespawnTower(Tower tower)
        {
            var pd = _pointData.SingleOrDefault(pd => pd.Tower == tower);
            pd.Tower = null;
            Destroy(tower.gameObject);
            return pd.PointIndex;
        }

        private int GetRandomAvailableTowerPointIndex()
        {
            List<int> availablePoints = _pointData.Where(pd => pd.Tower == null).Select(pd => pd.PointIndex).ToList();
            if (availablePoints.Count == 0)
            {
                return -1;
            }

            int availableIndex = 0;
            if (availablePoints.Count > 1)
            {
                availableIndex = UnityEngine.Random.Range(0, availablePoints.Count());
            }
            return availablePoints[availableIndex];
        }

        private void UpdatePointData(Tower tower, int pointIndex)
        {
            _pointData.SingleOrDefault(pd => pd.PointIndex == pointIndex).Tower = tower;
        }

        public bool IsFieldFull()
        {
            return _spawnTowerCount == towerpoint.Points.Length;
        }
    }

    public class PointData
    {
        public Tower Tower { get; set; }
        public int PointIndex { get; private set; }

        public PointData(int index)
        {
            PointIndex = index;
        }
    }
}
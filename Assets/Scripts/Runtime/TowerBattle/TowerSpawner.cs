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
        [SerializeField] private EnemySpawner enemySpawner;

        private Transform _towersContainer;
        private Towerpoint _towerpoint;
        private List<PointData> _pointData;
        private List<Tower> _playerSelectedTowers => Singleton.Instance.PlayerManager.SelectedTowers;
        private int RandomTowerIndex => (int)Mathf.Round(Random.value) * (_playerSelectedTowers.Count - 1);
        private int _spawnTowerCount;
        private List<TowerScriptableObject> _towerData;

        private void OnEnable()
        {
            TowerBattle_UI.OnEnergyIncrease += OnEnergyIncrease;
        }

        private void OnDisable()
        {
            TowerBattle_UI.OnEnergyIncrease -= OnEnergyIncrease;
        }

        private void Start()
        {
            _towerpoint = GetComponent<Towerpoint>();
            _towersContainer = new GameObject("Towers").transform;
            _pointData = Enumerable.Range(0, _towerpoint.Points.Length).Select(i => new PointData(i)).ToList();
            _spawnTowerCount = 0;

            _towerData = new List<TowerScriptableObject>();
            foreach (var prefab in _playerSelectedTowers)
            {
                _towerData.Add(prefab.TowerData);
            }
        }

        public void SpawnRandomFirstTower()
        {
            int randomTowerPointIndex = GetRandomAvailableTowerPointIndex();
            if (randomTowerPointIndex >= 0)
            {
                SpawnTower(RandomTowerIndex, randomTowerPointIndex);
            }
        }

        public void SpawnRandomTowerAtPointOfMergeLevel(int towerPointIndex, int mergeLevel)
        {
            SpawnTower(RandomTowerIndex, towerPointIndex, mergeLevel);
        }

        private void SpawnTower(int towerIndex, int towerPointIndex, int mergeLevel = 1)
        {
            Tower instance = Instantiate(_playerSelectedTowers[towerIndex], _towerpoint.Points[towerPointIndex], Quaternion.identity, _towersContainer);
            UpdatePointData(instance, towerPointIndex);
            instance.name += " " + ++_spawnTowerCount;
            if (mergeLevel > 0)
            {
                instance.SetMergeLevel(mergeLevel);
            }
            instance.UpdateEnemies(enemySpawner.Enemies);
            OnTowerSpawn?.Invoke(instance);
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
                availableIndex = Random.Range(0, availablePoints.Count());
            }
            return availablePoints[availableIndex];
        }

        private void UpdatePointData(Tower tower, int pointIndex)
        {
            _pointData.SingleOrDefault(pd => pd.PointIndex == pointIndex).Tower = tower;
        }

        private void OnEnergyIncrease(int index)
        {
            var energyLevel = _towerData[index].IncrementEnergyLevel();
            OnTowerEnergyIncrease?.Invoke(index, energyLevel);
        }

        public bool IsFieldFull()
        {
            return _spawnTowerCount == _towerpoint.Points.Length;
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
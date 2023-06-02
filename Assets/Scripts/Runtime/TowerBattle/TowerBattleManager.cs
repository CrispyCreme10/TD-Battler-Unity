using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TDBattler.Runtime
{
    // responsible for handling the state for an active battle
    public class TowerBattleManager : SerializedMonoBehaviour
    {
        public static Action<int, int, List<int>, bool> OnManaChange;

        [Header("References")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private TowerSpawner towerSpawner;

        [Header("Attributes")]
        [SerializeField] private PlayerGameData playerGameData;
        [ReadOnly]
        [NonSerialized, OdinSerialize]
        private TowerDataTracker _towerDataTracker;
        [ReadOnly]
        [SerializeField]
        private Dictionary<string, List<GameObject>> _towerRefs = new Dictionary<string, List<GameObject>>();

        public int Mana => playerGameData.Mana;
        public int TowerCost => playerGameData.Towercost;

        private Collider2D _selectedTowerCollider;
        private Tower _selectedTower => _selectedTowerCollider?.transform.parent?.parent.GetComponent<Tower>();
        private Vector3 _selectedTowerOGPosition;
        private Vector3 _selectedTowerOffset;

        private void Awake()
        {
            playerGameData.TowerEnergyLevelCostIndices = Enumerable.Repeat(0, 5).ToList();
            _towerDataTracker = new TowerDataTracker(playerManager.SelectedTowers);
        }

        private void OnEnable()
        {
            Enemy.OnDeath += OnEnemyDeath;
            TowerBattle_UI_Adapter.OnEnergyIncrease += OnTowerEnergyIncrease;
            Damageable.OnAfterDamageGlobal += OnAfterDamageableDamage;
        }

        private void OnDisable()
        {
            Enemy.OnDeath -= OnEnemyDeath;
            TowerBattle_UI_Adapter.OnEnergyIncrease -= OnTowerEnergyIncrease;
            Damageable.OnAfterDamageGlobal -= OnAfterDamageableDamage;
        }

        private void Start()
        {
            playerGameData.TowerEnergyLevelCostIndices = new List<int> { 0, 0, 0, 0, 0 };
        }

        private void Update()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D[] results = Physics2D.OverlapPointAll(mousePosition);
                if (results.Length > 0)
                {
                    foreach (var result in results)
                    {
                        if (result.name == "Merge")
                        {
                            _selectedTowerCollider = result;
                            _selectedTowerCollider.gameObject.SetActive(false);
                            _selectedTower.GetComponent<Tower>().enabled = false;
                            _selectedTowerOGPosition = _selectedTower.transform.position;
                            _selectedTowerOffset = _selectedTower.transform.position - mousePosition;
                        }
                    }
                }
            }

            if (_selectedTower)
            {
                _selectedTower.transform.position = mousePosition + _selectedTowerOffset;

                Collider2D[] results = Physics2D.OverlapPointAll(mousePosition);
                // if we are dragged over a tower of the same name + merge level, then we can show highlight of tower under mouse
                if (results.Any(c => c.name == "Merge"))
                {
                    Transform tower = results.FirstOrDefault(c => c.name == "Merge")?.transform?.parent?.parent;
                    if (_selectedTower.name.Split('(')[0] == tower.name.Split('(')[0])
                    {
                        // Debug.Log(tower);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && _selectedTower)
            {
                // if we are dragged over a tower of the same name + merge level, then we can merge
                // else, send tower back to original position
                Collider2D[] results = Physics2D.OverlapPointAll(mousePosition);
                Transform towerTransform = results.SingleOrDefault(c => c.name == "Merge")?.transform?.parent?.parent;
                if (towerTransform == null)
                {
                    _selectedTower.transform.position = _selectedTowerOGPosition;
                }
                else if (!MergeTowers(towerTransform.GetComponent<Tower>()))
                {
                    _selectedTower.transform.position = _selectedTowerOGPosition;
                }

                _selectedTower.GetComponent<Tower>().enabled = true;
                _selectedTowerCollider.gameObject.SetActive(true);
                _selectedTowerCollider = null;
            }
        }

        public void SpawnTower()
        {
            if (playerGameData.Mana >= playerGameData.Towercost)
            {
                playerGameData.Mana -= playerGameData.Towercost;
                playerGameData.Towercost += playerGameData.CostIncrease;
                GameObject newTowerInstance = towerSpawner.SpawnRandomFirstTower(GetTowerEnergyLevel);
                AddTowerRef(newTowerInstance);
                ManaChange();
            }
        }

        private void AddTowerRef(GameObject obj)
        {
            string key = GetTowerkey(obj);
            if (!_towerRefs.ContainsKey(key))
            {
                _towerRefs[key] = new List<GameObject>();
            }
            _towerRefs[key].Add(obj);
        }

        private void RemoveTowerRef(GameObject obj)
        {
            string key = GetTowerkey(obj);
            if (!_towerRefs.ContainsKey(key)) return;

            _towerRefs[key].Remove(obj);
        }

        public List<GameObject> GetAllActiveTowerRefs()
        {
            List<GameObject> towerRefs = new List<GameObject>();
            foreach(string key in _towerRefs.Keys)
            {
                towerRefs.AddRange(_towerRefs[key].Where(go => go.activeSelf));
            }
            return towerRefs;
        }

        private string GetTowerkey(GameObject obj)
        {
            Tower tower = obj.GetComponent<Tower>();
            return tower == null ? "" : tower.TowerData.Name;
        }

        private int GetTowerEnergyLevel(int selectedTowerIndex)
        {
            return playerGameData.GetEnergyLevel(selectedTowerIndex);
        }

        private int GetTowerEnergyLevel(string towerName)
        {
            int index = playerManager.SelectedTowers.TowerIndexOf(towerName);
            return playerGameData.GetEnergyLevel(index);
        }

        private void OnEnemyDeath(Enemy enemy)
        {
            IncreaseMana(enemy.DeathCoinReward);
        }

        public void TryIncreaseMana(int incrMana)
        {
            IncreaseMana(incrMana);
        }

        private void IncreaseMana(int incrMana)
        {
            playerGameData.Mana += incrMana;
            ManaChange();
        }

        private void DecreaseMana(int decrMana)
        {
            playerGameData.Mana -= decrMana;
            ManaChange();
        }

        private void ManaChange()
        {
            var energyCosts = playerGameData.TowerEnergyLevelCostIndices.Select(i => playerGameData.EnergyLevelCosts[i]).ToList();
            OnManaChange?.Invoke(playerGameData.Mana, playerGameData.Towercost, energyCosts, towerSpawner.IsFieldFull());
        }

        private void OnTowerEnergyIncrease(string name)
        {
            Debug.Log($"energy: {name}");
            if (!_towerRefs.ContainsKey(name)) return;
            // update towers
            _towerRefs[name].ForEach(go => {
                Debug.Log($"Increase Energy: {go.name}");
                go.GetComponent<Tower>().IncreaseTowerEnergy();
            });

            // update this & UI
            int index = playerManager.SelectedTowers.TowerIndexOf(name);
            if (index == -1) return;

            int decrManaAmt = playerGameData.GetEnergyLevelCost(index);
            string debugOut = string.Join(',', playerGameData.TowerEnergyLevelCostIndices);
            Debug.Log($"{name} {index} {decrManaAmt} {debugOut}");
            DecreaseMana(decrManaAmt);
        }

        private bool MergeTowers(Tower otherTower)
        {
            int selectedMergeLevel = _selectedTower?.MergeLevel ?? 1;
            int otherTowerMergeLevel = otherTower?.MergeLevel ?? 1;
            if (IsValidMerge(otherTower, selectedMergeLevel, otherTowerMergeLevel))
            {
                // allow towers to update something on merge
                _selectedTower.OnMerge(this);
                otherTower.OnMerge(this);

                // merge cleanup
                towerSpawner.DespawnTower(_selectedTower);
                RemoveTowerRef(_selectedTower.gameObject);
                var towerPointIndex = towerSpawner.DespawnTower(otherTower);
                RemoveTowerRef(otherTower.gameObject);

                int energyLevel = GetTowerEnergyLevel(_selectedTower.TowerData.Name);
                GameObject newTower = towerSpawner.SpawnRandomTowerAtPointOfMergeLevel(towerPointIndex, energyLevel, ++selectedMergeLevel);
                AddTowerRef(newTower);
                return true;
            }

            return false;
        }

        private bool IsValidMerge(Tower otherTower, int selectedMergeLevel, int otherTowerMergeLevel)
        {
            return _selectedTower.name.Split('(')[0] == otherTower.name.Split('(')[0] && 
                selectedMergeLevel == otherTowerMergeLevel && 
                selectedMergeLevel < TowerScriptableObject.MAX_MERGE_LEVEL;
        }

        private void OnAfterDamageableDamage(string name, int damage)
        {
            _towerDataTracker.IncrementTowerDamage(name, damage);
        }
    }

    // duties of TBM
    // track tower gameObject references
    // track energy levels foreach distinct tower
    // call tower spawner to spawn towers
    // handles merging of towers
    // tracks mana
    // tracks cost to spawn a random tower & how much to increment the cost by each click

    [Serializable]
    public class PlayerGameData
    {
        public int Mana;
        public int Towercost;
        public int CostIncrease;
        public List<int> EnergyLevelCosts;
        [ReadOnly]
        public List<int> TowerEnergyLevelCostIndices;

        public int GetEnergyLevelCost(int towerIndex)
        {
            if (towerIndex < 0 || towerIndex >= TowerEnergyLevelCostIndices.Count) return int.MaxValue;
            return EnergyLevelCosts[TowerEnergyLevelCostIndices[towerIndex]++];
        }

        public int GetEnergyLevel(int towerIndex)
        {
            if (towerIndex < 0 || towerIndex >= TowerEnergyLevelCostIndices.Count) return 1;
            return TowerEnergyLevelCostIndices[towerIndex] + 1; // energy level is index + 1
        }
    }

    [Serializable]
    public class TowerDataTracker
    {
        [OdinSerialize] private Dictionary<string, int> totalTowerDamageDict = new Dictionary<string, int>();

        public TowerDataTracker(PlayerTowers playerTowers)
        {
            totalTowerDamageDict = playerTowers.Towers.ToDictionary(towerPrefab => towerPrefab.name, i => 0);
        }

        public void IncrementTowerDamage(string name, int damage)
        {
            if (totalTowerDamageDict == null || !totalTowerDamageDict.ContainsKey(name)) return;
            totalTowerDamageDict[name] += damage;
        }
    }
}
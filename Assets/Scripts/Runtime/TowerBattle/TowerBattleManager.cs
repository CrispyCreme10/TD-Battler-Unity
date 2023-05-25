using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDBattler.Runtime
{
    // responsible for handling the state for an active battle
    public class TowerBattleManager : MonoBehaviour
    {
        public static Action<int, int, List<int>, bool> OnManaChange;

        [Header("References")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private TowerSpawner towerSpawner;

        [Header("Attributes")]
        [SerializeField] private int mana = 100;
        [SerializeField] private int towerCost = 10;
        [SerializeField] private int costIncrease = 10;
        [SerializeField] private List<int> energyLevelCosts;

        public int Mana => mana;
        public int TowerCost => towerCost;

        private int _currentMana;

        private Collider2D _selectedTowerCollider;
        private Tower _selectedTower => _selectedTowerCollider?.transform.parent?.parent.GetComponent<Tower>();
        private Vector3 _selectedTowerOGPosition;
        private Vector3 _selectedTowerOffset;
        private List<int> _energyLevelCostsTowerIndex;
        private IDictionary<string, Tower> _towerNameDict;

        private void Awake()
        {
            _towerNameDict = playerManager.SelectedTowers.Towers.ToDictionary(t => t.name, t => t);
        }

        private void OnEnable()
        {
            Enemy.OnDeath += OnEnemyDeath;
            TowerBattle_UI_Adapter.OnEnergyIncrease += OnTowerEnergyIncrease;
        }

        private void OnDisable()
        {
            Enemy.OnDeath -= OnEnemyDeath;
        }

        private void Start()
        {
            _currentMana = mana;
            _energyLevelCostsTowerIndex = new List<int> { 0, 0, 0, 0, 0 };
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
            if (mana >= towerCost)
            {
                mana -= towerCost;
                towerCost += costIncrease;
                towerSpawner.SpawnRandomFirstTower();
                ManaChange();
            }
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
            mana += incrMana;
            ManaChange();
        }

        private void DecreaseMana(int decrMana)
        {
            mana -= decrMana;
            ManaChange();
        }

        private void ManaChange()
        {
            var energyCosts = _energyLevelCostsTowerIndex.Select(i => energyLevelCosts[i]).ToList();
            OnManaChange?.Invoke(mana, towerCost, energyCosts, towerSpawner.IsFieldFull());
        }

        private void OnTowerEnergyIncrease(string name)
        {
            if (_towerNameDict.ContainsKey(name))
            {
                // update tower
                Tower tower = _towerNameDict[name];
                tower.IncreaseTowerEnergy();

                // update this & UI
                int index = playerManager.SelectedTowers.Towers.IndexOf(tower);
                int decrManaAmt = energyLevelCosts[_energyLevelCostsTowerIndex[index]];
                _energyLevelCostsTowerIndex[index]++;
                string debugOut = string.Join(',', _energyLevelCostsTowerIndex);
                Debug.Log($"{name} {index} {decrManaAmt} {debugOut}");
                DecreaseMana(decrManaAmt);
            }
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
                var towerPointIndex = towerSpawner.DespawnTower(otherTower);
                towerSpawner.SpawnRandomTowerAtPointOfMergeLevel(towerPointIndex, ++selectedMergeLevel);
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
    }
}
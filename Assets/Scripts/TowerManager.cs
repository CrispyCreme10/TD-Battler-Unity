using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerManager : MonoBehaviour
{

    public static Action<int, int> OnManaChange;

    [Header("References")]

    [Header("Attributes")]
    [SerializeField] private int mana = 100;
    [SerializeField] private int towerCost = 10;
    [SerializeField] private int costIncrease = 10;

    public int Mana => mana;
    public int TowerCost => towerCost;

    private int _currentMana;
    private TowerSpawner _towerSpawner;
    private Transform _selectedTowerCollider;
    private Transform _selectedTower => _selectedTowerCollider?.parent?.parent;
    private Vector3 _selectedTowerOGPosition;
    private Vector3 _selectedTowerOffset;

    private void OnEnable()
    {
        Enemy.OnDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        Enemy.OnDeath -= OnEnemyDeath;
    }

    private void Start()
    {
        _currentMana = mana;
        _towerSpawner = GameObject.Find("Towers").GetComponent<TowerSpawner>();
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D[] results = Physics2D.OverlapPointAll(mousePosition);
            if (results.Length > 0)
            {
                foreach(var result in results)
                {
                    if (result.name == "Merge")
                    {
                        _selectedTowerCollider = result.transform;
                        _selectedTowerCollider.gameObject.SetActive(false);
                        _selectedTower.GetComponent<Tower>().enabled = false;
                        _selectedTowerOGPosition = _selectedTower.position;
                        _selectedTowerOffset = _selectedTower.position - mousePosition;
                    }
                }
            }
        }

        if (_selectedTower)
        {
            _selectedTower.position = mousePosition + _selectedTowerOffset;

            Collider2D[] results = Physics2D.OverlapPointAll(mousePosition);
            // if we are dragged over a tower of the same name + merge level, then we can show highlight of tower under mouse
            if (results.Any(c => c.name == "Merge"))
            {
                Transform tower = results.SingleOrDefault(c => c.name == "Merge")?.transform?.parent?.parent;
                if (_selectedTower.name.Split('(')[0] == tower.name.Split('(')[0])
                {
                    Debug.Log(tower);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && _selectedTower)
        {
            // if we are dragged over a tower of the same name + merge level, then we can merge
            // else, send tower back to original position
            Collider2D[] results = Physics2D.OverlapPointAll(mousePosition);
            Transform tower = results.SingleOrDefault(c => c.name == "Merge")?.transform?.parent?.parent;
            if (tower != null && _selectedTower.name.Split('(')[0] == tower.name.Split('(')[0])
            {
                // --perform merge
                // destroy both merged towers
                // release the tower point that the selected tower was on
                // create a random tower of merge level + 1 at the merge location
                int mergeLevel = _selectedTower.GetComponent<Tower>().MergeLevel;
                _towerSpawner.DespawnTower(_selectedTower.gameObject);
                var towerPointIndex = _towerSpawner.DespawnTower(tower.gameObject);
                _towerSpawner.SpawnRandomTowerAtPointOfMergeLevel(towerPointIndex, mergeLevel + 1);
                
            }
            else
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
            _towerSpawner.SpawnRandomFirstTower();
            ManaChange();
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        IncreaseMana(enemy.DeathCoinReward);
    }

    private void IncreaseMana(int incrMana)
    {
        mana += incrMana;
        ManaChange();
    }

    private void ManaChange()
    {
        OnManaChange?.Invoke(mana, towerCost);
    }
}

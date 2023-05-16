using System;
using System.Collections;
using System.Collections.Generic;
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

    public void SpawnTower()
    {
        if (mana >= towerCost)
        {
            mana -= towerCost;
            towerCost += costIncrease;
            _towerSpawner.SpawnTower();
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

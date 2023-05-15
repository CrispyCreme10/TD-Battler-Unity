using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerBattle : MonoBehaviour
{

    private VisualElement _root;
    private Button _spawnBtn;
    private TowerManager _towerManager;

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _towerManager = GameObject.Find("TowerManager").GetComponent<TowerManager>();
        _spawnBtn = _root.Q<Button>("SpawnBtn");

        SetSpawnBtnText(_towerManager.Mana);

        _spawnBtn.clicked += OnSpawn;
        TowerManager.OnManaChange += OnManaChange;
    }

    private void OnSpawn()
    {
        _towerManager.SpawnTower();
    }

    private void OnManaChange(int mana, int towerCost)
    {
        SetSpawnBtnText(mana);
        _spawnBtn.SetEnabled(mana >= towerCost);
    }

    private void SetSpawnBtnText(int mana)
    {
        _spawnBtn.text = mana.ToString();
    }
}

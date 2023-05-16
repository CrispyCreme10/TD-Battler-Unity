using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerBattle : MonoBehaviour
{

    private VisualElement _root;
    private Label _roundTimer;
    private Label _manaLabel;
    private Button _spawnBtn;
    private TowerManager _towerManager;

    private void OnEnable()
    {
        Spawner.OnTimerChanged += SetRoundTimer;
    }

    private void OnDisable()
    {
        Spawner.OnTimerChanged -= SetRoundTimer;
    }

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _towerManager = GameObject.Find("TowerManager").GetComponent<TowerManager>();
        _roundTimer = _root.Q<Label>("RoundTimer");
        _manaLabel = _root.Q<Label>("ManaLabel");
        _spawnBtn = _root.Q<Button>("SpawnBtn");

        SetManaLabel(_towerManager.Mana);
        SetSpawnBtnText(_towerManager.TowerCost);

        _spawnBtn.clicked += OnSpawn;
        TowerManager.OnManaChange += OnManaChange;
    }

    private void OnSpawn()
    {
        _towerManager.SpawnTower();
    }

    private void OnManaChange(int mana, int towerCost)
    {
        SetManaLabel(mana);
        SetSpawnBtnText(towerCost);
        _spawnBtn.SetEnabled(mana >= towerCost);
    }

    private void SetRoundTimer(float startingTime, float timeInSeconds)
    {
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        _roundTimer.text = $"0{Mathf.FloorToInt(timeInSeconds / 60)}:{(seconds < 10 ? '0' + seconds.ToString() : seconds)}";
    }

    private void SetManaLabel(int mana)
    {
        _manaLabel.text = mana.ToString();
    }

    private void SetSpawnBtnText(int towerCost)
    {
        _spawnBtn.text = towerCost.ToString();
    }
}

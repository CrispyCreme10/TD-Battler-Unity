using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerBattle : MonoBehaviour
{

    public Action<int> OnEnergyIncrease;

    private VisualElement _root;
    private Label _roundTimer;
    private Label _manaLabel;
    private Button _spawnBtn;
    private List<Button> _energyButtons;
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
        _energyButtons = _root.Q<VisualElement>("MergeBtns").Children().Select(x => (Button)x).ToList();

        SetManaLabel(_towerManager.Mana);
        SetSpawnBtnText(_towerManager.TowerCost);
        for(int i = 0; i < _energyButtons.Count; i++)
        {
            switch(i)
            {
                case 0:
                    _energyButtons[i].clicked += IncreaseEnergyLevel1;
                    break;
                case 1:
                    _energyButtons[i].clicked += IncreaseEnergyLevel2;
                    break;
                case 2:
                    _energyButtons[i].clicked += IncreaseEnergyLevel3;
                    break;
                case 3:
                    _energyButtons[i].clicked += IncreaseEnergyLevel4;
                    break;
                case 4:
                    _energyButtons[i].clicked += IncreaseEnergyLevel5;
                    break;
                default:
                    break;
            }
        }


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

    private void IncreaseEnergyLevel1()
    {
        IncreaseEnergyLevel(0);
    }

    private void IncreaseEnergyLevel2()
    {
        IncreaseEnergyLevel(1);
    }

    private void IncreaseEnergyLevel3()
    {
        IncreaseEnergyLevel(2);
    }

    private void IncreaseEnergyLevel4()
    {
        IncreaseEnergyLevel(3);
    }

    private void IncreaseEnergyLevel5()
    {
        IncreaseEnergyLevel(4);
    }

    private void IncreaseEnergyLevel(int index)
    {
        OnEnergyIncrease?.Invoke(index);
    }
}

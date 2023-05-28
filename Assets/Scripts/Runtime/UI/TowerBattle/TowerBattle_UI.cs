using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace TDBattler.Runtime
{
    public class TowerBattle_UI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TowerBattle_UI_Adapter adapter;

        private VisualElement _root;
        private Label _roundTimer;
        private VisualElement _enemyCountContainer;
        private IMGUIContainer _heart1Img;
        private IMGUIContainer _heart2Img;
        private IMGUIContainer _heart3Img;
        private Label _manaLabel;
        private Button _spawnBtn;
        private Button _energyButton1;
        private Button _energyButton2;
        private Button _energyButton3;
        private Button _energyButton4;
        private Button _energyButton5;
        private Label _energyLabel1;
        private Label _energyLabel2;
        private Label _energyLabel3;
        private Label _energyLabel4;
        private Label _energyLabel5;

        private List<IMGUIContainer> _hearts;

        // sprites
        Sprite _emptyHeart;
        Sprite _filledHeart;
        Sprite _monsterIcon;

        private void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _roundTimer = _root.Q<Label>("RoundTimer");
            _enemyCountContainer = _root.Q<VisualElement>("EnemyCount");
            _heart1Img = _root.Q<IMGUIContainer>("Heart1");
            _heart2Img = _root.Q<IMGUIContainer>("Heart2");
            _heart3Img = _root.Q<IMGUIContainer>("Heart3");
            _manaLabel = _root.Q<Label>("ManaLabel");
            _spawnBtn = _root.Q<Button>("SpawnBtn");
            _energyButton1 = _root.Q<Button>("Tower1Btn");
            _energyButton2 = _root.Q<Button>("Tower2Btn");
            _energyButton3 = _root.Q<Button>("Tower3Btn");
            _energyButton4 = _root.Q<Button>("Tower4Btn");
            _energyButton5 = _root.Q<Button>("Tower5Btn");
            _energyLabel1 = _root.Q<Label>("Tower1Label");
            _energyLabel2 = _root.Q<Label>("Tower2Label");
            _energyLabel3 = _root.Q<Label>("Tower3Label");
            _energyLabel4 = _root.Q<Label>("Tower4Label");
            _energyLabel5 = _root.Q<Label>("Tower5Label");

            _hearts = new List<IMGUIContainer>() { _heart3Img, _heart2Img, _heart1Img };

            _emptyHeart = ResourceManager.GetSprite("hearts", "hearts_0");
            _filledHeart = ResourceManager.GetSprite("hearts", "hearts_5");
            _monsterIcon = ResourceManager.GetSprite("monster-icon", "monster-icon_0");

            SetManaLabel(adapter.GetMana());
            SetSpawnBtnText(adapter.GetTowerCost());
            SetEnergyButtonsText();
            _energyButton1.clicked += IncreaseEnergyLevel1;
            _energyButton2.clicked += IncreaseEnergyLevel2;
            _energyButton3.clicked += IncreaseEnergyLevel3;
            _energyButton4.clicked += IncreaseEnergyLevel4;
            _energyButton5.clicked += IncreaseEnergyLevel5;

            _spawnBtn.clicked +=  adapter.SpawnTower;
        }

        public void OnManaChange(int mana, int towerCost, List<int> energyCosts, bool fieldIsFull)
        {
            SetManaLabel(mana);
            SetSpawnBtnText(towerCost);
            _spawnBtn.SetEnabled(mana >= towerCost && !fieldIsFull);
            _energyButton1.SetEnabled(mana >= energyCosts[0]);
            _energyButton2.SetEnabled(mana >= energyCosts[1]);
            _energyButton3.SetEnabled(mana >= energyCosts[2]);
            _energyButton4.SetEnabled(mana >= energyCosts[3]);
            _energyButton5.SetEnabled(mana >= energyCosts[4]);
        }

        public void OnLivesChange(int totalLives)
        {
            for (int i = 0; i < _hearts.Count; i++)
            {
                _hearts[i].style.backgroundImage = new StyleBackground(totalLives > i ? _filledHeart : _emptyHeart);
            }
        }

        public void OnEnemiesChange(int totalEnemies)
        {
            if (totalEnemies > _enemyCountContainer.childCount)
            {
                // add
                _enemyCountContainer.Add(CreateMonsterIcon());
            }
            else if (totalEnemies < _enemyCountContainer.childCount)
            {
                // remove
                _enemyCountContainer.RemoveAt(_enemyCountContainer.childCount - 1);
            }
        }

        private IMGUIContainer CreateMonsterIcon()
        {
            IMGUIContainer newMonsterImg = new IMGUIContainer();
            newMonsterImg.style.width = new StyleLength(30);
            newMonsterImg.style.height = new StyleLength(30);
            newMonsterImg.style.backgroundImage = new StyleBackground(_monsterIcon);
            return newMonsterImg;
        }

        public void SetRoundTimer(string text)
        {
            _roundTimer.text = text;
        }

        private void SetManaLabel(int mana)
        {
            _manaLabel.text = mana.ToString();
        }

        private void SetSpawnBtnText(int towerCost)
        {
            _spawnBtn.text = towerCost.ToString();
        }

        private void SetEnergyButtonsText()
        {
            var names = adapter.GetTowerNames().ToList();
            if (names.Count > 0)
            {
                _energyButton1.text = names[0];
            }
            if (names.Count > 1)
            {
                _energyButton2.text = names[1];
            }
            if (names.Count > 2)
            {
                _energyButton3.text = names[2];
            }
            if (names.Count > 3)
            {
                _energyButton4.text = names[3];
            }
            if (names.Count > 4)
            {
                _energyButton5.text = names[4];
            }
        }

        private void IncreaseEnergyLevel1()
        {
            int newLevel = int.Parse(_energyLabel1.text);
            _energyLabel1.text = (++newLevel).ToString();
            IncreaseEnergyLevel(_energyButton1.text);
        }

        private void IncreaseEnergyLevel2()
        {
            int newLevel = int.Parse(_energyLabel2.text);
            _energyLabel2.text = (++newLevel).ToString();
            IncreaseEnergyLevel(_energyButton2.text);
        }

        private void IncreaseEnergyLevel3()
        {
            int newLevel = int.Parse(_energyLabel3.text);
            _energyLabel3.text = (++newLevel).ToString();
            IncreaseEnergyLevel(_energyButton3.text);
        }

        private void IncreaseEnergyLevel4()
        {
            int newLevel = int.Parse(_energyLabel4.text);
            _energyLabel4.text = (++newLevel).ToString();
            IncreaseEnergyLevel(_energyButton4.text);
        }

        private void IncreaseEnergyLevel5()
        {
            int newLevel = int.Parse(_energyLabel5.text);
            _energyLabel5.text = (++newLevel).ToString();
            IncreaseEnergyLevel(_energyButton5.text);
        }

        private void IncreaseEnergyLevel(string towerName)
        {
            adapter.IncreaseEnergyLevel(towerName);
        }
    }
}
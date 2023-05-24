using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Health Upgrades", fileName = "New Health Upgrades")]
public class HealthUpgrades_SO : ScriptableObject
{
    public Action OnHealthIncrease;
    [SerializeField] private List<HealthUpgrade> healthUpgrades;
    [ReadOnly]
    [SerializeField]
    private int spawnHealth;

    public int SpawnHealth => spawnHealth;

    private void OnEnable()
    {
        spawnHealth = healthUpgrades[0].InitHealth;
    }

    public void SetCurrentHealth(float initialTime, float timeRemaining, int wave)
    {
        HealthUpgrade currHealthUpgrade = healthUpgrades[wave - 1];
        float timeDelta = initialTime - timeRemaining;
        int timeBetweenChange = currHealthUpgrade.TimeFrequency;
        int flatTime = Mathf.FloorToInt(timeDelta / timeBetweenChange);
        var newHealth = currHealthUpgrade.InitHealth + (currHealthUpgrade.HealthIncrement * flatTime);
        if (newHealth > spawnHealth)
        {
            spawnHealth = currHealthUpgrade.InitHealth + (currHealthUpgrade.HealthIncrement * flatTime);
            OnHealthIncrease?.Invoke();
        }
    }
}

[Serializable]
public class HealthUpgrade
{
    [SerializeField] private int timeFrequency = 10;
    [SerializeField] private int initHealth = 0;
    [SerializeField] private int healthIncrement = 100;

    public int TimeFrequency => timeFrequency;
    public int InitHealth => initHealth;
    public int HealthIncrement => healthIncrement;
}
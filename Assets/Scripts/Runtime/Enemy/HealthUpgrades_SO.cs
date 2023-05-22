using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Health Upgrades", fileName = "New Health Upgrades")]
public class HealthUpgrades_SO : ScriptableObject
{
    public Action<int> OnHealthIncrease;
    [SerializeField] private List<HealthUpgrade> healthUpgrades;

    public int GetCurrentHealth(float initialTime, float timeRemaining, int wave)
    {
        HealthUpgrade currHealthUpgrade = healthUpgrades[wave - 1];
        float timeDelta = initialTime - timeRemaining;
        int timeBetweenChange = currHealthUpgrade.TimeFrequency;
        int flatTime = Mathf.FloorToInt(timeDelta / timeBetweenChange);
        return currHealthUpgrade.InitHealth + (currHealthUpgrade.HealthIncrement * flatTime);
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
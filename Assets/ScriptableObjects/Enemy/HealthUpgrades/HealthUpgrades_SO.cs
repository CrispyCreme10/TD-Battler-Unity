using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Health Upgrades", fileName = "New Health Upgrades")]
public class HealthUpgrades_SO : ScriptableObject
{
    [SerializeField] private int timeBetweenChange = 10;
    [SerializeField] private List<HealthUpgrades> healthUpgrades;

    private bool waitUntilChange;
    private int currentIndex;
    private int currentHealth;
    private int currentWave;

    private void OnEnable()
    {
        waitUntilChange = false;
        currentIndex = -1;
        currentHealth = 69;
        currentWave = 1;
    }

    public int GetCurrentHealth(float initialTime, float timeRemaining, int wave)
    {
        float timeDelta = initialTime - timeRemaining;
        int flatTime = Mathf.FloorToInt(timeDelta / timeBetweenChange);
        if (flatTime > currentIndex)
        {
            currentIndex = flatTime;
            waitUntilChange = true;
        }

        if (currentIndex < healthUpgrades[currentWave - 1].Health.Count && waitUntilChange)
        {
            currentHealth = GetHealthByWave(wave, currentIndex);
            waitUntilChange = false;
        }

        return currentHealth;
    }

    public int GetHealthByWave(int wave, int index)
    {
        return healthUpgrades[wave - 1].Health[index];
    }
}

[Serializable]
public class HealthUpgrades
{
    [SerializeField] private int wave;
    [SerializeField] private List<int> health;

    public int Wave => wave;
    public List<int> Health => health;
}
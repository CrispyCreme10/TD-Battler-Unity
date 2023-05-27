using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    [CreateAssetMenu(menuName = "Player/Player Towers")]
    public class PlayerTowers : SerializedScriptableObject
    {
        [AssetList(Path = "/Prefabs/Towers", Tags = "Tower")]
        [SerializeField] private List<GameObject> towerPrefabs;

        public List<GameObject> Towers => towerPrefabs;

        public IEnumerable<string> TowerNames()
        {
            return towerPrefabs.Select(t => t.GetComponent<Tower>().TowerData.Name);
        }

        public string GetTowerNameAtIndex(int index)
        {
            return TowerNames().ToList()[index];
        }

        public int TowerIndexOf(string name)
        {
            for(int i = 0; i < towerPrefabs.Count; i++)
            {
                if (towerPrefabs[i].name == name)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}

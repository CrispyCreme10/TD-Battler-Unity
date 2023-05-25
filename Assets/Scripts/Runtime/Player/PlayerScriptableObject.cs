using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    [CreateAssetMenu(menuName = "Player/Player Info")]
    public class PlayerScriptableObject : SerializedScriptableObject
    {
        [SerializeField] private int criticalStrike = 200;
        [SerializeField] private PlayerTowers selectedTowers;
        [SerializeField] private PlayerTowers availableTowers;

        public PlayerTowers SelectedTowers => selectedTowers;
    }
}

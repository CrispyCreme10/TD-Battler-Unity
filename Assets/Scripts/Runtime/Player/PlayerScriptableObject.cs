using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    [CreateAssetMenu(menuName = "Player/Player Info")]
    public class PlayerScriptableObject : SerializedScriptableObject
    {
        [SerializeField] private List<Tower> _selectedTowers;
        [SerializeField] private List<Tower> _availableTowers;

        public List<Tower> SelectedTowers => _selectedTowers;
    }
}

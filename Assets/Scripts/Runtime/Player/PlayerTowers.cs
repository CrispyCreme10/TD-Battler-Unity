using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    [CreateAssetMenu(menuName = "Player/Player Towers")]
    public class PlayerTowers : SerializedScriptableObject
    {
        [SerializeField] private List<Tower> towers;

        public List<Tower> Towers => towers;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    [CreateAssetMenu(menuName = "Player/Player Towers")]
    public class PlayerTowers : ScriptableObject
    {
        [SerializeField] private List<Tower> towers;

        public List<Tower> Towers => towers;
    }
}

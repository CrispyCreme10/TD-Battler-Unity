using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    // responsible for handling the state of player related data
    public class PlayerManager : MonoBehaviour
    {
        // relevant things
        // towers
        // hero
        // emoticons

        [SerializeField] private PlayerScriptableObject player;

        public PlayerTowers SelectedTowers => player.SelectedTowers;
    }

}
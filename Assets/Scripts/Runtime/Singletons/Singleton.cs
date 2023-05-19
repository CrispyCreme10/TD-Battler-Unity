using UnityEngine;

namespace TDBattler.Runtime
{
    public class Singleton : MonoBehaviour
    {
        public static Singleton Instance { get; private set; }
        public UIManager UIManager { get; private set; }
        public PlayerManager PlayerManager { get; private set; }

        private void Awake()
        {
            if (Instance != null & Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            UIManager = GetComponentInChildren<UIManager>();
            PlayerManager = GetComponentInChildren<PlayerManager>();
        }
    }
}
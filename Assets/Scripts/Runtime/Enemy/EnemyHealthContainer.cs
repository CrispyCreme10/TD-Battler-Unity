using TMPro;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class EnemyHealthContainer : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;

        public void SetHealthText(int newHealth)
        {
            textComponent.text = newHealth.ToString();
        }
    }
}
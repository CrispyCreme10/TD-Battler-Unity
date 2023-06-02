using TMPro;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class EnemyHealthContainer : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;

        public void SetHealthText(int newHealth)
        {
            // format health
            if (newHealth >= 1000)
            {
                textComponent.text = (newHealth / 1000).ToString() + "k";
                return;
            }

            textComponent.text = newHealth.ToString();
        }
    }
}
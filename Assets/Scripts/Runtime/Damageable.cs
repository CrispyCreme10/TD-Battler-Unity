using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class Damageable : MonoBehaviour
    {
        public Action AfterDamage;
        public int Damage { get; private set; }

        public void SetDamage(float damage)
        {
            Damage = (int)damage;
        }

        public void PerformedDamage()
        {
            AfterDamage?.Invoke();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class Damageable : MonoBehaviour
    {
        public static Action<string, int> OnAfterDamageGlobal;
        public Action AfterDamage;
        public int Damage { get; private set; }

        public void SetDamage(float damage)
        {
            Damage = (int)damage;
        }

        public void PerformedDamage()
        {
            AfterDamage?.Invoke();
            Projectile projectile = GetComponent<Projectile>();
            if (projectile == null) return;
            OnAfterDamageGlobal?.Invoke(projectile.SourceTowerName, Damage);
        }
    }
}

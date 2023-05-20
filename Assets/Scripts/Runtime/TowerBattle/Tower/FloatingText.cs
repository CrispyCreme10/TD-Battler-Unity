using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(-2, 1, 0);

        private void Start()
        {
            transform.localPosition += offset;
        }
    }
}

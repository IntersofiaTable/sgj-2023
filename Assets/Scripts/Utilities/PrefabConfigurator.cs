using System;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    [ExecuteInEditMode]
    public class PrefabConfigurator : MonoBehaviour
    {
        public void OnValidate()
        {
            if (transform.childCount == 1)
            {
                transform.name = transform.GetChild(0).name;
                transform.GetChild(0).transform.localPosition = Vector3.zero;
            }
        }

        public void Update()
        {
            OnValidate();
        }
    }
}
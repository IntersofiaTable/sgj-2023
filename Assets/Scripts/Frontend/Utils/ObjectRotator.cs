using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.Scripts.Frontend.Utils
{
    internal class ObjectRotator : MonoBehaviour
    {
        public float Speed = 15f;

        public bool On;

        public void Update()
        {
            if (On)
            {
                transform.Rotate(0, Speed, 0);
            }
        }
    }
}

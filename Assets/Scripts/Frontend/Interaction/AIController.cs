using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Frontend.Interaction
{
    internal class AIController : MonoBehaviour
    {
        public int HP;

        public static AIController Instance;

        private void Start()
        {
            Instance = this;
        }

        public async UniTask SetNewHP(int hp)
        {
            Debug.Log($"hp is {hp}");
        }
    }
}

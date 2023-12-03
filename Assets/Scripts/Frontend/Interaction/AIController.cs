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

        public int MaxHP = 0;

        public float HealthPercentage => HP / (float)MaxHP;
        
        public static AIController Instance;

        public AIHealthBar healthBar;
        
        private void Start()
        {
            Instance = this;
        }

        public async UniTask SetNewHP(int hp)
        {
            if (MaxHP == 0)
            {
                MaxHP = hp;
            }
            HP = hp;
            healthBar.LerpToState(HealthPercentage);
            Debug.Log($"hp is {hp}");
        }
    }
}

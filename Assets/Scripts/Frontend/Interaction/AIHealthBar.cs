using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Frontend.Interaction
{
    public class AIHealthBar : MonoBehaviour
    {
        public Scrollbar scrollbar;

        public void LerpToState(float fNorm)
        {
            DOTween.To(() => scrollbar.size, (val) => { scrollbar.size = val; }, fNorm, 0.3f);
        }
    }
}
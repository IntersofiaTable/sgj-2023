using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Assets.Scripts.Frontend.Interaction
{
    public class EndVideoStuff : MonoBehaviour
    {
        public VideoPlayer vp;

        private void Start()
        {
            vp.loopPointReached += source =>
            {
                SceneManager.LoadScene("SampleScene");

            };
        }
    }
}
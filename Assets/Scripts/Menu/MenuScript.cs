using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadIntro()
    {
        SceneManager.LoadScene("Intro");
    }
}

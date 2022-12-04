using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // the only purpose of this scene/button/script is to
    // load the actual game scene.

    public void BeginGame()
    {
        SceneManager.LoadScene("Prototype 3");
    }
}

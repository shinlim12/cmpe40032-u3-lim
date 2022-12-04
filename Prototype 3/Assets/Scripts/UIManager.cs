using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    /** BEGIN SINGLETON DECLARATION **/

    private static UIManager _instance;
    public static UIManager Instance 
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The UIManager doesn't exist!");
            }
            return _instance;
        }
    }

    void Awake() => _instance = this;
    /** END SINGLETON DECLARATION **/

    public GameObject scoreDisplay;
    public GameObject finalScoreDisplay;
    public GameObject restartButton;

    // UI manager only needs to know about GameOver and GameRestart
    // events, and switches which elements are displayed accordingly

    private void Start()
    {
        StartUI();
        PlayerController.PlayerHitObstacle += GameOver;
        GameManager.GameRestart += StartUI;
    }

    // when the game is started or restarted, we make sure the death
    // screen is deactivated, and the basic score display is activated

    private void StartUI()
    {
        scoreDisplay.SetActive(true);
        finalScoreDisplay.SetActive(false);
        restartButton.SetActive(false);
    }

    // score is 'how many obstacles we've avoided.' score is kept in
    // GameManager and updated whenever an obstacle fires its own
    // 'despawn' event, which is caused by the obstacle leaving the
    // screen and going out of bounds

    private void Update()
    {
       scoreDisplay.gameObject.GetComponent<Text>().text = "Score: "
            + GameManager.Instance.score;
    }
    

    private void GameOver()
    {
        StartCoroutine("DeathScreen");
    }

    // this is a coroutine that waits a couple of seconds after death, hides
    // the score display at the top, and displays the final score in the middle
    // of the screen along with a restart button

    // the slight delay is there both for fatalistic 'well, darn, I died'
    // flavor and to let the death animation finish playing.

    // without this delay, it's possible for the player to press 'restart' fast
    // enough for silly things to happen, such as the player character sliding
    // back onscreen in the 'intro' but also still in his dying animation.

    IEnumerator DeathScreen()
    {
        yield return new WaitForSeconds(4);
        scoreDisplay.SetActive(false);
        finalScoreDisplay.SetActive(true);
        finalScoreDisplay.GetComponent<Text>().text = "Final Score\n"
            + GameManager.Instance.score;
        restartButton.SetActive(true);
    }
}

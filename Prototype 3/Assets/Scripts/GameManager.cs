using UnityEngine;

public class GameManager : MonoBehaviour
{
    /** BEGIN SINGLETON DECLARATION **/

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The GameManager doesn't exist!");
            }
            return _instance;
        }
    }

    private void Awake() => _instance = this;
    /** END SINGLETON DECLARATION **/

    /** GLOBAL CONSTANTS **/
    
    // I really dislike the way having strings right in my function calls
    // looks, so I prefer to use constants for things like this - helps
    // enforce consistency and reduce issues with potential typos, too

    public const string ANIM_JUMP_TRIG = "Jump_trig";
    public const string ANIM_DEATH_B = "Death_b";
    public const string ANIM_SPEED_F = "Speed_f";
    public const string TAG_WALKABLE = "Walkable";
    public const string TAG_OBSTACLE = "Obstacle";
    public const string TAG_PLAYER = "Player";
    public const string STATIC_B = "Static_b";

    /** END GLOBAL CONSTANTS **/

    public int score;
    public bool isGameStopped;
    public delegate void RestartAction();
    public static event RestartAction GameRestart;

    // in a perfect world, i probably wouldn't need the below bool, but
    // i'm running out of time and it's a quick hack

    public bool playerIsDashing;

    // lots of events to subscribe to here - we want to know when the
    // player is dashing so that we know whether to enhance their score,
    // when they've finished the intro animation so we can initialize
    // the game, and when they've hit an obstacle so we can stop the game.

    // we also want to know when an object has despawned itself, as that is
    // the condition that causes the score to increase

    void Start()
    {
        score = 0;
        isGameStopped = true;
        PlayerController.PlayerStartDashing += EnhanceScore;
        PlayerController.PlayerStopDashing += DeEnhanceScore;
        PlayerController.PlayerFinishedIntro += FinishedIntro;
        PlayerController.PlayerHitObstacle += GameOver;
        MoveLeft.Despawn += IncreaseScore;
    }

    // IncreaseScore looks at playerIsDashing, so we want the value
    // to change accordingly based on what the player is doing

    private void EnhanceScore() => playerIsDashing = true;
    private void DeEnhanceScore() => playerIsDashing = false;

    private void GameOver() => isGameStopped = true;

    // clearing obstacles is worth double if the player is currently
    // in dash mode

    private void IncreaseScore()
    {
        if (playerIsDashing)
        {
            score += 2;
        }
        else if (!playerIsDashing)
        {
            score++;
        }
    }

    // MoveLeft asks about isGameStopped to know whether it needs to currently
    // be moving or not. FinishedIntro() is fired when the player character
    // has finished the 'walk in' sequence. The background and obstacles will
    // know to start moving now.

    public void FinishedIntro()
    {
        isGameStopped = false;
    }


    // This is fired when the Restart button is pressed. Score is reset,
    // we temporarily stop the game so that the intro can replay, and we
    // let event subscribers know to smash that like button-- I mean
    // re-initialize their states to the start of the game.

    public void RestartGame()
    {
        score = 0;
        isGameStopped = true;
        GameRestart?.Invoke();
    }
}

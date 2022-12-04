using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    // we have this event so that the GameManager can know
    // an object has despawned; despawning an object causes
    // the GameManager to increase the score.
    public delegate void DespawnEvent();
    public static event DespawnEvent Despawn;

    // there are two movespeed variables because of dash mode.

    private float moveSpeed;
    private float modifiedMoveSpeed;

    // we need to know about the player going into and out of
    // dash mode, so we subscribe to those two events.

    void Start()
    {
        moveSpeed = 20f;
        modifiedMoveSpeed = moveSpeed;
        PlayerController.PlayerStartDashing += SpeedUp;
        PlayerController.PlayerStopDashing += SlowDown;
    }

    // since this script is attached to both the background
    // and the obstacles, it has to act slightly differently for
    // each. for obstacles, when they are enabled, they are placed
    // off of the right side of the screen

    void OnEnable()
    {
        if (gameObject.tag == GameManager.TAG_OBSTACLE)
        {
            transform.position = new Vector3(25, 0, 0);
            transform.rotation = Quaternion.identity;
        }
    }

    // these are fired when the player uses dash mode.
    // we will move the background and objects twice as
    // fast when the player is dashing

    void SpeedUp() => modifiedMoveSpeed = moveSpeed * 2f;
    void SlowDown() => modifiedMoveSpeed = moveSpeed;

    void FixedUpdate()
    {
        // do nothing if the game is stopped

        if (GameManager.Instance.isGameStopped)
        {
            return;
        }
        else if (!GameManager.Instance.isGameStopped)
        {
            // set our modifiedMoveSpeed according to whether the player
            // is currently dashing or not
            if (GameManager.Instance.playerIsDashing)
            {
                modifiedMoveSpeed = moveSpeed * 2f;
            }
            else
            {
                modifiedMoveSpeed = moveSpeed;
            }

            // do our actual movement, with our modifiedMoveSpeed applied
            transform.Translate(Vector3.left * Time.fixedDeltaTime * modifiedMoveSpeed);

            // finally, if we're an obstacle and we've gone out of bounds,
            // despawn self and notify anyone who is interested in this
            // event (specifically, the GameManager)

            if (gameObject.tag == GameManager.TAG_OBSTACLE && (transform.position.x < -2 || transform.position.y < -2))
            {
                Despawn?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}

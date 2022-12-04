using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    private Vector3 startPos;

    // subscribe to GameManager event at the beginning so
    // we know when we're supposed to restart

    void Start()
    {
        startPos = transform.position;
        GameManager.GameRestart += ResetBackground;
    }

    // this event fires when GameManager restarts, placing the background
    // back to its starting position.

    void ResetBackground() => transform.position = startPos;

    void Update()
    {
        // we're being moved by the MoveLeft script, but because MoveLeft also
        // gets applied to obstacles, I wanted to make resetting the background
        // a different script.
        //
        // reset position if we've moved further than half our background width

        if (transform.position.x < startPos.x - gameObject.GetComponent<SpriteRenderer>().bounds.extents.x)
        {
            transform.position = startPos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    /** BEGIN SINGLETON DECLARATION **/
    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("SpawnManager doesn't exist!");
            }
            return _instance;
        }
    }

    void Awake() => _instance = this;
    /** END SINGLETON DECLARATION **/

    // there are three objects that we CAN make obstacles out of,
    // and then an object pool that we assign them to.

    // the object pool exists as a way to save on garbage collection
    // by reusing the same GameObjects over and over again.

    public GameObject[] obstaclePrefabs = new GameObject[3];
    private List<GameObject> obstaclePool = new List<GameObject>();


    // we need to subscribe to three events: one to tell us when to
    // prepare the object pool, one to tell us when the player intro
    // has finished and the player is now actively running, and one to
    // tell us when the player has died

    // when the game has started or restarted, we initialize the object
    // pool, making a randomized list so that there is a slightly different
    // ratio of object variety every time.
    //
    // when the player begins running, that's when we begin spawning
    // objects.
    // 
    // when the game is over, we disable all obstacles and do nothing until
    // the game restarts

    private void Start()
    {
        InitializePool();
        GameManager.GameRestart += InitializePool;
        PlayerController.PlayerFinishedIntro += StartSpawner;
        PlayerController.PlayerHitObstacle += GameOver;
    }

    // start with ten objects in the pool. if the player makes it far enough
    // for more than ten objects to want to spawn in the play area at once,
    // we'll make more objects and add them to the pool as we need them

    private void InitializePool()
    {
        for (int i = 1; i <= 10; i++)
        {
            InitializeObstacle(false);
        }
    }

    // this is fired when the player has finished the intro, and it starts
    // a coroutine that runs until the player dies

    private void StartSpawner()
    {
        StartCoroutine("CycleObstacles");
    }

    // this is fired when the player hits an obstacle; it destroys all
    // objects in the pool so we can create a slightly different pool for
    // the next round

    private void GameOver()
    {
        StopCoroutine("CycleObstacles");
        foreach (GameObject _obstacle in obstaclePool)
        {
            Destroy(_obstacle);
        }
        obstaclePool.Clear();
    }

    // this coroutine spawns the obstacles. as the player's score gets
    // higher, the lower and upper limits of the spawn timer become lower
    // and tighter. the game becomes pretty much impossible fairly quickly,
    // these numbers would be more balanced in a proper game that you'd want
    // to actually sell

    private IEnumerator CycleObstacles()
    {
        float _lowerFuzz;
        float _upperFuzz;
        do
        {
            switch (GameManager.Instance.score)
            {
                case int score when score < 5:
                    _lowerFuzz = 2f;
                    _upperFuzz = 3f;
                    break;
                case int score when score < 10:
                    _lowerFuzz = 1f;
                    _upperFuzz = 2.5f;
                    break;
                case int score when score < 20:
                    _lowerFuzz = 0.5f;
                    _upperFuzz = 1.5f;
                    break;
                case int score when score < 30:
                    _lowerFuzz = 0.4f;
                    _upperFuzz = 0.9f;
                    break;
                case int score when score < 40:
                    _lowerFuzz = 0.3f;
                    _upperFuzz = 0.8f;
                    break;
                default:
                    _lowerFuzz = 0.2f;
                    _upperFuzz = 0.7f;
                    break;
            }

            // once we have the possible values of our range based on
            // the current score, we get a random float between those
            // values. then we spawn in a new obstacle, and wait for
            // a period of seconds equal to the random float we got.
            // after that time period, this function will run again

            float _timer = Random.Range(_lowerFuzz, _upperFuzz);
            SpawnManager.Instance.PlaceObstacle();
            yield return new WaitForSecondsRealtime(_timer);
        } while (true);
    }

    // create an obstacle using a random selection from the possible prefabs
    // and assign it to the object pool. the value of activeOnInit lets us
    // decide whether the object is immediately active and moving or not.
    //
    // when initializing the pool during the game intro, we make the objects
    // start out disabled, but if we're creating a new obstacle for the pool
    // at the time that the player is running, we'll let the object start out
    // already active.

    private GameObject InitializeObstacle(bool activeOnInit)
    {
        int _choice = Random.Range(0, obstaclePrefabs.Length);
        GameObject _newObstacle = Instantiate(obstaclePrefabs[_choice],
            new Vector3(25, 0, 0), Quaternion.identity, transform);
        obstaclePool.Add(_newObstacle);
        if (!activeOnInit)
        {
            _newObstacle.SetActive(false);
        }
        return _newObstacle;
    }

    // this is a function that randomizes the object pool list. this is used
    // by PlaceObstacle() to make it so that during time periods where our
    // object spawning is infrequent, the player isn't just seeing the exact
    // same create or spool over and over again, and is likely to instead
    // see a variety of objects.

    private List<GameObject> Shuffle(List<GameObject> list)
    {
        List<GameObject> _newList = new List<GameObject>();
        while (list.Count > 0)
        {
            var _newRandom = Random.Range(0, list.Count);
            _newList.Add(list[_newRandom]);
            list.RemoveAt(_newRandom);
        }
        return _newList;
    }

    // pick a currently inactive object from the object pool and activate
    // it. this will cause it to appear from the right of the screen and
    // move towards the player.
    //
    // if we've gotten to a point where all of the objects in our pool are
    // currently in use, we'll create a new one that starts off immediately
    // active.

    private GameObject PlaceObstacle()
    {
        obstaclePool = Shuffle(obstaclePool);

        foreach (GameObject _obstacle in obstaclePool)
        {
            if (_obstacle.activeSelf == false)
            {
                _obstacle.SetActive(true);
                return _obstacle;
            }
        }
        return InitializeObstacle(true);
    }
}

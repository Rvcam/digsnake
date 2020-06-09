using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MyUtil;
using UnityEngine.SceneManagement;

public class PlayerController : SnakePart
{
    #region variables and events
    private Vector3 touchStartPosition;
    private float timeToWalkOwnSize;
    private float timeLastDirectionChange;
    private int initialSize = 0;
    Vector3 oldDirection;
    private int oldSizeDiff;
    private bool directionSet = false;
    private bool teleporting;
    [SerializeField]
    [Range(0.3f, 3.0f)]
    private float teleportingDistance = 3;
    bool paused = false;

    public event Action<bool> Finished;
    public event Action FruitCollected;

    private bool isAccelerating;
    private float speedChangedBy;
    public bool accelerateEnabled = true;

    [SerializeField]
    AudioClip getFruitClip=null;
    [SerializeField]
    AudioClip deathClip=null;
    [SerializeField]
    AudioClip accelerateClip=null;
    AudioSource audioSource;

    #endregion

   
    protected override void Start()
    {
        base.Start();
        if (speed < Mathf.Epsilon)
        {
            print("Speed not set");
        }        
        gameSceneController = FindObjectOfType<GameSceneController>();
        snakeLength++;
        isAccelerating = false;
        speedChangedBy = 0;
        audioSource = GetComponent<AudioSource>();
    }

    #region movement, update and collision

    protected void directionalInput()
    {
        Vector3 newDirection = oldDirection;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            newDirection = new Vector3(1, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            newDirection = new Vector3(-1, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            newDirection = new Vector3(0, 1, 0);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            newDirection = new Vector3(0, -1, 0);
        }

        timeToWalkOwnSize = myRenderer.bounds.size.x / speed;
        if (Time.time - timeLastDirectionChange > timeToWalkOwnSize)
        {
            if (newDirection != currentDirection && newDirection != currentDirection * -1.0f)
            {
                currentDirection= newDirection; //if player has not made a move, newDirection = oldDirection
                timeLastDirectionChange = Time.time;
            }
            else
            {
                //invalid move, do nothing
            }
        }
        oldDirection = newDirection;
    }

    protected override void IndependentMove()
    {
        if (teleporting == true)
        {
            myRigidbody.MovePosition(transform.position + currentDirection * teleportingDistance);
            teleporting = false;
        }
        else
        {
            myRigidbody.MovePosition(transform.position + currentDirection * Time.fixedDeltaTime * speed);
        }
    }

    protected void Update()
    {
        if (!gameSceneController.gameOver)
        {
            checkForPause();
            if (!directionSet)
            {
                timeLastDirectionChange = Time.time;
                currentDirection = gameSceneController.getRoomDirectionAsVector(); // vector is used here because of enum. Update that someday
                oldDirection = currentDirection;
                directionSet = true;
            }
            directionalInput();
            if ((snakeLength - initialSize) > oldSizeDiff + 5)
            {
                changeSnakeSpeed(speed + 0.5f);
                oldSizeDiff = snakeLength - initialSize;
            }
            if (isReady())
            {
                transform.right = currentDirection;
            }
            else
            {
                transform.right = gameSceneController.getRoomDirectionAsVector();
            }

            if (accelerateEnabled && Input.GetKeyDown(KeyCode.Space))
            {
                accelerate(3);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                //teleport();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger)
        {
            if (gameSceneController.gameOver == false)
            {
                if (tagger.containsCustomTag("fruit"))
                {
                    audioSource.PlayOneShot(getFruitClip);
                    FruitCollected();
                    Destroy(collision.gameObject);
                    growParts();
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger)
        {
            if (gameSceneController.gameOver == false)
            {
                if (tagger.containsCustomTag("outer game bounds"))
                {
                    die();
                }
            }
        }
    }

    public void onChildObstacleHit()
    {
        die();
    }

    #endregion

    private void die()
    {
        speed = 0;
        if (childScript)
        {
            childScript.transform.Translate(new Vector3(0, 0, -0.1f));
            //childScript.StartCoroutine(childScript.blink(Color.red));
        }
        Finished(false);
        myRenderer.color = new Color(1, 1, 1, 0);
        myLight.intensity = 0;
        audioSource.PlayOneShot(deathClip, 2);
    }

    public void delayedGrowth(int numberToGrow)
    {
        StartCoroutine(coDelayedGrowth(numberToGrow));
    }

    private IEnumerator coDelayedGrowth(int numberToGrow)
    {
        while (isReady()==false)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(2 * myRenderer.bounds.size.x / speed);
        setLength(snakeLength + numberToGrow);
    }

    public bool isReady()
    {
        return currentDirection.magnitude > Mathf.Epsilon && speed > Mathf.Epsilon;
    }

    public void accelerate(float rate)
    {
        
        if (!isAccelerating)
        {
            audioSource.PlayOneShot(accelerateClip);
            float originalSpeed = speed;
            float newSpeed = speed * rate;
            changeSnakeSpeed(newSpeed);
            isAccelerating = true;
            speedChangedBy = newSpeed - originalSpeed;
            StartCoroutine(undoAccelerate(1));
        }
      
    }

    private IEnumerator undoAccelerate(float time)
    {
        yield return new WaitForSeconds(time);
        changeSnakeSpeed(speed - speedChangedBy);
        isAccelerating = false;
    }

    public void teleport()
    {
        teleporting = true;
    }

    private void checkForPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.P))
        {
            paused = !paused;
        }
        if (paused)
        {

            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}

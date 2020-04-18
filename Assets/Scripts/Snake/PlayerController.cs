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
    private bool teleporting;
    [SerializeField]
    [Range(0.3f, 3.0f)]
    private float teleportingDistance=3;

    public event Action<bool> Finished;
    public event Action FruitCollected;

    #endregion
    protected override void Start()
    {
        base.Start();
        if (speed < Mathf.Epsilon)
        {
            print("Speed not set");
        }
        timeToWalkOwnSize = GetComponent<Renderer>().bounds.size.x / speed;
        timeLastDirectionChange = -timeToWalkOwnSize;
        oldDirection = currentDirection;
        gameSceneController = FindObjectOfType<GameSceneController>();
        
    }

    protected void directionalInput()
    {

        Vector3 newDirection = oldDirection;
        if (Input.GetKeyDown("right"))
        {
            newDirection = new Vector3(1, 0, 0);
        }

        if (Input.GetKeyDown("left"))
        {
            newDirection = new Vector3(-1, 0, 0);
        }

        if (Input.GetKeyDown("up"))
        {
            newDirection = new Vector3(0, 1, 0);
        }

        if (Input.GetKeyDown("down"))
        {
            newDirection = new Vector3(0, -1, 0);
        }

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
        directionalInput();
        if ((snakeLength - initialSize) > oldSizeDiff + 5)
        {
            changeSnakeSpeed(speed + 0.5f);
            oldSizeDiff = snakeLength - initialSize;
            timeToWalkOwnSize = myRenderer.bounds.size.x / speed;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            teleporting = true;
        }
        if (gameSceneController.numberOfFruits <= 0 && gameSceneController.gameOver==false)
        {
            //win();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger)
        {
            if (gameSceneController.gameOver == false)
            {
                if (tagger.containsCustomTag("fruit"))
                {
                    FruitCollected();
                    Destroy(collision.gameObject);
                    growParts();
                }
            }
        }
    }
    public void onChildObstacleHit()
    {
        die();
    }

    private void die()
    {
        speed = 0;
        if (childScript)
        {
            childScript.transform.Translate(new Vector3(0, 0, -0.1f));
            childScript.StartCoroutine(childScript.blink(Color.red));
        }
        Finished(false);
        myRenderer.color = new Color(1, 1, 1, 0);
    }

    private void win()
    {
        speed = 0;
        if (childScript)
        {
            childScript.transform.Translate(new Vector3(0, 0, -0.1f));
            childScript.StartCoroutine(childScript.blink(Color.green));
        }
        Finished(true);
        myRenderer.color = new Color(1, 1, 1, 0);
            
    }
}

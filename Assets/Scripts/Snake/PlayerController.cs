using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MyUtil;
public class PlayerController : SnakePart
{
    private Vector3 touchStartPosition;
    private float timeToWalkOwnSize;
    private float timeLastDirectionChange;
    private int initialSize=0;
    Vector3 oldDirection;
    private int oldSizeDiff;
    private bool teleporting;

    protected override void Start()
    {
        base.Start();
        if (speed<Mathf.Epsilon)
        {
            print("Speed not set");
        }
        timeToWalkOwnSize = GetComponent<Renderer>().bounds.size.x/speed;
        timeLastDirectionChange = -timeToWalkOwnSize;
        oldDirection = currentDirection;
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

        /*
        if (Input.touchCount>0)
        {
            Vector3 walkTowards = transform.position;

            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchStartPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                walkTowards = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) - touchStartPosition;

                if (walkTowards.magnitude > 0.5)
                {
                    if (Math.Abs(walkTowards.x) > Math.Abs(walkTowards.y))
                    {
                        if (walkTowards.x > 0)
                        {
                            newDirection = new Vector3(1, 0, 0);
                        }
                        else
                        {
                            newDirection = new Vector3(-1, 0, 0);
                        }
                    }
                    else
                    {
                        if (walkTowards.y > 0)
                        {
                            newDirection = new Vector3(0, 1, 0);
                        }
                        else
                        {
                            newDirection = new Vector3(0, -1, 0);
                        }
                    }
                }
            }
        }
        */

        if (Time.time - timeLastDirectionChange > timeToWalkOwnSize)
        {
            if (newDirection != currentDirection && newDirection != currentDirection * -1.0f)
            {
                currentDirection = newDirection; //if player has not made a move, newDirection = oldDirection
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
        if (teleporting==true)
        {
            myRigidbody.MovePosition(new Vector3(0, 0, transform.position.z));
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
        if ((snakeLength - initialSize) > oldSizeDiff+5 )
        {
            changeSnakeSpeed(speed + 0.5f);
            oldSizeDiff = snakeLength - initialSize;
            timeToWalkOwnSize = myRenderer.bounds.size.x / speed;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            teleporting = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger)
        {
            if (tagger.containsCustomTag("obstacle"))
            {
                Destroy(gameObject);
            }
            else if (tagger.containsCustomTag("fruit"))
            {

                Destroy(collision.gameObject);
                growParts();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : SnakePart
{
    private Vector3 touchStartPosition;
    private float timeToWalkOwnSize;
    private float timeLastDirectionChange;
    Vector3 oldDirection;

    protected override void Start()
    {
        base.Start();
        if (speed == 0)
        {

            speed = 3;
        }
        timeToWalkOwnSize = GetComponent<Renderer>().bounds.size.x/speed;
        timeLastDirectionChange = -timeToWalkOwnSize;
        oldDirection = getDirection();
        //StartCoroutine("spawnTest");
    }

    private IEnumerator spawnTest()
    {
        /*
        while (true)
        {
            if(getDirection().magnitude>Mathf.Epsilon)
                growParts();
            yield return new WaitForSeconds(3);     
        }
        */
        while (getDirection().magnitude <= Mathf.Epsilon)
        {
            yield return new WaitForSeconds(1);
        }
        for (int i=0; i<30; i++)
        {

            growParts();
        }
        yield return new WaitForSeconds(1);
    }

    protected override void DetermineDirection()
    {

        Vector3 newDirection =oldDirection;
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

        if (Time.time - timeLastDirectionChange > timeToWalkOwnSize)
        {
            if (newDirection != getDirection() && newDirection != getDirection() * -1.0f)
            {
                changeIntendedDirection(newDirection); //if player has not made a move, newDirection = oldDirection
                timeLastDirectionChange = Time.time;
            }
            else
            { 
                //invalid move, do nothing
            }
            oldDirection = newDirection; //to make sure we clear oldDirection, it will be ignored next time because it will be equal to getDirection()
        }
        else
        {
            oldDirection = newDirection;
        }

    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("snake part"))
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("fruit"))
        {
            Debug.Log("fruit");
            growParts();
        }
    }
}

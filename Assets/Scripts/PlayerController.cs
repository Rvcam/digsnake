using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : SnakePart
{
    protected override void Start()
    {
        base.Start();
        if (speed == 0)
        {

            speed = 3;
        }
        StartCoroutine("spawnTest");
    }

    private IEnumerator spawnTest()
    {
        while (true)
        {
            growParts();
            yield return new WaitForSeconds(3);
        }
    }

    protected override void DetermineDirection()
    {
        if (Input.GetKeyDown("right"))
        {
            changeDirection( new Vector3(1, 0, 0));
        }

        if (Input.GetKeyDown("left"))
        {
            changeDirection( new Vector3(-1, 0, 0));
        }

        if (Input.GetKeyDown("up"))
        {
            changeDirection( new Vector3(0, 1, 0));
        }

        if (Input.GetKeyDown("down"))
        {
            changeDirection( new Vector3(0, -1, 0));
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) || (Input.touchCount>0))
        {
            Vector3 walkTowards = transform.position;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                walkTowards = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            }
            else if (Input.touchCount > 0)
            {
                walkTowards = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f);
            }

            if  (Math.Abs(walkTowards.x)>Math.Abs(walkTowards.y))
            {
                if (walkTowards.x > 0)
                {
                    changeDirection(new Vector3(1, 0, 0));
                }
                else
                {
                    changeDirection(new Vector3(-1, 0, 0));
                }
            }
            else
            {
                if (walkTowards.y > 0)
                {
                    changeDirection(new Vector3(0, 1, 0));
                }
                else
                {
                    changeDirection(new Vector3(0, -1, 0));
                }
            }
        }
    }

    protected override void Update()
    {
        base.Update();
    }

}

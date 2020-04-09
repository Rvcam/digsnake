using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakePart : MonoBehaviour
{
    [Range(0.1f,2f)]
    public float speed;
    private Vector3 currentDirection;
    private Vector3 intendedDirection;
    [SerializeField]
    [Range(0.05f, 1f)]
    private float gridsize;
    protected GameObject childPart;
    public GameObject partToSpawn;
    private SnakeBodyController partScript;
    public Queue<int> turnPositions;
    protected SnakePart front;
    private int lastTurnSquare;
    private Queue<Vector3> directionsToFollow;

    protected virtual void Move()
    {
        Vector3 moveBy = currentDirection * speed * Time.deltaTime;
        int currentSquare=0;
        int futureSquare=0;

        if (currentDirection.x != 0)
        {
            currentSquare = (int)Math.Floor((transform.position.x) / gridsize);
            futureSquare = (int)Math.Floor((transform.position.x + moveBy.x) / gridsize);
        }
        else if (currentDirection.y != 0)
        {
            currentSquare = (int)Math.Floor((transform.position.y) / gridsize);
            futureSquare = (int)Math.Floor((transform.position.y + moveBy.y) / gridsize);
        }

        if (currentSquare != futureSquare)
        // means we are about to cross from one square to another and, thus, can rotate.
        {

            transform.Translate(moveBy.normalized * (float)(Math.Floor(moveBy.magnitude / gridsize) * gridsize)); //move until reach turning point
            if (front == null || (turnPositions.Count>0 && currentSquare==turnPositions.Peek())) 
            // we are at a turn position
            {
                if (turnPositions.Count > 0)
                {
                    turnPositions.Dequeue();
                    changeDirection(directionsToFollow.Dequeue());
                }
                if (intendedDirection != currentDirection)
                {
                    if (partScript) // it's here so the head only enqueues when changing direction
                    {
                        partScript.turnPositions.Enqueue(currentSquare);
                        partScript.directionsToFollow.Enqueue(intendedDirection);
                    }
                    currentDirection = intendedDirection;

                }
            }

            moveBy = currentDirection * (moveBy.magnitude % gridsize); //move the rest
            transform.Translate(moveBy);           
        }
        else
        {
            transform.Translate(currentDirection * speed * Time.deltaTime);
        }

    }

    protected virtual void DetermineDirection()
    {

    }

    protected virtual void Start()
    {
        if (partToSpawn == null)
        {
            Debug.LogError("Assign a part to spawn to the snake head");
        }
        turnPositions = new Queue<int>();
        directionsToFollow = new Queue<Vector3>();
    }

    protected virtual void Update()
    {
        DetermineDirection();
        Move();
    }

    public Vector3 getDirection()
    {
        return currentDirection;
    }

    protected void changeDirection(Vector3 newDirection)
    {
        if (currentDirection.magnitude < Mathf.Epsilon)
        {
            currentDirection = newDirection;
        }
        intendedDirection = newDirection;

    }

    protected void growParts()
    {
        if (childPart == null)
        {
            childPart = Instantiate(partToSpawn, transform.position+currentDirection*-1.0f * GetComponent<Renderer>().bounds.size.magnitude, Quaternion.identity);
            partScript = childPart.GetComponent<SnakeBodyController>();
            partScript.front = this;
            partScript.speed = speed;
            partScript.partToSpawn = partToSpawn;
            partScript.gridsize = gridsize;
            partScript.changeDirection(getDirection());
        }
        else
        {
            childPart.GetComponent<SnakeBodyController>().growParts();
        }
    }
}

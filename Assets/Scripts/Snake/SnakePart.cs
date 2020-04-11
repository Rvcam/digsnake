using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class SnakePart : MonoBehaviour
{

    #region variables

    [Range(1,5)]
    public int speed;
    private Vector3 currentDirection;
    private Vector3 intendedDirection;
    protected GameObject childPart;
    public GameObject partToSpawn;
    private SnakeBodyController partScript;
    public Queue<Vector3> turnPositions;
    protected SnakePart front;
    private int lastTurnSquare;
    private Queue<Vector3> directionsToFollow;
    private bool isTurning = false;
    private Renderer myRenderer;
    private Rigidbody2D myRigidbody;

    #endregion

    #region movement
    protected virtual void Move()
    {
         Vector3 moveBy = currentDirection * speed * Time.fixedDeltaTime;
         myRigidbody.MovePosition(transform.position + moveBy);
    }

    protected void processTurn()
    {
        if (turnPositions.Count == 0 && intendedDirection != currentDirection)
        {//independent movement
            
            isTurning = true;
            turn();
}
        else if (turnPositions.Count > 0 && (turnPositions.Peek() - transform.position).magnitude < ((speed*Time.fixedDeltaTime)/2) )
        {//following the square ahead, we are at a turn position
            
            isTurning = true;
            turn();
        }
        else
        {//no turns
           
            isTurning = false;
        }
    }

    protected virtual void DetermineDirection()
    {

    }

    protected void changeIntendedDirection(Vector3 newDirection)
    {
        if (currentDirection.magnitude < Mathf.Epsilon)
        {
            currentDirection = newDirection;
        }
        intendedDirection = newDirection;
    }

    protected virtual void turn()
    {
        if (turnPositions.Count > 0)
        {
            Vector3 newDirection = directionsToFollow.Dequeue();
            Vector3 newPosition = turnPositions.Dequeue();
            changeIntendedDirection(newDirection);

            if (partScript != null)
            {
                partScript.turnPositions.Enqueue(newPosition);
                partScript.directionsToFollow.Enqueue(newDirection);
            }
        }
        else
        {
            if (partScript != null)
            {
                partScript.turnPositions.Enqueue(transform.position);
                partScript.directionsToFollow.Enqueue(intendedDirection);
            }
        }
        currentDirection = intendedDirection;
    }

    #endregion

    #region basics
    protected virtual void Start()
    {
        if (partToSpawn == null)
        {
            Debug.LogError("Assign a part to spawn to the snake head");
        }
        turnPositions = new Queue<Vector3>();
        directionsToFollow = new Queue<Vector3>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myRenderer = GetComponent<Renderer>();
        myRigidbody.isKinematic = true;
        
    }

    protected virtual void Update()
    {
        DetermineDirection();
        //Move();
    }

    protected virtual void FixedUpdate()
    {
        Move();
        processTurn();
    }

    public Vector3 getDirection()
    {
        return currentDirection;
    }

    public bool GetTurningStatus()
    {
        return isTurning;
    }

    #endregion

    protected void growParts()
    {
        if (childPart == null)
        {
            childPart = Instantiate(partToSpawn, transform.position+currentDirection*-1.0f * GetComponent<Renderer>().bounds.size.x, Quaternion.identity);
            partScript = childPart.GetComponent<SnakeBodyController>();
            partScript.front = this;
            partScript.speed = speed;
            partScript.partToSpawn = partToSpawn;
            partScript.changeIntendedDirection(getDirection());
        }
        else
        {
            childPart.GetComponent<SnakeBodyController>().growParts();
        }
    }
}

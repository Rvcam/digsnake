using System;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Tagger))]
public class SnakePart : MonoBehaviour
{

    #region variables

    [Range(1, 10)]
    [SerializeField]
    private int editorSpeed = 6;
    protected float speed;

    [Range(0, 1)]
    [SerializeField]
    private float distanceFactor; //the fraction of the snake part's total size that they use as distance from the snake in front

    protected Vector3 currentDirection;

    protected GameObject childPart;
    public GameObject partToSpawn;
    protected SnakeBodyController childScript;
    protected SnakePart front;

    private Queue<Vector3> positionsAfterTurn;

    protected SpriteRenderer myRenderer;
    protected Rigidbody2D myRigidbody;

    protected int snakeLength;


    #endregion

    #region movement

    private void recursiveMovement()
    {
        if (front == null)
        {
            IndependentMove();
        }
        else
        {
            if (front.positionsAfterTurn.Count >0)// ((1.0f + distanceFactor) * myRenderer.bounds.size.x) / (Time.fixedDeltaTime * speed))
            {
                myRigidbody.MovePosition(front.positionsAfterTurn.Peek());
            }
            else
            {
                //fica parado, implementar depois
            }
        }

        while (positionsAfterTurn.Count > ((1.0f + distanceFactor) * myRenderer.bounds.size.x) / (Time.fixedDeltaTime * speed))
        {
            //we have moved too far and our trail is too long
            positionsAfterTurn.Dequeue();
        }

        positionsAfterTurn.Enqueue(transform.position);

        if (childScript)
        {
            childScript.currentDirection = currentDirection;
            childScript.recursiveMovement();
        }
    }

    protected virtual void IndependentMove()
    {

    }

    #endregion

    #region basics
    protected virtual void Start()
    {
        if (partToSpawn == null)
        {
            Debug.LogError("Assign a part to spawn to the snake head");
        }

        positionsAfterTurn = new Queue<Vector3>();
        
        myRigidbody = GetComponent<Rigidbody2D>();
        myRenderer = GetComponent<SpriteRenderer>();
        myRigidbody.isKinematic = true;
        
        snakeLength = 0;

        if (speed < Mathf.Epsilon && editorSpeed > Mathf.Epsilon)
        {
            speed = editorSpeed / 2.0f;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (front == null)
        {
            recursiveMovement();
        }
    }

    #endregion


    #region other recursive methods
    protected void growParts()
    {
        snakeLength++;
        if (childPart == null )
        {
            childPart = Instantiate(partToSpawn, transform.position+currentDirection* (-1.0f-distanceFactor) * myRenderer.bounds.size.x, Quaternion.identity);
            childScript = childPart.GetComponent<SnakeBodyController>();
            childScript.front = this;
            childScript.speed = speed;
            childScript.partToSpawn = partToSpawn;
            childScript.distanceFactor = distanceFactor;
        }
        else if (childScript!=null) //just in case the child object gets destroyed prior to this call
        {
            childScript.growParts();
        }
    }

    protected void changeSnakeSpeed(float newSpeed)
    {
        speed = newSpeed;
        if (childScript)
        {
            childScript.changeSnakeSpeed(newSpeed);
        }
    }

    #endregion
}

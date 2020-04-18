using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyUtil;

public class SavePoint : MonoBehaviour
{
    private enum States
    {
        OffScreen,
        OnScreen,
        FarEnough,
        Judging,
        ReadyToActivate,
        Activated
    }

    private GameSceneController gameSceneController;

    [SerializeField]
    private Transform cameraTransform;
    private Vector3 targetPosition;

    [SerializeField]
    private float errorMarginFactor;

    [Range(0f, 4f)]
    public float futureSpeed;
    public Directions futureDirection;
    
    private States state=States.OffScreen;

    public event Action<SavePoint, bool> savePointActivated;
    public event Action arrivedAtSavePoint;

    private int savedLength;

    private void Awake()
    {
        if (savedLength == 0)
        {
            savedLength = 1;
        }
    }

    void Start()
    {
        if (errorMarginFactor<Mathf.Epsilon)
        {
            Debug.LogError("erro margin for save point not set");
        }
        gameSceneController = FindObjectOfType<GameSceneController>();
        targetPosition = cameraTransform.position; // it will be adjusted later at adjustTarget
        targetPosition.z = transform.position.z;
    }
     
    public void adjustTarget()
    {
       switch (gameSceneController.getRoomDirection())
        {
            case Directions.Up:
                targetPosition.x = transform.position.x;
                targetPosition.y = transform.position.y - gameSceneController.getBounds().height / 2 - GetComponent<SpriteRenderer>().bounds.size.y / 2; ;
                break;
            case Directions.Down:
                targetPosition.x = transform.position.x;
                targetPosition.y = transform.position.y + gameSceneController.getBounds().height / 2 + GetComponent<SpriteRenderer>().bounds.size.y / 2; ;
                break;
            case Directions.Left:
                targetPosition.y = transform.position.y;
                targetPosition.x = transform.position.x + gameSceneController.getBounds().width / 2 + GetComponent<SpriteRenderer>().bounds.size.x / 2;
                break;
            case Directions.Right:
                targetPosition.y = transform.position.y;
                targetPosition.x = transform.position.x - gameSceneController.getBounds().width / 2 - GetComponent<SpriteRenderer>().bounds.size.x / 2;
                break;
        }
    }

    void Update()
    {
        if (state == States.OnScreen && (transform.position - targetPosition).magnitude > gameSceneController.roomSpeed * Time.deltaTime * errorMarginFactor)
        {
            state = States.FarEnough;
        }
        if (state == States.FarEnough && (transform.position - targetPosition).magnitude < gameSceneController.roomSpeed * Time.deltaTime * errorMarginFactor)
        {
            arrivedAtSavePoint();
            state = States.Judging;
        }
        if (state==States.Judging && gameSceneController.roomSpeed==0 && gameSceneController.isCollectingWell())
        {
            GetComponent<SpriteRenderer>().color = new Color(100f/256, 240f/256, 70f/256);
            state = States.ReadyToActivate;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger != null && tagger.containsCustomTag("player"))
        {
            if (state == States.ReadyToActivate)
            {
                savedLength = collision.gameObject.GetComponent<PlayerController>().getLength();
                state = States.Activated;
                GetComponent<SpriteRenderer>().color = new Color(70f / 256, 220f / 256, 200f / 256, 0.5f);
                savePointActivated(this, true);
            }
        }
        else if (tagger!=null && tagger.containsCustomTag("outer screen bounds"))
        {
            if (state==States.OffScreen)
            {
                state = States.OnScreen;
                adjustTarget();
            }
        }
    }

    public int getSavedLength()
    {
        return savedLength;
    }
    public void setSavedLength(int newLength)
    {
        savedLength = newLength;
    }
    public void setAsActivated()
    {
        state = States.Activated;
        GetComponent<SpriteRenderer>().color = new Color(70f / 256, 220f / 256, 200f / 256, 0.5f);
    }
}

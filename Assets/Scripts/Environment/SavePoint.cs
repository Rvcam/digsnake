using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyUtil;

public class SavePoint : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;
    private Vector3 targetPosition;
    private GameSceneController gameSceneController;
    private float environmentalSpeed;
    [SerializeField]
    private float errorMarginFactor;
    public event Action arrivedAtSavePoint;
    private bool readyToActivate;
    private bool activated;
    [Range(0f, 4f)]
    public float futureSpeed;
    public Directions futureDirection;
    public event Action<SavePoint, bool> savePointReached;
    [SerializeField]
    [Range(1, 4)]
    private float inactiveTime;
    private bool farEnough;
    private bool targetAdjusted;

    void Start()
    {
        if (errorMarginFactor<Mathf.Epsilon)
        {
            Debug.LogError("erro margin for save point not set");
        }
        gameSceneController = FindObjectOfType<GameSceneController>();
        targetPosition = cameraTransform.position; // it will be adjusted later at adjustTarget
        targetPosition.z = transform.position.z;
        environmentalSpeed = gameSceneController.roomSpeed;
        readyToActivate = false;
        activated = false;
        farEnough = true;
        targetAdjusted = false;
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
        targetAdjusted = true;
    }

    void Update()
    {
        if (targetAdjusted)
        {
            if (farEnough && (transform.position - targetPosition).magnitude < environmentalSpeed * Time.deltaTime * errorMarginFactor)
            {
                arrivedAtSavePoint();
                farEnough = false;
            }
        }
        if ((transform.position - targetPosition).magnitude > environmentalSpeed * Time.deltaTime * 6 * errorMarginFactor)
        {
            farEnough = true;
        }
        if (targetAdjusted && gameSceneController.roomSpeed==0 && activated == false && readyToActivate==false && gameSceneController.isCollectingWell())
        {
            GetComponent<SpriteRenderer>().color = new Color(100f/256, 240f/256, 70f/256);
            readyToActivate = true;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger != null && tagger.containsCustomTag("player"))
        {
            if (activated == false && readyToActivate == true)
            {
                readyToActivate = false;
                activated = true;
                GetComponent<SpriteRenderer>().color = new Color(70f / 256, 220f / 256, 200f / 256, 0.5f);
                savePointReached(this, true);
            }
            else if (activated == true && readyToActivate == true)
            {
                GetComponent<SpriteRenderer>().color = new Color(70f / 256, 220f / 256, 200f / 256, 0.5f);
                readyToActivate = false;
                savePointReached(this, false);
            }
        }
        else if (tagger!=null && tagger.containsCustomTag("outer screen bounds"))
        {
            if (activated == false)
            {
                adjustTarget();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;
using System;

public class LevelPortal : MonoBehaviour
{
    private GameSceneController gameSceneController;
    private Vector3 targetPosition;
    private SpriteRenderer myRenderer;
    private bool centered=false;
    private bool playerTouched=false;
    private bool won = false;
    [SerializeField]
    private string nextLevel="";

    public event Action<String> win;

    [SerializeField]
    private float errorMargin = 1.0f;

    private void Start()
    {
        gameSceneController = FindObjectOfType<GameSceneController>();
    }
    private void Update()
    {
        if (centered == false && (transform.position - targetPosition).magnitude < errorMargin)
        {
            centered = true;
            gameSceneController.roomSpeed = 0;
        }
        else if (won==false && centered == true && playerTouched == true)
        {   
            win(nextLevel);
            won = true;
        }
        
    }
    private Vector3 getSize()
    {
        return GetComponent<Collider2D>().bounds.size;
    }
    public void adjustTarget()
    {

        if (gameSceneController != null)
        {
            switch (gameSceneController.getRoomDirection())
            {
                case Directions.Up:
                    targetPosition.x = transform.position.x;
                    targetPosition.y = transform.position.y - gameSceneController.getBounds().height / 2 - getSize().y / 2;
                    break;
                case Directions.Down:
                    targetPosition.x = transform.position.x;
                    targetPosition.y = transform.position.y + gameSceneController.getBounds().height / 2 + getSize().y / 2; ;
                    break;
                case Directions.Left:
                    targetPosition.y = transform.position.y;
                    targetPosition.x = transform.position.x + gameSceneController.getBounds().width / 2 + getSize().x / 2;
                    break;
                case Directions.Right:
                    targetPosition.y = transform.position.y;
                    targetPosition.x = transform.position.x - gameSceneController.getBounds().width / 2 - getSize().x / 2;
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger != null)
        {
            if (tagger.containsCustomTag("outer screen bounds"))
            {
                adjustTarget();
            }
            else if (tagger.containsCustomTag("player") && playerTouched==false)
            {
                playerTouched = true;
            }
        }   
    }
}

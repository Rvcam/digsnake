using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;

public class SnakeHeadCollider : MonoBehaviour
{
    private GameSceneController gameSceneController;
    PlayerController head;
    private void Start()
    {
        gameSceneController = FindObjectOfType<GameSceneController>();
        head = transform.parent.gameObject.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Tagger tagger = collider.gameObject.GetComponent<Tagger>();
        if (tagger)
        {
            if (gameSceneController.gameOver == false)
            {
                if (tagger.containsCustomTag("obstacle"))
                {
                    head.onChildObstacleHit();
                }
            }
        }
    }
}

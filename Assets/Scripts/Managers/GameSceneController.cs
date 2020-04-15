using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    //public Rectangle bounds { get => _bounds; }
    //private Rectangle _bounds;

    public Rect bounds { get => _bounds; }
    private Rect _bounds;

    public bool gameOver { get => _gameOver; }
    private bool _gameOver;
    
    public int numberOfFruits;
    [Range(0f, 2f)]
    public float roomSpeed;

    private void Awake()
    {
        float width = GetComponent<BoxCollider2D>().bounds.size.x;
        float height = GetComponent<BoxCollider2D>().bounds.size.y;
        _bounds = new Rect (transform.position.x-width/2, transform.position.y-height/2, width, height);
        numberOfFruits = 0;
    }

    private void Start()
    {
        FindObjectOfType<PlayerController>().Finished += showEndMessage;
        FindObjectOfType<PlayerController>().Finished += endGame;
    }

    private void endGame(bool won)
    {
        _gameOver = true;
    }

    private void showEndMessage(bool won)
    {
        if (won)
        {
            FindObjectOfType<Text>().GetComponent<Text>().text = "You are Winner !!!";
            FindObjectOfType<Text>().GetComponent<Text>().color = Color.green;
            FindObjectOfType<Text>().GetComponent<Text>().fontSize = 54;
        }        
        else
        {
            FindObjectOfType<Text>().GetComponent<Text>().text = "try again? (ENTER)";
            FindObjectOfType<Text>().GetComponent<Text>().color = Color.red;
        }
    }

    
}

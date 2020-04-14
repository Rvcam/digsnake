using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    private Rectangle _bounds;
    private bool _gameOver;
    public bool gameOver { get => _gameOver; }
    public Rectangle bounds { get => _bounds; }
    public int numberOfFruits;
    private void Awake()
    {
        _bounds = new Rectangle(transform.position, GetComponent<BoxCollider2D>().bounds.size.x, GetComponent<BoxCollider2D>().bounds.size.y);
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

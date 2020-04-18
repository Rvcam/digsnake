using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameSceneController : MonoBehaviour
{
    private Rect bounds;

    public bool gameOver { get => _gameOver; }
    private bool _gameOver;

    FruitManager fruitManager;
    public int numberOfFruits;
    [Range(0f, 4f)]
    public float roomSpeed;
    [SerializeField]
    private Directions roomDirection;
    private int requiredFruits;
    private int totalCollectedFruit;
    [SerializeField]
    private int fruitLenience;
    public event Action directionChanged;

    private void Awake()
    {
        if (FindObjectOfType<GameManager>()==null)
        {
            SceneManager.LoadScene("preload");
        }
        numberOfFruits = 0;
        float width = GetComponent<BoxCollider2D>().bounds.size.x;
        float height = GetComponent<BoxCollider2D>().bounds.size.y;
        bounds = new Rect(transform.position.x - width / 2, transform.position.y - height / 2, width, height);
    }

    private void Start()
    {
        fruitManager = FindObjectOfType<FruitManager>();
        PlayerController playerController = FindObjectOfType<PlayerController>();
        foreach (SavePoint sp in FindObjectsOfType<SavePoint>())
        {
            sp.arrivedAtSavePoint += stopScreen;
            sp.savePointReached += useSavePoint;
        }
        playerController.Finished += showEndMessage;
        playerController.Finished += endGame;
        playerController.FruitCollected += fruitCollected;
        fruitManager.spawned += fruitSpawned;
        numberOfFruits += FindObjectsOfType<Fruit>().Length;
        totalCollectedFruit = 0;
        requiredFruits = numberOfFruits;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (roomSpeed==0)
        {

        }
    }

    public Rect getBounds()
    {
        return bounds;
    }

    public Directions getRoomDirection()
    {
        return roomDirection;
    }

    public Vector3 getRoomDirectionAsVector()
    {
        return translateDirection(roomDirection);
    }

    private void stopScreen()
    {
        roomSpeed = 0;
        fruitManager.controlSpawning(false);
    }

    private void fruitCollected()
    {
        numberOfFruits--;
        totalCollectedFruit++;
    }

    private void fruitSpawned()
    {
        numberOfFruits++;
        requiredFruits++;
    }

    public bool isCollectingWell()
    {
        return totalCollectedFruit + fruitLenience >= requiredFruits;
    }

    private void useSavePoint(SavePoint sp, bool firstActivation)
    {
        roomDirection = sp.futureDirection;
        roomSpeed = sp.futureSpeed;
        foreach(Fruit fruit in FindObjectsOfType<Fruit>())
        {
            Destroy(fruit.gameObject);
        }
        requiredFruits = 0;
        totalCollectedFruit = 0;
        fruitManager.controlSpawning(true);
        directionChanged();
    }

    private Vector3 translateDirection(Directions original)
    {
        switch (original)
        {
            case Directions.Down:
                return Vector3.down;
            case Directions.Left:
                return Vector3.left;
            case Directions.Right:
                return Vector3.right;
            case Directions.Up:
                return Vector3.up;
            default:
                return Vector3.zero;
        }
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

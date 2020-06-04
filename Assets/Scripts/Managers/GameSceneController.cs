using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Dynamic;
using TMPro;

public class GameSceneController : MonoBehaviour
{
    #region variables
    private Rect bounds;

    public bool gameOver { get => _gameOver; }
    private bool _gameOver;

    [HideInInspector]
    public float roomSpeed;
    [SerializeField]
    private Directions roomDirection;
    private bool started;

    FruitManager fruitManager;
    GameManager gameManager;
    PlayerController playerController;
    private LostFruitsUI lostFruitsUI;
    private LevelTransitionUI levelTransitionUI;

    private int numberOfFruits;
    private int requiredFruits;
    private int totalCollectedFruit;
    private int fruitLenience;
    private int fruitLost;

    [SerializeField]
    private SavePoint startingSP;
    private SavePoint lastSP;

    public TextMeshPro fruitCollectedNumberIndicator=null;

    #endregion

    #region monobehaviour methods
    private void Awake()
    {
        if (FindObjectOfType<GameManager>() == null)
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
        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        lostFruitsUI = FindObjectOfType<LostFruitsUI>();
        levelTransitionUI = FindObjectOfType<LevelTransitionUI>();

        fruitLenience = lostFruitsUI.getQuantity();

        foreach (SavePoint sp in FindObjectsOfType<SavePoint>())
        {
            sp.arrivedAtSavePoint += stopScreen;
            sp.savePointActivated += useSavePoint;
            if (gameManager.activeSPs.ContainsKey(sp.gameObject.name) && gameManager.activeSPs[sp.gameObject.name] == true)
            {
                sp.externalActivate();
            }
            if (sp.gameObject.name == gameManager.startingSP)
            {
                startingSP = sp;
                startingSP.setSavedLength(gameManager.startingLength);
            }
        }
        lastSP = startingSP;
        saveSPToGameManager(startingSP);
        startingSP.externalActivate();

        roomSpeed = startingSP.futureSpeed;
        roomDirection = startingSP.futureDirection;

        playerController.transform.Translate(startingSP.transform.position - playerController.transform.position + getRoomDirectionAsVector() * (startingSP.getSize().magnitude));
        Camera mainCamera = FindObjectOfType<Camera>();
        mainCamera.transform.Translate
            (
                playerController.transform.position.x - mainCamera.transform.position.x + getRoomDirectionAsVector().x * (bounds.width / 2 - 2 * startingSP.getSize().x),
                playerController.transform.position.y - mainCamera.transform.position.y + getRoomDirectionAsVector().y * (bounds.height / 2 - 2 * startingSP.getSize().y),
                0
            );
        fruitManager.transform.Translate(mainCamera.transform.position - fruitManager.transform.position);
        transform.Translate(mainCamera.transform.position.x - transform.position.x, mainCamera.transform.position.y - transform.position.y, 0);


        foreach (LevelPortal portal in FindObjectsOfType<LevelPortal>())
        {
            portal.win += nextLevel;
        }
        playerController.Finished += endGame;
        playerController.FruitCollected += fruitCollected;
        fruitManager.Spawned += fruitSpawned;
        numberOfFruits += FindObjectsOfType<Fruit>().Length;

        fruitLost = 0;
        totalCollectedFruit = 0;
        requiredFruits = numberOfFruits;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            respawn();
        }

        if (started == false && playerController.isReady())
        {
            playerController.delayedGrowth(startingSP.getSavedLength() - 1);
            started = true;
        }
    }

    #endregion

    #region room space and movement

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

    #endregion

    #region fruits
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

    public void DealWithFruitLoss()
    {
        fruitLost++;
        if (fruitLost <= fruitLenience)
        {
            lostFruitsUI.indicateLoss();
        }
        else
        {
            lostFruitsUI.indicateNotEnoughFruit();
            Invoke("respawn", 1.6f);
        }
    }

    public bool isCollectingWell()
    {
        return totalCollectedFruit + fruitLenience >= requiredFruits && FindObjectsOfType<Fruit>().Length == 0;
    }

    public int getCollectedFruit()
    {
        return totalCollectedFruit;
    }

#endregion

    #region savepoints and respawn

private void useSavePoint(SavePoint sp)
    {
        lostFruitsUI.reset();
        if ( ! (gameManager.activeSPs.ContainsKey(sp.gameObject.name) && gameManager.activeSPs[sp.gameObject.name] == true)) //we have encountered this save point for the first time
        {// pay attention to the negation on the if condition (!)
            roomDirection = sp.futureDirection;
            roomSpeed = sp.futureSpeed;
            foreach (Fruit fruit in FindObjectsOfType<Fruit>())
            {
                Destroy(fruit.gameObject);
            }
            requiredFruits = 0;
            totalCollectedFruit = 0;
            fruitManager.controlSpawning(true);
            lastSP = sp;
            saveSPToGameManager(sp);
        }
    }

    private void saveSPToGameManager(SavePoint sp)
    {
        if (gameManager.activeSPs.ContainsKey(sp.gameObject.name))
        {
            gameManager.activeSPs[sp.gameObject.name] = true;
        }
        else
        {
            gameManager.activeSPs.Add(sp.gameObject.name, true);
        }
    }

    private void respawn()
    {
        FindObjectOfType<DeathUI>().disappear();
        gameManager.startingSP = lastSP.gameObject.name;
        gameManager.startingLength = lastSP.getSavedLength();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    #region game end and level transition

    private void endGame(bool won)
    {
        _gameOver = true;
        FindObjectOfType<DeathUI>().appear();
    }

    private void nextLevel(string nextLevelName)
    {
        if (!_gameOver)
        {
            fruitCollectedNumberIndicator.text = playerController.getLength().ToString();
            Animator transitionAnimator = levelTransitionUI.GetComponent<Animator>();
            transitionAnimator.enabled = true;
            float animationTime = transitionAnimator.runtimeAnimatorController.animationClips[0].length;
            StartCoroutine(delayedNextLevel(animationTime, nextLevelName));
        }
    }

    private IEnumerator delayedNextLevel(float delay, string nextLevelName)
    {
        yield return new WaitForSeconds(delay);
        while (!Input.GetKey(KeyCode.Space))
        {
            yield return new WaitForEndOfFrame();
        }
        gameManager.transition(nextLevelName);
    }

    #endregion
}

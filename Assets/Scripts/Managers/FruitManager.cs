using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MyUtil;

public class FruitManager : MonoBehaviour
{
    [SerializeField]
    private GameObject fruit= null;
    private Vector2 fruitExtents;
    [SerializeField]
    private float minSpawnTime=2;
    [SerializeField]
    private float maxSpawnTime=2;
    private GameSceneController gameSceneController;
    private Rect bounds;
    public event Action spawned;
    private float spawnHeight;
    private float spawnWidth;
    Coroutine spawner;
    // Start is called before the first frame update
    void Start()
    {
        fruitExtents = fruit.GetComponent<BoxCollider2D>().size;
        gameSceneController = FindObjectOfType<GameSceneController>();
        controlSpawning(true);
        spawnWidth = GetComponent<BoxCollider2D>().bounds.size.x;
        spawnHeight = GetComponent<BoxCollider2D>().bounds.size.y;
        adjustSpawnArea();
        gameSceneController.directionChanged += adjustSpawnArea;
    }

    private IEnumerator spawnRandomFruit()
    {
        while (gameSceneController.gameOver==false)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minSpawnTime, maxSpawnTime));
            Vector3 randomPosition = new Vector3(
                UnityEngine.Random.Range(bounds.min.x,bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                gameSceneController.transform.position.z);
            
            int accumulator;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(randomPosition, 2 * fruitExtents, 0);
            for (accumulator=0; accumulator<colliders.Length; accumulator++)
            {
                Tagger colTagger = colliders[accumulator].gameObject.GetComponent<Tagger>();
                if (colTagger!=null && colTagger.containsCustomTag("obstacle"))
                {
                    break;
                }
            }

            if (accumulator==colliders.Length)
            {
                spawned();
                gameSceneController.numberOfFruits++;
                Instantiate(fruit, randomPosition, Quaternion.identity);
            }
        }
    }

    public void controlSpawning(bool shouldSpawn)
    {
        if (!shouldSpawn)
        {
            if (spawner != null)
            {
                StopCoroutine(spawner);
                spawner = null;
            }
            else
            {
                Debug.LogError("tried to stop fruit spawn when it's already stopped");
            }
        }
        else if (shouldSpawn)
        {
            if (spawner==null)
            {
                spawner = StartCoroutine(spawnRandomFruit());
            }
            else
            {
                Debug.LogError("tried to start fruit spawn when it's already started");
            }

        }
    }

    public float getMinSpawnTime()
    {
        return minSpawnTime;
    }

    public float getMaxSpawnTime()
    {
        return maxSpawnTime;
    }

    private void adjustSpawnArea()
    {
        if (gameSceneController.getRoomDirectionAsVector().x > Mathf.Epsilon)
        {
            bounds = new Rect(transform.position.x, transform.position.y - spawnHeight / 2, spawnWidth, spawnHeight);
        }
        else if (gameSceneController.getRoomDirectionAsVector().x < -Mathf.Epsilon)
        {
            bounds = new Rect(transform.position.x - spawnWidth, transform.position.y - spawnHeight / 2, spawnWidth, spawnHeight);
        }
        else if (gameSceneController.getRoomDirectionAsVector().y > Mathf.Epsilon)
        {
            bounds = new Rect(transform.position.x - spawnWidth / 2, transform.position.y, spawnWidth, spawnHeight);
        }
        else if (gameSceneController.getRoomDirectionAsVector().y < -Mathf.Epsilon)
        {
            bounds = new Rect(transform.position.x - spawnWidth / 2, transform.position.y-spawnHeight, spawnWidth, spawnHeight);
        }
    }
}


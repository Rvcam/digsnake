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
    public event Action Spawned;
    private float horizontalSpawnHeight;
    private float horizontalSpawnWidth;
    private float verticalSpawnHeight;
    private float verticalSpawnWidth;
    Coroutine spawner;
    // Start is called before the first frame update
    void Start()
    {
        fruitExtents = fruit.GetComponent<BoxCollider2D>().size;
        gameSceneController = FindObjectOfType<GameSceneController>();
        horizontalSpawnWidth = GetComponents<BoxCollider2D>()[0].bounds.size.x;
        horizontalSpawnHeight = GetComponents<BoxCollider2D>()[0].bounds.size.y;
        verticalSpawnWidth = GetComponents<BoxCollider2D>()[1].bounds.size.x;
        verticalSpawnHeight = GetComponents<BoxCollider2D>()[1].bounds.size.y;

        controlSpawning(true);
    }

    private void Update()
    {
        adjustSpawnArea();
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
            Collider2D[] colliders = Physics2D.OverlapBoxAll(randomPosition, fruitExtents, 0);
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
                Instantiate(fruit, randomPosition, Quaternion.identity);
                Spawned();
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
        }
        else if (shouldSpawn)
        {
            if (spawner==null)
            {
                spawner = StartCoroutine(spawnRandomFruit());
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
        if (gameSceneController != null)
        {
            switch (gameSceneController.getRoomDirection())
            {
                case Directions.Right:
                    bounds = new Rect(transform.position.x, transform.position.y - horizontalSpawnHeight / 2, horizontalSpawnWidth, horizontalSpawnHeight);
                    break;

                case Directions.Left:
                    bounds = new Rect(transform.position.x - horizontalSpawnWidth, transform.position.y - horizontalSpawnHeight / 2, horizontalSpawnWidth, horizontalSpawnHeight);
                    break;

                case Directions.Up:
                    bounds = new Rect(transform.position.x - verticalSpawnWidth / 2, transform.position.y, verticalSpawnWidth, verticalSpawnHeight);
                    break;

                case Directions.Down:
                    bounds = new Rect(transform.position.x - verticalSpawnWidth / 2, transform.position.y - horizontalSpawnHeight, verticalSpawnWidth, verticalSpawnHeight);
                    break;
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;

public class FruitManager : MonoBehaviour
{
    private GameSceneController gsController;
    [SerializeField]
    private GameObject fruit= null;
    private Vector2 fruitExtents;
    [SerializeField]
    private float minSpawnTime=2;
    [SerializeField]
    private float maxSpawnTime=2;
    private GameSceneController gameSceneController;

    // Start is called before the first frame update
    void Start()
    {
        gsController = FindObjectOfType<GameSceneController>();
        fruitExtents = fruit.GetComponent<BoxCollider2D>().size;
        gameSceneController = FindObjectOfType<GameSceneController>();
        gameSceneController.numberOfFruits += FindObjectsOfType<Fruit>().Length;
        StartCoroutine(spawnRandomFruit());
    }

    private IEnumerator spawnRandomFruit()
    {
        while (gameSceneController.gameOver==false)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(gsController.bounds.min.x, gsController.bounds.max.x),
                Random.Range(gsController.bounds.min.y, gsController.bounds.max.y),
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
                gameSceneController.numberOfFruits++;
                Instantiate(fruit, randomPosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(Random.Range(minSpawnTime,maxSpawnTime));
        }
    }

}


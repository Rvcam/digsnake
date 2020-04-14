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
    private float minSpawnTime;
    [SerializeField]
    private float maxSpawnTime;
    private GameSceneController gameSceneController;

    // Start is called before the first frame update
    void Start()
    {
        gsController = FindObjectOfType<GameSceneController>();
        fruitExtents = fruit.GetComponent<BoxCollider2D>().size;
        Instantiate(fruit, gsController.bounds.bottomLeft, Quaternion.identity);
        Instantiate(fruit, gsController.bounds.bottomRight, Quaternion.identity);
        Instantiate(fruit, gsController.bounds.topLeft, Quaternion.identity);
        Instantiate(fruit, gsController.bounds.topRight, Quaternion.identity);
        gameSceneController = FindObjectOfType<GameSceneController>();
        gameSceneController.numberOfFruits += 5;
        StartCoroutine(spawnRandomFruit());
    }

    private IEnumerator spawnRandomFruit()
    {
        
        while (gameSceneController.gameOver==false)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(gsController.bounds.bottomLeft.x, gsController.bounds.topRight.x),
                Random.Range(gsController.bounds.bottomLeft.y, gsController.bounds.topRight.y),
                gsController.bounds.topRight.z);
            
            int i;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(randomPosition, 2 * fruitExtents, 0);
            for (i=0; i<colliders.Length; i++)
            {
                Tagger colTagger = colliders[i].gameObject.GetComponent<Tagger>();
                if (colTagger!=null && colTagger.containsCustomTag("obstacle"))
                {
                    break;
                }
            }
            if (i==colliders.Length)
            {
                gameSceneController.numberOfFruits++;
                Instantiate(fruit, randomPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(minSpawnTime,maxSpawnTime));
        }
    }

}


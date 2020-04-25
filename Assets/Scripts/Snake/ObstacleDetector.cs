using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;

public class ObstacleDetector : MonoBehaviour
{
    [SerializeField]
    private GameObject associatedSprite=null;
    private SpriteRenderer associatedRenderer;

    private void Start()
    {
        associatedRenderer = associatedSprite.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger != null && tagger.containsCustomTag("obstacle") && !tagger.containsCustomTag("snake part"))
        {
            Color original = associatedRenderer.color;
            associatedRenderer.color = new Color(original.r, original.g, original.b, 0.2f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger != null && tagger.containsCustomTag("obstacle") && !tagger.containsCustomTag("snake part"))
        {
            Color original = associatedRenderer.color;
            associatedRenderer.color = new Color(original.r, original.g, original.b, 1f);
        }
    }
}

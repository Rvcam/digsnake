using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;

public class GameSceneController : MonoBehaviour
{
    private Rectangle _bounds;

    private void Awake()
    {
        _bounds = new Rectangle(transform.position, GetComponent<BoxCollider2D>().bounds.size.x, GetComponent<BoxCollider2D>().bounds.size.y);
    }

    public Rectangle bounds { get=>_bounds; }
    
}

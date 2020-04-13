using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyController : SnakePart
{
    Vector3 lastPosition;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        currentDirection = (transform.position - lastPosition).normalized;
        lastPosition = transform.position;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;

public class SnakeBodyController : SnakePart
{
    Vector3 lastPosition;
    Animator animator;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        currentDirection = (transform.position - lastPosition).normalized;
        lastPosition = transform.position;
    }

    public IEnumerator blink(Color newColor)
    {
        Color oldColor = myRenderer.color;
        Color actualColor = newColor;
        while (true)
        {
            myRenderer.color = actualColor;
            actualColor = actualColor == newColor ? oldColor : newColor;
            yield return new WaitForSeconds(0.75f);
        }
    }
}

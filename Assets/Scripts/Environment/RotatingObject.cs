using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [SerializeField]
    [Range(-180, 180)]
    private float rotationPerSecond=0;

    void Update()
    {
        transform.Rotate(Vector3.forward, rotationPerSecond * Time.deltaTime);
    }
}

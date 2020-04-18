using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;

[RequireComponent(typeof (Tagger))]

public class GenericObstacle : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Tagger>().addCustomTag("obstacle");
    }
}

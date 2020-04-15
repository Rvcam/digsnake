using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalObject : MonoBehaviour
{
    private float roomSpeed = 0;
    private GameSceneController gameSceneController;
    private void Start()
    {
        gameSceneController = FindObjectOfType<GameSceneController>();
        roomSpeed = gameSceneController.roomSpeed;
    }

    private void Update()
    {
        transform.Translate(-roomSpeed*Time.deltaTime, 0, 0);
    }
}

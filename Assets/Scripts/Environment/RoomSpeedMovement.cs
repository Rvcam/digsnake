using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpeedMovement : MonoBehaviour
{
    private GameSceneController gameSceneController;
    private void Start()
    {
        gameSceneController = FindObjectOfType<GameSceneController>();
    }

    private void Update()
    {
        transform.Translate(-1 * gameSceneController.getRoomDirectionAsVector() * gameSceneController.roomSpeed * Time.deltaTime, Space.World);
    }
}

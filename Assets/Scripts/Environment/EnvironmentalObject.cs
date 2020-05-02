using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;
public class EnvironmentalObject : MonoBehaviour
{
    private GameSceneController gameSceneController;
    [SerializeField]
    [Range(-3f, 3)]
    private float aditionalSpeed=0;
    [SerializeField]
    private bool aditionalSpeedEnabled = false;
    private Vector3 originalDirection;

    private void Start()
    {
        gameSceneController = FindObjectOfType<GameSceneController>();
        if (aditionalSpeedEnabled)
        {
            originalDirection = gameSceneController.getRoomDirectionAsVector();
        }
    }

    private void Update()
    {
        Vector3 translateBy = -1 * gameSceneController.roomSpeed * gameSceneController.getRoomDirectionAsVector() * Time.deltaTime;
        if (aditionalSpeedEnabled)
        {
            translateBy += originalDirection * aditionalSpeed * Time.deltaTime;
        }
        transform.Translate(translateBy, Space.World);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger != null)
        {
            if (aditionalSpeedEnabled==false && tagger.containsCustomTag("outer screen bounds"))
            {
                aditionalSpeedEnabled = true;
                originalDirection = gameSceneController.getRoomDirectionAsVector();
            }
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public string nextLevel;
    private void Awake()
    {
        if (FindObjectOfType<GameManager>() == null)
        {
            SceneManager.LoadScene(nextLevel);
        }
    }

    private void Start()
    {
        FindObjectOfType<GameManager>().GetComponent<AudioSource>().Stop();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) 
        {
            FindObjectOfType<GameManager>().GetComponent<AudioSource>().Play();
            SceneManager.LoadScene(nextLevel);
        }
    }
}
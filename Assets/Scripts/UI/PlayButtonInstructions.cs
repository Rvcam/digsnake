using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonInstructions : MonoBehaviour
{
    public string nextLevel;
    public string preloadLevel;
    private void Awake()
    {
        if (FindObjectOfType<GameManager>() == null)
        {
            SceneManager.LoadScene(preloadLevel);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) )
        {
            FindObjectOfType<GameManager>().transition(nextLevel);
        }
    }
}
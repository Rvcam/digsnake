using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonIntro : MonoBehaviour
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
        if (Input.anyKeyDown) 
        {
            SceneManager.LoadScene(nextLevel);
        }
    }
}
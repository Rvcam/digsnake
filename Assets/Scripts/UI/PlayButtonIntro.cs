using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonIntro : MonoBehaviour
{
    public string nextLevel;
    private void Awake()
    {
        if (FindObjectOfType<GameManager>() == null)
        {
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            FindObjectOfType<GameManager>().GetComponent<AudioSource>().Stop();
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
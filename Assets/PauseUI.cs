using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    bool paused = false;
    void Update()
    {
        if (paused == false && Time.timeScale < Mathf.Epsilon)
        {
            paused = true;
            GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (paused == true && Time.timeScale > Mathf.Epsilon)
        {
            paused = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }

    }
}

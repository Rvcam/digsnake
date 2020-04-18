using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string firstScene;
    public string startingSP;
    public int startingLength;
    public Dictionary<string, bool> activeSPs;

    private void Awake()
    {
        activeSPs = new Dictionary<string, bool>();
        startingLength = 1;
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(firstScene);
    }
}

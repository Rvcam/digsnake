using System;
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

    public void transition(string sceneName)
    {
        resetLevelData();
        float timeToWait = 2;
        StartCoroutine(fadeOut(timeToWait));
        StartCoroutine(waitAndPlay(timeToWait + 0.5f));
        StartCoroutine(waitAndLoadScene(timeToWait, sceneName));
    }
    private void resetLevelData()
    {
        startingLength = 1;
        activeSPs.Clear();
        
    }

    private IEnumerator fadeOut(float duration)
    {
        AudioSource audioSource  = GetComponent<AudioSource>();
        float currentTime = 0;
        float start = audioSource.volume;
        
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0, currentTime / duration);
            yield return null;
        }
        audioSource.volume = 0;
    }

    private IEnumerator waitAndPlay(float duration)
    {
        yield return new WaitForSeconds(duration);
        GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().volume = 1;
       
    }

    private IEnumerator waitAndLoadScene(float duration, string sceneName)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(sceneName);
    }
}

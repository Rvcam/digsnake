using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string firstScene;
    public string startingSP;
    public int startingLength;
    public Dictionary<string, bool> activeSPs;
    
    [SerializeField]
    AudioClip mainClip=null;

    private float originalVolume;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        activeSPs = new Dictionary<string, bool>();
        startingLength = 1;
        GetComponent<AudioSource>().volume = 1.5f;
        GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(firstScene);
    }

    public void transition(string sceneName)
    {
        GetComponent<AudioSource>().clip = mainClip;
        GetComponent<AudioSource>().volume = 0.5f;
        resetLevelData();
        float timeToWait = 2;
        StartCoroutine(fadeOut(timeToWait));
        StartCoroutine(waitAndPlay(timeToWait + 0.5f));
        StartCoroutine(waitAndLoadScene(timeToWait, sceneName));
    }
    private void resetLevelData()
    {
        startingLength = 1;
        startingSP = null;
        activeSPs.Clear();
        
    }

    private IEnumerator fadeOut(float duration)
    {
        AudioSource audioSource  = GetComponent<AudioSource>();
        float currentTime = 0;
        float start = audioSource.volume;
        originalVolume = start;
        
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
        GetComponent<AudioSource>().volume = originalVolume;
       
    }

    private IEnumerator waitAndLoadScene(float duration, string sceneName)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(sceneName);
    }
}

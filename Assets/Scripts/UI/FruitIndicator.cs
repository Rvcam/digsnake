using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FruitIndicator : MonoBehaviour
{
    [SerializeField]
    Light2D lightToFlash=null;

    public void disappear()
    {
        StartCoroutine(blinkAndVanish());
    }

    private IEnumerator blinkAndVanish()
    {
        for (int i = 0; i < 3; i++)
        {
            lightToFlash.intensity = 1.5f;
            yield return new WaitForSeconds(0.15f);
            lightToFlash.intensity = 0;
            yield return new WaitForSeconds(0.15f);
        }
        gameObject.SetActive(false);
    }

    public void reset()
    {
        lightToFlash.intensity = 0;
        gameObject.SetActive(true);
    }
}

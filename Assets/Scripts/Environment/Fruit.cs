using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;
using System;
using System.Security.Cryptography;

public class Fruit : MonoBehaviour
{
    public event Action exitedScreen;
    private bool isOffScreen = false;
    private bool messageSent = false;
    private void Update()
    {
        if (messageSent==false && isOffScreen)
        {
            messageSent = true;
            exitedScreen();
            Destroy(gameObject);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger)
        {
            if (tagger.containsCustomTag("outer screen bounds"))
            {
                isOffScreen = true;
            }
            else if (tagger.containsCustomTag("inner screen bounds"))
            {
                if (isOffScreen == false)
                {
                    //here would be a code to flash before disappearing, if playtest indicates we should implement that
                }
            }
        }
    }
}

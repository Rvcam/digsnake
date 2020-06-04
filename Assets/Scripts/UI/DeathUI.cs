using UnityEngine;

public class DeathUI : MonoBehaviour
{
    public void appear()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<Animator>().enabled = true;
    }

    public void disappear()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Animator>().enabled = false;
    }
}

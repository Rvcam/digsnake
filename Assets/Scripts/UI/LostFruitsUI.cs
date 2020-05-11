using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostFruitsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject[] fruitIndicators=null;
    private int indicatorsQuantity;
    private void Start()
    {
        indicatorsQuantity = fruitIndicators.Length;
    }

    public void indicateLoss()
    {
        if (indicatorsQuantity - 1 >= 0)
        {   
            fruitIndicators[indicatorsQuantity - 1].SetActive(false);
        }
        indicatorsQuantity--;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostFruitsUI : MonoBehaviour
{
    [SerializeField]
    private FruitIndicator[] fruitIndicators = null;
    private int indicatorsQuantity;

    private void Awake()
    {
        indicatorsQuantity = fruitIndicators.Length;
    }

    public void indicateLoss()
    {
        if (indicatorsQuantity - 1 >= 0)
        {
            fruitIndicators[indicatorsQuantity - 1].disappear();
        }
        indicatorsQuantity--;
    }

    public void indicateNotEnoughFruit()
    {
        NotEnoughIndicator indicator = GetComponentInChildren<NotEnoughIndicator>();
        if (indicator!=null)
        {
            indicator.warn();
        }
    }

    public int getQuantity()
    {
        return indicatorsQuantity;
    }

    public void reset()
    {
        indicatorsQuantity = fruitIndicators.Length;
        for (int i = 0; i < fruitIndicators.Length; i++)
        {
            fruitIndicators[i].reset();
        }
    }

}

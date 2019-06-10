using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleItemsInShop : MonoBehaviour
{
    [SerializeField] private GameObject[] foodItems;
    [SerializeField] private GameObject[] detergentItems;
    [SerializeField] private GameObject[] frigdeItems;
    [SerializeField] private GameObject[] washerItems;

    public void ChangeFoodItemsDisplayed(bool add, int actualNumber)
    {
        if(add)
        {
            if(actualNumber - 1 < foodItems.Length)
                foodItems[actualNumber - 1].SetActive(true); //actual number is one for behind
        }
        else //remove
        {
            if (actualNumber < foodItems.Length)
                foodItems[actualNumber].SetActive(false);
        }
    }
    public void ChangeDetergentItemsDisplayed(bool add, int actualNumber)
    {
        if (add)
        {
            if (actualNumber - 1 < detergentItems.Length)
                detergentItems[actualNumber - 1].SetActive(true); //actual number is one for behind
        }
        else //remove
        {
            if (actualNumber < detergentItems.Length)
                detergentItems[actualNumber].SetActive(false);
        }
    }
    public void ChangeFridgeItemsDisplayed(bool add, int actualNumber)
    {
        if (add)
        {
            if (actualNumber - 1 < frigdeItems.Length)
                frigdeItems[actualNumber - 1].SetActive(true); //actual number is one for behind
        }
        else //remove
        {
            if (actualNumber < frigdeItems.Length)
                frigdeItems[actualNumber].SetActive(false);
        }
    }
    public void ChangeWasherItemsDisplayed(bool add, int actualNumber)
    {
        if (add)
        {
            if (actualNumber - 1 < washerItems.Length)
                washerItems[actualNumber - 1].SetActive(true); //actual number is one for behind
        }
        else //remove
        {
            if (actualNumber < washerItems.Length)
                washerItems[actualNumber].SetActive(false);
        }
    }
    public void DayOne()
    {
        HideAllItems();
    }

    private void HideAllItems()
    {
        for (int i = 0; i < foodItems.Length; i++)
        {
            foodItems[i].SetActive(false);
        }
        for (int i = 0; i < frigdeItems.Length; i++)
        {
            frigdeItems[i].SetActive(false);
        }
        for (int i = 0; i < detergentItems.Length; i++)
        {
            detergentItems[i].SetActive(false);
        }
        for (int i = 0; i < washerItems.Length; i++)
        {
            washerItems[i].SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public List<Item> usableItems = new List<Item>();
    public int foodStored = 0;
    public Item fridgeItem;
    public Item washer;
    public float money;
    public Item lastWasher = null;
    public Item lastFridge = null;
    
    public void AddNewItem(Item item)
    { 
        if(item.itemType == Item.ItemType.FRIDGE)
        {
            fridgeItem = item;
        }
        else if(item.itemType == Item.ItemType.WASHER)
        {
            washer = item;
        }
        else
        {
            if (item.itemType == Item.ItemType.FOOD)
            {
                foodStored++;
            } 
            usableItems.Add(item);
        }
            
    }

    public void UpdateInventory()
    {
        int i = 0;
        while (i < usableItems.Count)
        {
            if(fridgeItem == null && usableItems[i].itemType == Item.ItemType.FOOD)
            {
                foodStored--;             
                usableItems.RemoveAt(i);
            }
            else if (!usableItems[i].NextDay())
            {
                if(usableItems[i].itemType == Item.ItemType.FOOD)
                {
                    GetComponent<Npc>().ChangeRelationshipsOnUsedItems(usableItems[i], true,true);
                    foodStored--;
                }
                usableItems.RemoveAt(i);
                //other efects
            }
            else
                i++;
        }
        if(washer != null && !washer.NextDay())
        {
            GetComponent<Npc>().ChangeRelationshipsOnUsedItems(washer, true ,true);
            lastWasher = washer;
            washer = null;
        }
        if (fridgeItem != null && !fridgeItem.NextDay())
        {
            GetComponent<Npc>().ChangeRelationshipsOnUsedItems(fridgeItem, true, true);
            lastFridge = fridgeItem;
            fridgeItem = null;
        }
        
    }

    public object[] UseFood() //0 if founded, 1 quality, 2 bought from
    {
        bool end = false;
        int qualityResult = 0; // 0 = bad quality, 1 = medium quality, 2 = high quality
        WorldState.SalesmanCharacterName boughtFrom;

        for (int i = 0 ; i < usableItems.Count && !end; i++)
        {
            if(usableItems[i].itemType == Item.ItemType.FOOD)
            {
                end = true;
                boughtFrom = usableItems[i].boughtFrom;
                if(usableItems[i].itemQuality <20)
                {
                    qualityResult = 0;
                }
                else if(usableItems[i].itemQuality < 80)
                {
                    qualityResult = 1;
                }
                else
                {
                    qualityResult = 2;
                }
                foodStored--;
                usableItems.RemoveAt(i);

                return new object[] { end, qualityResult, boughtFrom };
            }
        }
        
        return new object[] { end, qualityResult, WorldState.SalesmanCharacterName.START_ITEM }; //no food in inventory
    }

    public object[] CleanCloth() // 0 Cloth state, 1 bought from
    {
        if(washer == null)
            return new object[] { 0, WorldState.SalesmanCharacterName.START_ITEM };

        int end = 0; //return values: 0 no washer or detergent, 1 dirty cloth, 2 clean cloth
        WorldState.SalesmanCharacterName boughtFrom;

        for (int i = 0; i < usableItems.Count && end == 0; i++)
        {
            if (usableItems[i].itemType == Item.ItemType.DETERGENT)
            {
                //quality changes on washer and cloth state after clean. Magic numbers will be replaced later
                if(usableItems[i].itemQuality < 20)
                {
                    washer.itemSpectedLifeTime--; //reduce lifetime                 
                }
                else if(usableItems[i].itemQuality < 50 && Random.Range(0, 100) < usableItems[i].itemQuality * 2)
                {
                    washer.itemSpectedLifeTime--;
                }
                else if(usableItems[i].itemQuality > 80)
                {
                    washer.itemSpectedLifeTime++;
                }
                end = Random.Range(0f, 100f) > usableItems[i].itemQuality * 2 ? 1 : 2;
                boughtFrom = usableItems[i].boughtFrom;

                usableItems.RemoveAt(i);

                return new object[] { end, boughtFrom };
            }
        }

        return new object[] { end, WorldState.SalesmanCharacterName.START_ITEM }; //no detergent
    }

    public void DayOne() //add starting items
    {
        fridgeItem = new Item(Item.ItemType.FRIDGE);
        washer = new Item(Item.ItemType.WASHER);
        usableItems.Add(new Item(Item.ItemType.DETERGENT));
        usableItems.Add(new Item(Item.ItemType.DETERGENT));
        for (int i = 0; i < 3; i++)
        {
            foodStored++;
            usableItems.Add(new Item(Item.ItemType.FOOD));
        }
    }

    

    public void Test()
    {
        foodStored++;
        usableItems.Add(new Item(Item.ItemType.FOOD));
    }
}

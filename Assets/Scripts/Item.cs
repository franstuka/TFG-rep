using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public enum ItemType { FOOD, DETERGENT, FRIDGE, WASHER}
    public ItemType itemType;
    public float itemValue;
    public float itemQuality; //percentual
    public int itemSpectedLifeTime; //days
    public int itemLifeTime; //days
    public int itemBaseLifeTime; //days
    public WorldState.SalesmanCharacterName boughtFrom;

    public Item(ItemType itemType, float itemValue, float itemQuality, int itemSpectedLifeTime, int itemBaseLifeTime, WorldState.SalesmanCharacterName boughtFrom)//params
    {
        this.itemType = itemType;
        this.itemValue = itemValue;
        this.itemQuality = itemQuality;
        this.itemSpectedLifeTime = itemSpectedLifeTime;
        this.boughtFrom = boughtFrom;
        this.itemBaseLifeTime = itemBaseLifeTime;
        itemLifeTime = 0;
    }
    public Item(ItemType itemType)//for random starting items
    {
        this.itemType = itemType;
        itemValue = 0;
        itemQuality = Random.Range(0f,100f);
        switch (itemType)
        {
            case ItemType.DETERGENT:
                {
                    itemSpectedLifeTime = WorldState.DetergentBaseLifetime;
                    itemBaseLifeTime = WorldState.DetergentBaseLifetime;
                    itemLifeTime = Mathf.RoundToInt(itemSpectedLifeTime * Random.Range(0f,1f));
                    break;
                }
            case ItemType.FOOD:
                {
                    itemSpectedLifeTime = WorldState.FoodBaseLifetime;
                    itemBaseLifeTime = WorldState.FoodBaseLifetime;

                    itemLifeTime = Mathf.RoundToInt(itemSpectedLifeTime * Random.Range(0f, 1f));
                    break;
                }
            case ItemType.FRIDGE:
                {
                    itemSpectedLifeTime = WorldState.FridgeBaseLifetime;
                    itemBaseLifeTime = WorldState.FridgeBaseLifetime;
                    itemLifeTime = Mathf.RoundToInt(itemSpectedLifeTime * Random.Range(0f, 1f));
                    break;
                }
            case ItemType.WASHER:
                {
                    itemSpectedLifeTime = WorldState.WasherBaseLifetime;
                    itemBaseLifeTime = WorldState.WasherBaseLifetime;
                    itemLifeTime = Mathf.RoundToInt(itemSpectedLifeTime * Random.Range(0f, 1f));
                    break;
                }
            default:
                {
                    Debug.LogError("On generate initial items, item don't recognised");
                    break;
                }

        }
        boughtFrom = WorldState.SalesmanCharacterName.START_ITEM;
    }

    public bool NextDay()
    {
        itemLifeTime++;

        if(itemLifeTime > itemSpectedLifeTime)
        {
            //destroy
            return false; //item broken or in bad state
        }
        return true;
    }

     
}

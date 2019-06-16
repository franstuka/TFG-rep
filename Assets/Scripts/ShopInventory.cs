using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : MonoBehaviour
{
    //const
    public const float ShopRent = 35f; 
    public const float FrigdeBasePrice = 400f;
    public const float WasherBasePrice = 300f;
    public const float FoodBasePrice = 5f;
    public const float DetergentBasePrice = 10f;

    //Shop config
    public float MaxItemQuality = 90f;
    public float MinItemQuality = 10f;
    public float ProfitMargin = 15f;

    //Shop inventory and state
    
    private VisibleItemsInShop itemsVisible;
    public List<Item> FridgeList = new List<Item>();
    public List<Item> WasherList = new List<Item>();
    public List<Item> FoodList = new List<Item>();
    public List<Item> DetergentList = new List<Item>();

    private void Awake()
    {
        itemsVisible = GetComponent<VisibleItemsInShop>();
    }

    public void AddFrigdeToStore(WorldState.SalesmanCharacterName buyedFrom)
    {
        float quality = Random.Range(MinItemQuality, MaxItemQuality);
        Item item = new Item(Item.ItemType.FRIDGE, FrigdeBasePrice * (ProfitMargin + 100) / 100, quality, 
            Mathf.RoundToInt(WorldState.FridgeBaseLifetime / 2 + WorldState.FridgeBaseLifetime * quality/ 100), WorldState.FridgeBaseLifetime, buyedFrom);
        FridgeList.Add(item);
        itemsVisible.ChangeFridgeItemsDisplayed(true, FridgeList.Count);
    }
    public void AddWasherToStore(WorldState.SalesmanCharacterName buyedFrom)
    {
        float quality = Random.Range(MinItemQuality, MaxItemQuality);
        Item item = new Item(Item.ItemType.WASHER, WasherBasePrice * (ProfitMargin + 100) / 100, quality, 
            Mathf.RoundToInt(WorldState.WasherBaseLifetime / 2 + WorldState.WasherBaseLifetime * quality / 100), WorldState.WasherBaseLifetime, buyedFrom);
        WasherList.Add(item);
        itemsVisible.ChangeWasherItemsDisplayed(true, WasherList.Count);
    }
    public void AddFoodToStore(WorldState.SalesmanCharacterName buyedFrom)
    {
        float quality = Random.Range(MinItemQuality, MaxItemQuality);
        Item item = new Item(Item.ItemType.FOOD, FoodBasePrice * (ProfitMargin + 100) / 100, quality, 
            Mathf.RoundToInt(WorldState.FoodBaseLifetime / 2 + WorldState.FoodBaseLifetime * quality / 100), WorldState.FoodBaseLifetime, buyedFrom);
        FoodList.Add(item);
        itemsVisible.ChangeFoodItemsDisplayed(true, FoodList.Count);
    }
    public void AddDetergentToStore(WorldState.SalesmanCharacterName buyedFrom)
    {
        float quality = Random.Range(MinItemQuality, MaxItemQuality);
        Item item = new Item(Item.ItemType.DETERGENT, DetergentBasePrice * (ProfitMargin + 100) / 100, quality, 
            Mathf.RoundToInt(WorldState.DetergentBaseLifetime / 2 + WorldState.DetergentBaseLifetime * quality/ 100), WorldState.DetergentBaseLifetime, buyedFrom);
        DetergentList.Add(item);
        itemsVisible.ChangeDetergentItemsDisplayed(true, DetergentList.Count);
    }
    
    public void DayOne(WorldState.SalesmanCharacterName buyedFrom)
    {
        itemsVisible.DayOne();
    }

    public void ItemBought(Item item)
    {
        switch(item.itemType)
        {
            case Item.ItemType.DETERGENT:
                {
                    DetergentList.Remove(item);
                    GetComponent<Salesman>().money += item.itemValue;
                    itemsVisible.ChangeDetergentItemsDisplayed(false, DetergentList.Count);
                    break;
                }
            case Item.ItemType.FOOD:
                {
                    FoodList.Remove(item);
                    GetComponent<Salesman>().money += item.itemValue;
                    itemsVisible.ChangeFoodItemsDisplayed(false, FoodList.Count);
                    break;
                }
            case Item.ItemType.FRIDGE:
                {
                    FridgeList.Remove(item);
                    GetComponent<Salesman>().money += item.itemValue;
                    itemsVisible.ChangeFridgeItemsDisplayed(false, FridgeList.Count);
                    break;
                }
            case Item.ItemType.WASHER:
                {
                    WasherList.Remove(item);
                    GetComponent<Salesman>().money += item.itemValue;
                    itemsVisible.ChangeWasherItemsDisplayed(false, WasherList.Count);
                    break;
                }
            default:
                {
                    Debug.LogError("Unknow Item");
                    break;
                }
        }
    }
}

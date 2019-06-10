﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salesman : MonoBehaviour //maybe can inherit from npc, but salesman don't need all stats that npc has
{
    public enum ShopState { OPEN, CLOSED};
    public enum BuySell { CHEAP, BALANCE, QUALITY };

    [SerializeField] private ShopState shopState;
    private ShopInventory ShopInventory;
    [SerializeField] private List<GameObject> BuyQueue;
    [SerializeField] private List<Transform> TransformsQueuePositition;
    private Personality personality;
    private SalesmanBehaviour salesmanBehaviour;
    public BuySell buySell;
    public WorldState.SalesmanCharacterName characterName;
    [SerializeField] private Transform shopStartPoint;
    [SerializeField] private Transform shopExitPoint;

    public float money = 500;
    [SerializeField] private int DaysInRedNumbers = 0;

    private const int DaysInRedNumbersToClose = 3;

    private Dictionary<int, float> npcsRelationships;

    private void Awake()
    {
        ShopInventory = GetComponent<ShopInventory>();
        personality = GetComponent<Personality>();
        salesmanBehaviour = GetComponent<SalesmanBehaviour>();

        shopState = ShopState.OPEN;
        BuyQueue = new List<GameObject>();
        npcsRelationships = new Dictionary<int, float>();
    }

    public void DayOne()
    {
        ShopInventory.DayOne(characterName);
        money = salesmanBehaviour.ResupplyShop(money, ref ShopInventory, characterName); //buy staff
    }

    public void NextDay()
    {
        money -= ShopInventory.ShopRent;

        if (money < 0f)
        {
            if(DaysInRedNumbers >= DaysInRedNumbersToClose) //close shop
            {
                shopState = ShopState.CLOSED;
                gameObject.SetActive(false);
                WorldState.instance.activeSalesman--;
                Debug.Log("ME HE ARRUINADO " + gameObject.name);
            }
            else
            {
                DaysInRedNumbers++;
            }
        }
        else
        {
            if(DaysInRedNumbers > 0)
                DaysInRedNumbers = 0;
            money = salesmanBehaviour.ResupplyShop(money,ref ShopInventory,characterName);
        }
        WorldState.instance.hud.UpdateInventoryAndMoneyHud();
    }

    public Transform GetStartPosition()
    {
        return shopStartPoint;
    }

    public ShopState GetShopState()
    {
        return shopState;
    }

    public bool IsQueueFull()
    {
        return BuyQueue.Count == TransformsQueuePositition.Count;
    }

    public Transform InsertInQueueLastPosition(GameObject npc)
    {
        if(!IsQueueFull())
        {
            BuyQueue.Add(npc);
            return TransformsQueuePositition[BuyQueue.Count -1];
        }
        else
        {
            return npc.GetComponent<Npc>().GetStartPosition();
        }
    }

    public void UpdateQueue()
    {
        if(BuyQueue.Count != 0) //if the second element in queue arrives later than the first end, this will execute twice
        {
            BuyQueue.RemoveAt(0);

            for (int i = 0; i < BuyQueue.Count; i++)
            {
                BuyQueue[i].GetComponent<Npc>().GoToPosition(TransformsQueuePositition[i]);
                BuyQueue[i].GetComponent<BuyBehaviour>().SetQueuePosition(i);
            }
        }
        
    }

    public void  BuyAction(Item.ItemType itemType)
    {
        
    }

    public void CheckAction()
    {

    }

    public Transform GetShopExitPoint()
    {
        return shopExitPoint;
    }

    public int GetLastPositionInQueue()
    {
        return BuyQueue.Count;
    }

    public ShopInventory GetShopInventory()
    {
        return ShopInventory;
    }

}
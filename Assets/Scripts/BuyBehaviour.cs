using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Npc))]
[RequireComponent(typeof(Inventory))]
public class BuyBehaviour : MonoBehaviour
{
    //relationships constants
    public const float positiveReacctionToEndurance = 65f;
    public const float negativeReacctionToEndurance = 35f;

    //to take in counter the price preferences
    public const float mediumPriceRelevanceBonusScore = 4f;
    public const float highPriceRelevanceBonusScore = 10f;


    public enum BuyState { NEED_TO_BUY, GOING_TO_SEE, ON_ROUTE, ON_QUEUE, PLANNED, BLOCKED, END };
    public enum BuyPreference {CHEAP, BALANCE, QUALITY };
    public enum PriceRelevance {NONE,MEDIUM,IMPORTANT };

    public float highSearchMultipleShopsProbability = 90f;
    public float mediumSearchMultipleShopsProbability = 40f;
    public float lowSearchMultipleShopsProbability = 10f;
    public BuyPreference buyPreference;
    public PriceRelevance priceRelevance;

    public float ratioRelationshipVsBetterPriceSelection = 50f;
    public List<WorldState.SalesmanCharacterName> shopsChecked = new List<WorldState.SalesmanCharacterName>();
    public List<WorldState.SalesmanCharacterName> shopToBuy = new List<WorldState.SalesmanCharacterName>();
    public List<Item.ItemType> itemToBuyType = new List<Item.ItemType>();
    public int positionInQueue;
    public BuyState buyState;
    private bool coroutineStarted;

    private const float WaitTimeToBuy = 4f;
    private const float ProbabilityToSeeAnotherShop = 35f;
    private void OnEnable()
    {
        shopsChecked = new List<WorldState.SalesmanCharacterName>();
        shopToBuy = new List<WorldState.SalesmanCharacterName>();
        itemToBuyType = new List<Item.ItemType>();

        if (buyState == BuyState.NEED_TO_BUY)
        {
            buyState = BuyState.GOING_TO_SEE;
            GoToSeeShop();
        }
        else
        {
            buyState = BuyState.END;
        }
    }

    private void GoToSeeShop()
    {
        GoToQueue(GetBetterSalesman());
    }

    private bool MakeBuyDecision() //return if need more money to buy all, return is optional
    {
        Inventory inventory = GetComponent<Inventory>();
        float moneyPrevision = GetComponent<Npc>().money;
        bool end = false;
        bool notEnoughMoney = false;
        int buyedItems = 0;

        //we try to buy first in the better shop selected before
        //first, the frigde decision
        if (ShouldBuyFrigde(ref inventory, moneyPrevision))
        {
            for (int i = 0; i < shopsChecked.Count && !end; i++)
            {
                for (int j = 0; j < WorldState.instance.SalesmanList.Count && !end; j++)
                {
                    if (WorldState.instance.SalesmanList[j].GetComponent<Salesman>().characterName == shopsChecked[i])
                    {
                        if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().FridgeList.Count > 0) //salesman has avaible frigdes
                        {
                            if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().FridgeList[0].itemValue < moneyPrevision) //npc has more money
                            {
                                //set in list to buy
                                end = true;
                                shopToBuy.Add(shopsChecked[i]);
                                itemToBuyType.Add(Item.ItemType.FRIDGE);
                                moneyPrevision -= WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().FridgeList[0].itemValue;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            if (!end)
            {
                notEnoughMoney = true;
            }
        }

        end = false;

        if (ShouldBuyWasher(ref inventory, moneyPrevision))
        {
            for (int i = 0; i < shopsChecked.Count && !end; i++)
            {
                for (int j = 0; j < WorldState.instance.SalesmanList.Count && !end; j++)
                {
                    if (WorldState.instance.SalesmanList[j].GetComponent<Salesman>().characterName == shopsChecked[i])
                    {
                        if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().WasherList.Count > 0) //salesman has avaible 
                        {
                            if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().WasherList[0].itemValue < moneyPrevision) //npc has more money
                            {
                                //set in list to buy
                                end = true;

                                shopToBuy.Add(shopsChecked[i]);
                                itemToBuyType.Add(Item.ItemType.WASHER);
                                moneyPrevision -= WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().WasherList[0].itemValue;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            if (!end)
            {
                notEnoughMoney = true;
            }
        }
        end = false;
        buyedItems = 0;
        if (ShouldBuyFood(ref inventory, moneyPrevision, buyedItems))
        {
            for (int i = 0; i < shopsChecked.Count && !end; i++)
            {
                for (int j = 0; j < WorldState.instance.SalesmanList.Count && !end; j++)
                {
                    if (WorldState.instance.SalesmanList[j].GetComponent<Salesman>().characterName == shopsChecked[i])
                    {
                        if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().FoodList.Count > 0) //salesman has avaible 
                        {
                            if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().FoodList[0].itemValue < moneyPrevision) //npc has more money
                            {
                                //set in list to buy
                                end = true;
                                shopToBuy.Add(shopsChecked[i]);
                                itemToBuyType.Add(Item.ItemType.FOOD);
                                moneyPrevision -= WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().FoodList[0].itemValue;
                                buyedItems++;
                                while (ShouldBuyFood(ref inventory, moneyPrevision, buyedItems)) //add until get all food needed
                                {
                                    if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().FoodList.Count > 0)
                                    {
                                        if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().FoodList[0].itemValue < moneyPrevision)
                                        {
                                            shopToBuy.Add(shopsChecked[i]);
                                            itemToBuyType.Add(Item.ItemType.FOOD);
                                            moneyPrevision -= WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().FoodList[0].itemValue;
                                            buyedItems++;
                                        }
                                        else
                                        {
                                            notEnoughMoney = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break; //no items in shop
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        end = false;
        buyedItems = 0;

        if (ShouldBuyDetergent(ref inventory, moneyPrevision, buyedItems))
        {
            for (int i = 0; i < shopsChecked.Count && !end; i++)
            {
                for (int j = 0; j < WorldState.instance.SalesmanList.Count && !end; j++)
                {
                    if (WorldState.instance.SalesmanList[j].GetComponent<Salesman>().characterName == shopsChecked[i])
                    {
                        if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().DetergentList.Count > 0) //salesman has avaible frigdes
                        {
                            if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().DetergentList[0].itemValue < moneyPrevision) //npc has more money
                            {
                                //set in list to buy
                                end = true;
                                shopToBuy.Add(shopsChecked[i]);
                                itemToBuyType.Add(Item.ItemType.DETERGENT);
                                moneyPrevision -= WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().DetergentList[0].itemValue;
                                buyedItems++;
                                while (ShouldBuyDetergent(ref inventory, moneyPrevision, buyedItems)) //add until get all detergent needed
                                {
                                    if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().DetergentList.Count > 0) //salesman has avaible frigdes
                                    {
                                        if (WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().DetergentList[0].itemValue < moneyPrevision)
                                        {
                                            shopToBuy.Add(shopsChecked[i]);
                                            itemToBuyType.Add(Item.ItemType.DETERGENT);
                                            moneyPrevision -= WorldState.instance.SalesmanList[j].GetComponent<ShopInventory>().DetergentList[0].itemValue;
                                            buyedItems++;
                                        }
                                        else
                                        {
                                            notEnoughMoney = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break; //no items in shop
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        return notEnoughMoney || shopToBuy.Count == 0;
    }

    private WorldState.SalesmanCharacterName GetBetterSalesman()
    {
        WorldState.SalesmanCharacterName betterScore = WorldState.SalesmanCharacterName.START_ITEM;
        float bestScore = float.MinValue;
        float auxScore = float.MinValue;
        bool wasVisited = false;
        for (int i = 0; i < WorldState.instance.SalesmanList.Count && WorldState.instance.SalesmanList.Count > shopsChecked.Count; i++)
        {
            if (WorldState.instance.SalesmanList[i].GetComponent<Salesman>().GetShopState() == Salesman.ShopState.CLOSED) //shop closed
            {
                continue;
            }

            wasVisited = false;
            for (int j = 0; j < shopsChecked.Count; j++) //check if this salesman was visited
            {
                if (shopsChecked[j] == WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName)
                {
                    wasVisited = true;
                    break;
                }
            }
            if (wasVisited)
            {
                continue;
            }

            //Get price preference score
            auxScore = 0;
            switch(priceRelevance)
            {
                case PriceRelevance.NONE:
                    {
                        break;
                    }
                case PriceRelevance.MEDIUM:
                    {
                        if (buyPreference == BuyPreference.BALANCE && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.BALANCE)
                        {
                            auxScore = mediumPriceRelevanceBonusScore;
                        }
                        else if (buyPreference == BuyPreference.CHEAP && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.CHEAP)
                        {
                            auxScore = mediumPriceRelevanceBonusScore;
                        }
                        else if (buyPreference == BuyPreference.QUALITY && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.QUALITY)
                        {
                            auxScore = mediumPriceRelevanceBonusScore;
                        }
                        break;
                    }
                case PriceRelevance.IMPORTANT:
                    {
                        if (buyPreference == BuyPreference.BALANCE && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.BALANCE)
                        {
                            auxScore = highPriceRelevanceBonusScore;
                        }
                        else if (buyPreference == BuyPreference.CHEAP && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.CHEAP)
                        {
                            auxScore = highPriceRelevanceBonusScore;
                        }
                        else if (buyPreference == BuyPreference.QUALITY && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.QUALITY)
                        {
                            auxScore = highPriceRelevanceBonusScore;
                        }
                        else
                        {
                            auxScore = -mediumPriceRelevanceBonusScore;
                        }
                        break;
                    }
            }

            //get final score adding the relationship factor
            if (GetComponent<Npc>().GetSalesmanRelationships(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName) >= 0)
            {
                auxScore += GetComponent<SocialBehaviour>().GetInteractionModifiers(WorldState.instance.SalesmanList[i].GetComponent<Personality>().npcPersonality)
                * GetComponent<Npc>().GetSalesmanRelationships(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName) + 
                + GetComponent<SocialBehaviour>().GetInteractionModifiers(WorldState.instance.SalesmanList[i].GetComponent<Personality>().npcPersonality); //this line is for solve the initial state
            }
            else //negative relationship, the modifier must divide because is a negative value
            {
                auxScore += GetComponent<Npc>().GetSalesmanRelationships(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName) /
                    GetComponent<SocialBehaviour>().GetInteractionModifiers(WorldState.instance.SalesmanList[i].GetComponent<Personality>().npcPersonality);
            }
            if (auxScore > bestScore)
            {
                bestScore = auxScore;
                betterScore = WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName;
            }
        }

        if (betterScore == WorldState.SalesmanCharacterName.START_ITEM)
        {
            Debug.LogError("Salesman returned is grong");
        }

        return betterScore;
    }

    public WorldState.SalesmanCharacterName GetPrefferedSalesman() //for hud
    {
        WorldState.SalesmanCharacterName betterScore = WorldState.SalesmanCharacterName.START_ITEM;
        float bestScore = float.MinValue;
        float auxScore = float.MinValue;
        for (int i = 0; i < WorldState.instance.SalesmanList.Count; i++)
        {
            if (WorldState.instance.SalesmanList[i].GetComponent<Salesman>().GetShopState() == Salesman.ShopState.CLOSED) //shop closed
            {
                continue;
            }

            auxScore = 0;
            switch (priceRelevance)
            {
                case PriceRelevance.NONE:
                    {
                        break;
                    }
                case PriceRelevance.MEDIUM:
                    {
                        if (buyPreference == BuyPreference.BALANCE && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.BALANCE)
                        {
                            auxScore = mediumPriceRelevanceBonusScore;
                        }
                        else if (buyPreference == BuyPreference.CHEAP && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.CHEAP)
                        {
                            auxScore = mediumPriceRelevanceBonusScore;
                        }
                        else if (buyPreference == BuyPreference.QUALITY && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.QUALITY)
                        {
                            auxScore = mediumPriceRelevanceBonusScore;
                        }
                        break;
                    }
                case PriceRelevance.IMPORTANT:
                    {
                        if (buyPreference == BuyPreference.BALANCE && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.BALANCE)
                        {
                            auxScore = highPriceRelevanceBonusScore;
                        }
                        else if (buyPreference == BuyPreference.CHEAP && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.CHEAP)
                        {
                            auxScore = highPriceRelevanceBonusScore;
                        }
                        else if (buyPreference == BuyPreference.QUALITY && WorldState.instance.SalesmanList[i].GetComponent<Salesman>().buySell == Salesman.BuySell.QUALITY)
                        {
                            auxScore = highPriceRelevanceBonusScore;
                        }
                        else
                        {
                            auxScore = -mediumPriceRelevanceBonusScore;
                        }
                        break;
                    }
            }

            if (GetComponent<Npc>().GetSalesmanRelationships(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName) >= 0)
            {
                auxScore += GetComponent<SocialBehaviour>().GetInteractionModifiers(WorldState.instance.SalesmanList[i].GetComponent<Personality>().npcPersonality)
                * GetComponent<Npc>().GetSalesmanRelationships(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName)
                + GetComponent<SocialBehaviour>().GetInteractionModifiers(WorldState.instance.SalesmanList[i].GetComponent<Personality>().npcPersonality); //this line is for solve the initial state
            }
            else //negative relationship, the modifier must divide because is a negative value
            {
                auxScore += GetComponent<Npc>().GetSalesmanRelationships(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName) /
                    GetComponent<SocialBehaviour>().GetInteractionModifiers(WorldState.instance.SalesmanList[i].GetComponent<Personality>().npcPersonality);
            }
            if (auxScore > bestScore)
            {
                bestScore = auxScore;
                betterScore = WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName;
            }
        }

        if (betterScore == WorldState.SalesmanCharacterName.START_ITEM)
        {
            Debug.LogError("Salesman returned is grong");
        }

        return betterScore;
    }


    private void GoToBuy()
    {
        GoToQueue(shopToBuy[0]);
    }

    private void GoToQueue(WorldState.SalesmanCharacterName salesman)
    {
        Transform position;
        for (int i = 0; i < WorldState.instance.SalesmanList.Count; i++)
        {
            if(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName == salesman)
            {
                
                position = WorldState.instance.SalesmanList[i].GetComponent<Salesman>().InsertInQueueLastPosition(gameObject);
                if(position == GetComponent<Npc>().GetStartPosition()) //queue full
                {
                    //wait some seconds
                    StartCoroutine(WaitToQueueDrop());
                }
                else
                {
                    positionInQueue = WorldState.instance.SalesmanList[i].GetComponent<Salesman>().GetLastPositionInQueue() -1; //if we enter here, salesman has included the npc in queue
                    if(buyState == BuyState.GOING_TO_SEE)
                        shopsChecked.Add(salesman);// we should add this later when the character gets the first position
                                                   //, but then we have to save this value somewhere, soo this is a easy solution   
                    buyState = BuyState.ON_ROUTE;
                                                   
                }
                GetComponent<Npc>().GoToPosition(position);
            }
        }
    }

    public void SetQueuePosition(int queuePos)
    {
        positionInQueue = queuePos;
    }

    public void CheckInPosition()
    {
        if(positionInQueue == 0 && buyState == BuyState.ON_QUEUE)
        {
            if(shopToBuy.Count == 0) //is checking the shop
            {        
                if(!coroutineStarted)
                {
                    Debug.Log(gameObject.name + " is checking the shop");
                    coroutineStarted = true;
                    StartCoroutine(SeeShop());
                }
            }
            else //is going to buy on shop
            {               
                if (!coroutineStarted)
                {
                    Debug.Log(gameObject.name + " is buying in the shop");
                    coroutineStarted = true;
                    StartCoroutine(BuyShop());
                }          
            }
        }
        else if(buyState == BuyState.GOING_TO_SEE)
        {
            GoToSeeShop();
        }
        else if(buyState == BuyState.PLANNED)
        {
            GoToBuy();
        }
    }

    public void EnteredInQueue()
    {
        buyState = BuyState.ON_QUEUE;
    }

    IEnumerator BuyShop()
    {
        yield return new WaitForSeconds(WaitTimeToBuy);
        WorldState.SalesmanCharacterName searchedSalesman = shopToBuy[0];
        int salesmanNum = 0;
        int searchPos = 0;
        Item itemToBuy;
        bool allBought = true;

        //search the salesman index
        while (salesmanNum < WorldState.instance.SalesmanList.Count)
        {
            if (WorldState.instance.SalesmanList[salesmanNum].GetComponent<Salesman>().characterName == shopToBuy[0])
            {
                break;
            }
            else
            {
                salesmanNum++;
            }
        }    

        //buy
        while(searchPos < shopToBuy.Count)
        {
            if(shopToBuy[searchPos] == searchedSalesman) //buy in this shop
            {
                //search item to buy
                itemToBuy = null;
                switch (itemToBuyType[searchPos])
                {
                    case Item.ItemType.FRIDGE:
                        {
                            if (WorldState.instance.SalesmanList[salesmanNum].GetComponent<ShopInventory>().FridgeList.Count != 0)
                                itemToBuy = WorldState.instance.SalesmanList[salesmanNum].GetComponent<ShopInventory>().FridgeList[0];
                            break;
                        }
                    case Item.ItemType.WASHER:
                        {
                            if (WorldState.instance.SalesmanList[salesmanNum].GetComponent<ShopInventory>().WasherList.Count != 0)
                                itemToBuy = WorldState.instance.SalesmanList[salesmanNum].GetComponent<ShopInventory>().WasherList[0];
                            break;
                        }
                    case Item.ItemType.FOOD:
                        {
                            if (WorldState.instance.SalesmanList[salesmanNum].GetComponent<ShopInventory>().FoodList.Count != 0)
                                itemToBuy = WorldState.instance.SalesmanList[salesmanNum].GetComponent<ShopInventory>().FoodList[0];
                            break;
                        }
                    case Item.ItemType.DETERGENT:
                        {
                            if(WorldState.instance.SalesmanList[salesmanNum].GetComponent<ShopInventory>().DetergentList.Count != 0)
                                itemToBuy = WorldState.instance.SalesmanList[salesmanNum].GetComponent<ShopInventory>().DetergentList[0];
                            break;
                        }
                    default:
                        {
                            Debug.LogError("Item out of list");
                            break;
                        }
                }
                //test if have an item avaible
                if(itemToBuy != null)
                {
                    if (GetComponent<Npc>().money >= itemToBuy.itemValue)
                    {
                        //buy
                        Debug.Log(gameObject.name + " Buy " + itemToBuy.itemType);
                        GetComponent<Npc>().money -= itemToBuy.itemValue;
                        GetComponent<Inventory>().AddNewItem(itemToBuy);
                        WorldState.instance.SalesmanList[salesmanNum].GetComponent<ShopInventory>().ItemBought(itemToBuy);
                        shopToBuy.RemoveAt(searchPos);
                        itemToBuyType.RemoveAt(searchPos);
                        WorldState.instance.hud.UpdateInventoryAndMoneyHud();
                    }
                    else
                    {
                        Debug.Log(gameObject.name + " not enought money");
                        shopToBuy.RemoveAt(searchPos);
                        itemToBuyType.RemoveAt(searchPos);
                        allBought = false;
                    }    
                }
                else
                {
                    Debug.Log(gameObject.name + " item not avaible");
                    allBought = false;
                    shopToBuy.RemoveAt(searchPos);
                    itemToBuyType.RemoveAt(searchPos);
                }
               
            }
            else //next
            {
                searchPos++;
            }
        }
        //move to exit
        gameObject.GetComponent<Npc>().GoToPosition(WorldState.instance.SalesmanList[salesmanNum].GetComponent<Salesman>().GetShopExitPoint());
        WorldState.instance.SalesmanList[salesmanNum].GetComponent<Salesman>().UpdateQueue();

        //has buy all?
        if(allBought && shopToBuy.Count == 0) //all bought in all
        {
            Debug.Log(gameObject.name + " has bought all");
            buyState = BuyState.END;
            GetComponent<Npc>().CheckIfEndedAll();
        }
        else if(allBought) //all bought in this shop
        {
            Debug.Log(gameObject.name + " has bought all in this shop");
            buyState = BuyState.PLANNED;
        }
        else
        {
            if(MakeBuyDecision())
            {
                buyState = BuyState.END; //has not enought money or stock
                GetComponent<Npc>().CheckIfEndedAll();
            }              
            else
                buyState = BuyState.PLANNED;   
        }
        coroutineStarted = false;
    }

    IEnumerator SeeShop()
    {
        yield return new WaitForSeconds(WaitTimeToBuy);
        for (int i = 0; i < WorldState.instance.SalesmanList.Count; i++) //move to exit
        {
            if(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName == shopsChecked[shopsChecked.Count-1])
            {
                gameObject.GetComponent<Npc>().GoToPosition(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().GetShopExitPoint());
                WorldState.instance.SalesmanList[i].GetComponent<Salesman>().UpdateQueue();
            }
        }
        if(shopsChecked.Count == WorldState.instance.activeSalesman) // all open shops visited
        {
            Debug.Log(gameObject.name + " is planing the buy");
            if (MakeBuyDecision())
            {
                buyState = BuyState.END;
                GetComponent<Npc>().CheckIfEndedAll();
            }
                
            else
                buyState = BuyState.PLANNED;
            
        }
        else if(GetComponent<Npc>().GetPersonality() == Personality.PersonalityType.PERFECTIONIST )
        {
            Debug.Log(gameObject.name + " is going to see another shop");
            buyState = BuyState.GOING_TO_SEE;
        }
        else
        {
            if(Random.Range(0,100) > ProbabilityToSeeAnotherShop)
            {
                Debug.Log(gameObject.name + " is going to see another shop");
                buyState = BuyState.GOING_TO_SEE;
            }
            else
            {
                Debug.Log(gameObject.name + " is planing the buy");
                if (MakeBuyDecision())
                {
                    buyState = BuyState.END;
                    GetComponent<Npc>().CheckIfEndedAll();
                }
                else
                    buyState = BuyState.PLANNED;
            }
        }
        coroutineStarted = false;
    }

    IEnumerator WaitToQueueDrop()
    {
        yield return new WaitForSeconds(WaitTimeToBuy);
        if(buyState == BuyState.PLANNED)
        {
            GoToBuy();
        }
        else if (buyState == BuyState.GOING_TO_SEE)
        {
            GoToSeeShop();
        }
    }


    public bool ShouldBuyFood(ref Inventory inventory, float money)
    {
        if(inventory.foodStored < 2 && inventory.fridgeItem != null)
        {
            return true;
        }
        else if (inventory.foodStored == 0)
        {
            return true;
        }

        return false;
    } // not calculate if there is enough money
    public bool ShouldBuyFood(ref Inventory inventory, float money, int estimatedBuy) //this is for refeal
    {
        if (inventory.foodStored + estimatedBuy < 5 && inventory.fridgeItem != null)
        {
            return true;
        }
        else if (inventory.foodStored + estimatedBuy == 0)
        {
            return true;
        }

        return false;
    } // not calculate if there is enough money
    public bool ShouldBuyDetergent(ref Inventory inventory, float money)
    {
        if(inventory.usableItems.Count - inventory.foodStored < 2)
        {
            return true;
        }
        return false;
    } // not calculate if there is enough money
    public bool ShouldBuyDetergent(ref Inventory inventory, float money, int estimatedBuy)
    {
        if (inventory.usableItems.Count - inventory.foodStored + estimatedBuy < 3)
        {
            return true;
        }
        return false;
    } // not calculate if there is enough money

    public bool ShouldBuyFrigde(ref Inventory inventory, float money)
    {
        if(inventory.fridgeItem == null && money > ShopInventory.FrigdeBasePrice)
        {
            return true;
        }
        return false;
    }
    public bool ShouldBuyWasher(ref Inventory inventory, float money)
    {
        if (inventory.washer == null && money > ShopInventory.WasherBasePrice)
        {
            return true;
        }
        return false;
    }
}

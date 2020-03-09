using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{


    private const byte maxCleanClothes = 3;

    public float money;
    public WorldState.NPCName characterName;
    public byte cleanClothes;
    public Dictionary<WorldState.SalesmanCharacterName, float> salesmanRelationships;

    [SerializeField] private float moneyWinnedEveryDay;
    [SerializeField] private Transform shopStartPoint;

    private Inventory inventory;
    private Personality personality;
    private BuyBehaviour buyBehaviour;
    private SocialBehaviour socialBehaviour;
    private UnityStandardAssets.Characters.ThirdPerson.AICharacterControl StandardIAControl;
    //public Dictionary<WorldState.NPCName, float> npcsRelationships; //not implemented
    

    private void Awake()
    {
        StandardIAControl = GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>();
        inventory = GetComponent<Inventory>();
        personality = GetComponent<Personality>();
        buyBehaviour = GetComponent<BuyBehaviour>();
        socialBehaviour = GetComponent<SocialBehaviour>();
        //npcsRelationships = new Dictionary<WorldState.NPCName, float>();
        salesmanRelationships = new Dictionary<WorldState.SalesmanCharacterName, float>();
    }

    public Personality.PersonalityType GetPersonality()
    {
        return personality.npcPersonality;
    }

    public virtual void DayOne()
    {
        cleanClothes = maxCleanClothes;
        InitialiceDictionaries();
        inventory.DayOne();
        socialBehaviour.AvaibleInteractions = socialBehaviour.DayOne();
    }

    public virtual void NextDay()
    {
        //consume items
        if(cleanClothes > 0)
            cleanClothes--;
        object[] foodResult = inventory.UseFood();

        if ((bool)foodResult[0])
        {
            ChangeRelationshipsOnUsedFood((int)foodResult[1], (WorldState.SalesmanCharacterName)foodResult[2]);
        }
        else //not food
        {
            if(inventory.fridgeItem == null)
            {
                ChangeRelationshipsOnUsedItems(inventory.lastFridge, true, false);
                //down relationship
            }
        }

        if(cleanClothes == 0)
        {
            object[] clothResult = inventory.CleanCloth();
            switch ((int)clothResult[0])
            {
                case 0: //no detergent or washer
                    {
                        if(inventory.washer == null)
                            ChangeRelationshipsOnUsedItems(inventory.lastWasher, true, false);
                        break;
                    }
                case 1: //notclean
                    {
                        ChangeRelationshipsOnUsedDetergent(false, (WorldState.SalesmanCharacterName)clothResult[1]);
                        break;
                    }
                case 2: //clean
                    {
                        ChangeRelationshipsOnUsedDetergent(true, (WorldState.SalesmanCharacterName)clothResult[1]);
                        cleanClothes = maxCleanClothes;
                        break;
                    }
            }
        }

        inventory.UpdateInventory();

        socialBehaviour.NextDay();
        money += moneyWinnedEveryDay;
    }

    public bool GoToShop()
    {
        //social necesities
        bool needToGo = false;
        if (socialBehaviour.AvaibleInteractions >= socialBehaviour.GetInteracionsNeededToGoShop())
        {
            needToGo = true;
        }
        //buy necesities
        if(buyBehaviour.ShouldBuyFood(ref inventory, money) || buyBehaviour.ShouldBuyDetergent(ref inventory, money)
            || buyBehaviour.ShouldBuyFrigde(ref inventory, money) || buyBehaviour.ShouldBuyWasher(ref inventory, money))
        {
            buyBehaviour.buyState = BuyBehaviour.BuyState.NEED_TO_BUY;
            needToGo = true;
        }

        return needToGo;
    }

    public Transform GetStartPosition()
    {
        return shopStartPoint;
    }

    public void GoToPosition(Transform position)
    {
        StandardIAControl.SetTarget(position);
        StartCoroutine(TestIfStopped());

    }

    public float GetSalesmanRelationships(WorldState.SalesmanCharacterName name)
    {
        for (int i = 0; i < WorldState.instance.SalesmanList.Count; i++)
        {
            if(name == WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName)
            {
                return salesmanRelationships[name];
            }
        }

        Debug.LogError("Salesman " + name + " not found");
        return 0;
    }

    private IEnumerator TestIfStopped()
    {
        UnityEngine.AI.NavMeshAgent nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        yield return new WaitUntil(() => Vector3.Distance(StandardIAControl.target.position, transform.position) <= 1.5f * nav.stoppingDistance);
        //yield return new WaitForSeconds(0.4f);
        //enter in queue?
        if (buyBehaviour.buyState == BuyBehaviour.BuyState.END)
        {
            DesactivateIfIsInExit();
        }
        else if (buyBehaviour != null && buyBehaviour.buyState == BuyBehaviour.BuyState.ON_ROUTE)
        {
            buyBehaviour.EnteredInQueue();
            buyBehaviour.CheckInPosition();

            if (socialBehaviour != null)
            {
                socialBehaviour.EnteredInQueue();
            }
        }
        else if(buyBehaviour != null && buyBehaviour.buyState != BuyBehaviour.BuyState.ON_QUEUE && socialBehaviour != null && socialBehaviour.socialState == SocialBehaviour.SocialState.ON_QUEUE)
        {
            socialBehaviour.socialState = SocialBehaviour.SocialState.FREE;
            buyBehaviour.CheckInPosition();
            socialBehaviour.CheckInPosition();
        }
        else if(buyBehaviour != null && socialBehaviour != null)
        {
            buyBehaviour.CheckInPosition();
            socialBehaviour.CheckInPosition();
        } 
    }

    private void DesactivateIfIsInExit()
    {
        if (Vector3.Distance(transform.position, WorldState.instance.ShopExitPoint.position) < 3)
        {
            gameObject.SetActive(false);
            //test if next day
            WorldState.instance.TestIfPassToNextDay();
        }
    }

    public void CheckIfEndedAll()
    {
        if(buyBehaviour.buyState == BuyBehaviour.BuyState.END)
        {
            Debug.Log("Go home " + gameObject.name);
            WorldState.instance.hud.UpdateDebugText("Go home " + gameObject.name);
            GoToPosition(WorldState.instance.ShopExitPoint);
        }
    }

    //We will add the normal interaction when detergent is used correctly, food after use has 3 posible final states, so we asign 
    // a change depending on quality.
    // If an item is broke or spoiled, we modifie the relationship depending on the endurance (quality of item) and the days that 
    //broke after or before it's normal broke time.

    public void ChangeRelationshipsOnUsedItems(Item item, bool isBrokenItem, bool brokenNow)
    {
        if(item.boughtFrom != WorldState.SalesmanCharacterName.START_ITEM)
        {
            if(!brokenNow)  //down relationship a bit, this is for fridge and washer
            {
                if(item.itemQuality <= BuyBehaviour.negativeReacctionToEndurance) //and quality was bad
                {
                    salesmanRelationships[item.boughtFrom] += SocialBehaviour.negativeInteractionResult;
                }        
            }
            else if (isBrokenItem)
            {
                if(item.itemQuality <= BuyBehaviour.negativeReacctionToEndurance)
                {
                    salesmanRelationships[item.boughtFrom] += - SocialBehaviour.negativeInteractionResult * (item.itemLifeTime-item.itemBaseLifeTime);
                }
                else if (item.itemQuality >= BuyBehaviour.positiveReacctionToEndurance)
                {
                    salesmanRelationships[item.boughtFrom] += SocialBehaviour.positiveInteractionResult * (item.itemLifeTime - item.itemBaseLifeTime);
                }
                else //medium values
                {
                    salesmanRelationships[item.boughtFrom] += SocialBehaviour.neutralInteractionResult * (item.itemLifeTime - item.itemBaseLifeTime);
                }
            }
            else
            {
                Debug.LogError("Not contempled case");
            }
            WorldState.instance.hud.UpdateSalesmanRelationshipText();
        }
    }

    public void ChangeRelationshipsOnUsedDetergent(bool goodResult, WorldState.SalesmanCharacterName salesman)
    {
        if (salesman != WorldState.SalesmanCharacterName.START_ITEM)
        {
            if(goodResult)
            {
                salesmanRelationships[salesman] += SocialBehaviour.neutralInteractionResult;
            }
            else
            {
                salesmanRelationships[salesman] += SocialBehaviour.negativeInteractionResult;
            }
            WorldState.instance.hud.UpdateSalesmanRelationshipText();
        }
    }

    public void ChangeRelationshipsOnUsedFood(int qualityResult, WorldState.SalesmanCharacterName salesman) //0 bad, 1 medium, 2 good
    {
        if (salesman != WorldState.SalesmanCharacterName.START_ITEM)
        {
            switch (qualityResult)
            {
                case 0:
                    {
                        salesmanRelationships[salesman] += SocialBehaviour.negativeInteractionResult;
                        break;
                    }
                case 1:
                    {
                        salesmanRelationships[salesman] += SocialBehaviour.neutralInteractionResult;
                        break;
                    }
                case 2:
                    {
                        salesmanRelationships[salesman] += SocialBehaviour.positiveInteractionResult;
                        break;
                    }
            }
            WorldState.instance.hud.UpdateSalesmanRelationshipText();
        }
    }

    private void InitialiceDictionaries()
    {
        /* not implemented
        for (int i = 0; i < WorldState.instance.NpcGlobalList.Count; i++)
        {
            if(WorldState.instance.NpcGlobalList[i] == this.gameObject)
            {
                continue;
            }
            else
            {
                npcsRelationships.Add(WorldState.instance.NpcGlobalList[i].GetComponent<Npc>().characterName, 0f);
            }
        }
        */

        for (int i = 0; i < WorldState.instance.SalesmanList.Count; i++)
        {
            salesmanRelationships.Add(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().characterName, 0f);
        }
    }
}

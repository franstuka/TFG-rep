using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalesmanBehaviour : MonoBehaviour
{
    int maxFoodToBuy = 15;
    int maxDetergentToBuy = 10;
    int maxFridgesToBuy = 3;
    int maxWashersToBuy = 5;
    [SerializeField] float minMoneyToBuy = 150;
    [SerializeField] int minMoneyToBuyFrigdeAndWasher = 800;
    [SerializeField] float maxPercentageDifference = 0.4f;

    public float ResupplyShop(float money, ref ShopInventory inventory, WorldState.SalesmanCharacterName salesman)
    {
        //we are going to resupply if difference between a member and his min is less than maxPercentageDifference
        //the salesman will try to have all the items in shop buying the less percentage item,
        //we have to check that salesman don't buy only food and detergent, saving for get aswell fridges and washers
        float foodPercentage = (float)inventory.FoodList.Count / maxFoodToBuy;
        float detergentPercentage = (float)inventory.DetergentList.Count / maxDetergentToBuy;
        float frigdePercentage = (float)inventory.FridgeList.Count / maxFridgesToBuy;
        float washerPercentage = (float)inventory.WasherList.Count / maxWashersToBuy;
        bool end = false;

        while (!end)
        {
            if(money <= minMoneyToBuy)
            {
                end = true;
            }
            else if (money <= minMoneyToBuyFrigdeAndWasher)
            {
                if (foodPercentage < detergentPercentage)
                {
                    if (foodPercentage < (frigdePercentage + maxPercentageDifference) && foodPercentage < (washerPercentage + maxPercentageDifference))
                    {
                        inventory.AddFoodToStore(salesman);
                        money -= ShopInventory.FoodBasePrice;
                        foodPercentage = (float)inventory.FoodList.Count / maxFoodToBuy;
                    }
                    else //fridges or washers percentage it's so low
                    {
                        end = true;
                    }
                }
                else if (detergentPercentage < (frigdePercentage + maxPercentageDifference) && detergentPercentage < (washerPercentage + maxPercentageDifference))
                {
                    inventory.AddDetergentToStore(salesman);
                    money -= ShopInventory.DetergentBasePrice;
                    detergentPercentage = (float)inventory.DetergentList.Count / maxDetergentToBuy;
                }
                else //fridges or washers percentage it's so low
                {
                    end = true;
                }
            }
            else //get who's the lowest percentage
            {
                if(foodPercentage == 1 && foodPercentage == detergentPercentage && frigdePercentage == washerPercentage && frigdePercentage == detergentPercentage)
                {
                    //shop full
                    end = true;
                }
                else
                {
                    if (foodPercentage <= detergentPercentage && foodPercentage <= frigdePercentage && foodPercentage <= washerPercentage)
                    {
                        inventory.AddFoodToStore(salesman);
                        money -= ShopInventory.FoodBasePrice;
                        foodPercentage = (float)inventory.FoodList.Count / maxFoodToBuy;
                    }
                    else if (detergentPercentage <= foodPercentage && detergentPercentage <= frigdePercentage && detergentPercentage <= washerPercentage)
                    {
                        inventory.AddDetergentToStore(salesman);
                        money -= ShopInventory.DetergentBasePrice;
                        detergentPercentage = (float)inventory.DetergentList.Count / maxDetergentToBuy;
                    }
                    else if (frigdePercentage <= detergentPercentage && frigdePercentage <= foodPercentage && frigdePercentage <= washerPercentage)
                    {
                        inventory.AddFrigdeToStore(salesman);
                        money -= ShopInventory.FrigdeBasePrice;
                        frigdePercentage = (float)inventory.FridgeList.Count / maxFridgesToBuy;
                    }
                    else if (washerPercentage <= detergentPercentage && washerPercentage <= frigdePercentage && washerPercentage <= foodPercentage)
                    {
                        inventory.AddWasherToStore(salesman);
                        money -= ShopInventory.WasherBasePrice;
                        washerPercentage = (float)inventory.WasherList.Count / maxWashersToBuy;
                    }
                }
            }
        }

        return money;
    }
}

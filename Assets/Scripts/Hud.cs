using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud : MonoBehaviour
{
    public GameObject NpcInventoryTextPrefab;
    public GameObject SalesmanRelationshipTextPrefab;
    public List<GameObject> SalesmanRelationshipTextList = new List<GameObject>();
    public List<GameObject> NpcInventoryList = new List<GameObject>();
    public TMPro.TextMeshProUGUI salesmanMoney;
    public TMPro.TextMeshProUGUI dayHud;
    public List<GameObject> Headers;

    public void UpdateHud()
    {

    }

    public void UpdateSalesmanRelationshipText()
    {
        for (int i = 0; i < SalesmanRelationshipTextList.Count; i++) //the 0 value is the text guide
        {

            SalesmanRelationshipTextList[i].GetComponent<TMPro.TextMeshProUGUI>().text = WorldState.instance.NpcGlobalList[i].GetComponent<Npc>().GetPersonality() + ": ";
            SalesmanRelationshipTextList[i].GetComponent<TMPro.TextMeshProUGUI>().text += WorldState.instance.NpcGlobalList[i].GetComponent<BuyBehaviour>().GetPrefferedSalesman() + ", ";
            foreach (KeyValuePair<WorldState.SalesmanCharacterName,float> relationship in WorldState.instance.NpcGlobalList[i].GetComponent<Npc>().salesmanRelationships)
            {
                  SalesmanRelationshipTextList[i].GetComponent<TMPro.TextMeshProUGUI>().text += "       " + relationship.Value;
            }        
        }
    }

    public void UpdateSalesmanMoney()
    {
        salesmanMoney.text = "Salesman money: ";
        for (int i = 0; i < WorldState.instance.SalesmanList.Count; i++)
        {
            salesmanMoney.text += " " + WorldState.instance.SalesmanList[i].GetComponent<Personality>().npcPersonality + " ->" + 
                Mathf.RoundToInt(WorldState.instance.SalesmanList[i].GetComponent<Salesman>().money);
        }
    }

    public void UpdateNpcInventory()
    {
        for (int i = 0; i < WorldState.instance.NpcGlobalList.Count; i++)
        {
            NpcInventoryList[i].GetComponent<TMPro.TextMeshProUGUI>().text = WorldState.instance.NpcGlobalList[i].GetComponent<Npc>().GetPersonality() + ": ";

            if (WorldState.instance.NpcGlobalList[i].GetComponent<Inventory>().fridgeItem != null)
                NpcInventoryList[i].GetComponent<TMPro.TextMeshProUGUI>().text += "true; ";
            else
                NpcInventoryList[i].GetComponent<TMPro.TextMeshProUGUI>().text += "false; ";

            if (WorldState.instance.NpcGlobalList[i].GetComponent<Inventory>().washer != null)
                NpcInventoryList[i].GetComponent<TMPro.TextMeshProUGUI>().text += "true; ";
            else
                NpcInventoryList[i].GetComponent<TMPro.TextMeshProUGUI>().text += "false; ";

            NpcInventoryList[i].GetComponent<TMPro.TextMeshProUGUI>().text += WorldState.instance.NpcGlobalList[i].GetComponent<Inventory>().foodStored + "; ";
            NpcInventoryList[i].GetComponent<TMPro.TextMeshProUGUI>().text += WorldState.instance.NpcGlobalList[i].GetComponent<Inventory>().usableItems.Count - 
                WorldState.instance.NpcGlobalList[i].GetComponent<Inventory>().foodStored + "; ";

            NpcInventoryList[i].GetComponent<TMPro.TextMeshProUGUI>().text += Mathf.RoundToInt(WorldState.instance.NpcGlobalList[i].GetComponent<Npc>().money) + "; ";
            NpcInventoryList[i].GetComponent<TMPro.TextMeshProUGUI>().text += WorldState.instance.NpcGlobalList[i].GetComponent<Npc>().cleanClothes;
        }
    }

    public void UpdateInventoryAndMoneyHud()
    {
        UpdateSalesmanMoney();
        UpdateNpcInventory();
    }

    public void DayOne()
    {
        for (int i = 0; i < WorldState.instance.NpcGlobalList.Count; i++) //inicialice salesman relationship list
        {
            GameObject newHudElement = Instantiate(SalesmanRelationshipTextPrefab,GetComponentInChildren<Transform>());
            newHudElement.GetComponent<Transform>().position = new Vector3(newHudElement.GetComponent<Transform>().position.x, 
                -SalesmanRelationshipTextPrefab.GetComponent<RectTransform>().rect.height * (SalesmanRelationshipTextList.Count + 1) + transform.position.y*2, newHudElement.GetComponent<Transform>().position.z);
            SalesmanRelationshipTextList.Add(newHudElement);
        }
        for (int i = 0; i < WorldState.instance.NpcGlobalList.Count; i++) //inicialice npc inventory list
        {
            GameObject newHudElement = Instantiate(NpcInventoryTextPrefab, GetComponentInChildren<Transform>());
            newHudElement.GetComponent<Transform>().position = new Vector3(newHudElement.GetComponent<Transform>().position.x,
                -NpcInventoryTextPrefab.GetComponent<RectTransform>().rect.height * (NpcInventoryList.Count + 3) + transform.position.y * 2, newHudElement.GetComponent<Transform>().position.z);
            NpcInventoryList.Add(newHudElement);
        }

        UpdateSalesmanRelationshipText();
        UpdateInventoryAndMoneyHud();
        UpdateDay();
    }
    
    public void UpdateDay()
    {
        dayHud.text = "Day " + WorldState.instance.Day;
    }

    public void EnableOrDisableExtendedHud()
    {
        if(SalesmanRelationshipTextList[0].gameObject.activeInHierarchy) //hide hud
        {
            for (int i = 0; i < SalesmanRelationshipTextList.Count; i++)
            {
                SalesmanRelationshipTextList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < NpcInventoryList.Count; i++)
            {
                NpcInventoryList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < Headers.Count; i++)
            {
                Headers[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < SalesmanRelationshipTextList.Count; i++)
            {
                SalesmanRelationshipTextList[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < NpcInventoryList.Count; i++)
            {
                NpcInventoryList[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < Headers.Count; i++)
            {
                Headers[i].gameObject.SetActive(true);
            }
        }
    }
}

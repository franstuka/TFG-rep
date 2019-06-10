using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    #region singleton
    public static WorldState instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        //NpcGlobalList = new List<GameObject>();
        //SalesmanList = new List<GameObject>();
    }
    #endregion

    
    //constants for instance initial items
    public const int FridgeBaseLifetime = 30;
    public const int WasherBaseLifetime = 20;
    public const int FoodBaseLifetime = 7;
    public const int DetergentBaseLifetime = 50;

    //some shop variables
    public Transform ShopExitPoint;

    public enum NPCName { BLAB1, RUDE1, FRIENDLY1, SOCIABLE1, LONELY1, SELFISH1, SAVER1, PERFECTIONIST1,
        BLAB2, RUDE2, FRIENDLY2, SOCIABLE2, LONELY2, SELFISH2, SAVER2, PERFECTIONIST2, RUDE3, RUDE4, RUDE5 };

    public enum SalesmanCharacterName { START_ITEM, RUDE_SALESMAN, LONELY_SALESMAN, SOCIABLE_SALESMAN };

    public List<GameObject> NpcPrefabs;
    public List<GameObject> SalesmanPrefabs;

    public List<GameObject> NpcGlobalList = new List<GameObject>();
    public List<GameObject> SalesmanList = new List<GameObject>();
    public Hud hud;
    public int Day = 1;
    public int activeSalesman;


    private void Start() //Day One
    {
        activeSalesman = SalesmanList.Count;
        DayOne();
    }

    private void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            NextDay();
        }
        if (Input.GetKeyDown("2"))
        {
            hud.EnableOrDisableExtendedHud();
        }
    }

    private void DayOne()
    {
        //spawn all prefabs
        for (int i = 0; i < NpcPrefabs.Count; i++)
        {
            NpcGlobalList.Add(Instantiate(NpcPrefabs[i]));
            NpcGlobalList[i].gameObject.name = NpcPrefabs[i].gameObject.name;
        }
        /*for (int i = 0; i < SalesmanPrefabs.Count; i++)
        {
            SalesmanList.Add(Instantiate(SalesmanPrefabs[i], SalesmanPrefabs[i].GetComponent<Salesman>().GetStartPosition().position,
                    SalesmanPrefabs[i].GetComponent<Salesman>().GetStartPosition().rotation));
        }*/
        for (int i = 0; i < SalesmanPrefabs.Count; i++)
        {
            SalesmanList[i].transform.SetPositionAndRotation(SalesmanList[i].GetComponent<Salesman>().GetStartPosition().position,
                    SalesmanList[i].GetComponent<Salesman>().GetStartPosition().rotation);
        }
        // disable all npc spawned

        DesactiveAllNpc();

        for (int i = 0; i < NpcPrefabs.Count; i++)
        {
            NpcGlobalList[i].GetComponent<Npc>().DayOne();
        }
        for (int i = 0; i < SalesmanPrefabs.Count; i++)
        {
            SalesmanList[i].GetComponent<Salesman>().DayOne();

        }

        ActivateNpcWithNecesities();
        hud.DayOne();
        TestIfPassToNextDay();
    }

    private void NextDay()
    {
        Debug.Log("END OF DAY " + Day);
        for (int i = 0; i < NpcGlobalList.Count; i++)
        {
            NpcGlobalList[i].GetComponent<Npc>().NextDay();
        }
        for (int i = 0; i < SalesmanList.Count; i++)
        {
            if(SalesmanList[i].GetComponent<Salesman>().GetShopState() == Salesman.ShopState.OPEN)
            {
                SalesmanList[i].GetComponent<Salesman>().NextDay();
            }    
        }

        DesactiveAllNpc();
        Day++;
        if(Day % 7 == 0) //On Sunday shops are closed
        {
            NextDay();
        }
        else
        {
            ActivateNpcWithNecesities();
            TestIfPassToNextDay();
        }

        hud.UpdateDay();
    }

    private void ActivateNpcWithNecesities()
    {
        for (int i = 0; i < NpcGlobalList.Count; i++)
        {
            if(NpcGlobalList[i].GetComponent<Npc>().GoToShop())
            {
                NpcGlobalList[i].gameObject.transform.SetPositionAndRotation(NpcGlobalList[i].GetComponent<Npc>().GetStartPosition().position, 
                    NpcGlobalList[i].GetComponent<Npc>().GetStartPosition().rotation);
                NpcGlobalList[i].SetActive(true);
            }
        }
    }

    private void DesactiveAllNpc()
    {
        for (int i = 0; i < NpcGlobalList.Count; i++)
        {
            NpcGlobalList[i].SetActive(false);
        }
    }

    public void TestIfPassToNextDay()
    {
        bool goNextDay = true;
        for (int i = 0; i < NpcGlobalList.Count && goNextDay; i++)
        {
            if (NpcGlobalList[i].activeInHierarchy)
            {
                goNextDay = false;
            }    
        }
        if(goNextDay)
            NextDay();
    }
}

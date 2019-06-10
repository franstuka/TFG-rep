using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControl : MonoBehaviour
{
    #region singleton
    public static TestControl instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public List<GameObject> Prefabs;
    public List<GameObject> UpdatedNpc;

    private void Update()
    {
        if(Input.GetKeyDown("1"))
        {
            Active(false);
        }
        if (Input.GetKeyDown("2"))
        {
            Active(true);  
        }
        if (Input.GetKeyDown("3"))
        {
            Load();
        }
        if (Input.GetKeyDown("4"))
        {
            MakeChange();
        }
    }

    private void Start()
    {
        OnStart();
    }

    private void Active(bool state)
    {
        UpdatedNpc[0].SetActive(state);
    }

    private void Load()
    {
        Instantiate(UpdatedNpc[0]);
    }

    private void Delete()
    {

    }

    private void MakeChange()
    {
        UpdatedNpc[0].GetComponent<TestItem>().NextDay();
    }

    private void OnStart()
    {
        UpdatedNpc.Add(Instantiate(Prefabs[0]));
        if (!UpdatedNpc[0].GetComponent<TestItem>())
        {
            Debug.LogError("component not found");
        }
        else
            UpdatedNpc[0].GetComponent<TestItem>().DayOne();
    }
}

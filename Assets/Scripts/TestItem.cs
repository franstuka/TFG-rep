using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : MonoBehaviour
{
    public enum TestName { ONE, TWO};
    public TestName Name;

    public Inventory inventory;
    public float money = 0;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    public void DayOne()
    {
        inventory.DayOne();
    }

    public void NextDay()
    {
        inventory.UpdateInventory();
        inventory.Test();
        money += 50f;
    }
}

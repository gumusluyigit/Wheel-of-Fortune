using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> itemDefinitions;
    static public GameManager instance;

    public int [] inventory;

    public int playerMoney = 100;
    public int zoneIndex = 1;

    public GameObject BronzeWheel;
    public GameObject SilverWheel;
    public GameObject GoldenWheel;


    public TextMeshProUGUI playerMoneyText;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        inventory = new int[(int)ItemType.Max - 1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToInventory(Item item)
    {
        inventory[(int)item.type] += item.amount;
    }

    public void ClearInventory()
    {
        for(int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = 0;
        };
    }
}

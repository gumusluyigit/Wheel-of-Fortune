using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum ItemType
{
    Gold,
    Cash,
    Box,
    Bomb,
    Max
};
public class Item : MonoBehaviour
{
    

    [SerializeField] public ItemType type;
    [SerializeField] public int minAmount;
    [SerializeField] public int maxAmount;

    [DoNotSerialize] public int amount;

    private void Start()
    {
        amount = Random.Range(minAmount, maxAmount);
    }
}

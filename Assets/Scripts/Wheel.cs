using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Wheel : MonoBehaviour
{
    public float rotationSpeed = 0f;
    public float lerpSpeed = 0.5f;

    public List<GameObject> items;
    public GameObject rewardPanel;
    public GameObject bombPanel;

    public Button rewardClaimButton;
    public Button bombReviveButton;
    public Button bombGiveUpButton;

    public TextMeshProUGUI rewardsText;
    public TextMeshProUGUI zoneReward;
    public TextMeshProUGUI levelText;


    public Transform rewardPanelArea;

    private float rewardAngle;
    private float targetAngle;
    private double sliceIndex;

    private int numSlices = 8;

    

    enum WheelState
    {
        Stationary,
        InitialSpin,
        Correction
    }

    WheelState State;
    private float correctionRotationSpeed = 10f;
    private float finalAngle;

    private void Start()
    {
        PopulateItems();

        rewardClaimButton.onClick.AddListener(DeactivateRewardPanel);
        bombReviveButton.onClick.AddListener(DeactivateBombPanel);    
        
    }

    void PopulateItems()
    {
        levelText.text = "Level: " + GameManager.instance.zoneIndex.ToString();
        string money = GameManager.instance.playerMoney.ToString();
        GameManager.instance.playerMoneyText.text = "Your credit: " + money;
        Vector3 direction = new Vector3(0, 1, 0);
        float distance = 1.9f;

        // clear items
        foreach (GameObject item in items)
        {
            Destroy(item);
        }

        items.Clear();

        // reset wheel rotation
        transform.rotation = Quaternion.identity;

        // generate new set of items
        int bombIndex = UnityEngine.Random.Range(0, numSlices);
        for (int i = 0; i < numSlices; i++)
        {
            GameObject item;

            // get an item, either bomb or regular reward
            if (i == bombIndex)
            {
                item = GameManager.instance.itemDefinitions.Find(obj => obj.GetComponent<Item>().type.ToString() == "Bomb");
            }
            else
            {
                // Choose a random item excluding the bomb
                List<GameObject> availableItems = new List<GameObject>(GameManager.instance.itemDefinitions);
                availableItems.RemoveAll(obj => obj.GetComponent<Item>().type.ToString() == "Bomb");
                item = availableItems[UnityEngine.Random.Range(0, availableItems.Count)];
            }

            // rotate item
            Vector3 rotatedVector = Quaternion.Euler(0, 0, -360 / numSlices * i) * direction;
            Vector3 spawnPosition = transform.position + rotatedVector * distance;
            spawnPosition.z += -1;

            // Create item
            GameObject spawnedObject = Instantiate(item, spawnPosition, Quaternion.identity);

            // set its parent to wheel
            spawnedObject.transform.SetParent(transform);

            items.Add(spawnedObject);
        }
    }

    void Update()
    {
        if (State == WheelState.Stationary)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                State = WheelState.InitialSpin;
                rotationSpeed = 500f; // Initial speed

            }
        }

        if (State == WheelState.InitialSpin)
        {

            rotationSpeed = Mathf.Lerp(rotationSpeed, 5f, Time.deltaTime * lerpSpeed);
            transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);

            // Stop spinning if rotation speed is very slow
            if (rotationSpeed < 10f)
            {
                rotationSpeed = 0f;

                rewardAngle = (float)(transform.rotation.eulerAngles.z - 22.5);
                if (rewardAngle % 45 > 22.5)
                {
                    sliceIndex = (rewardAngle / 45) + 3;
                    targetAngle = rewardAngle + 45 + (45 - rewardAngle % 45);
                }
                else
                {
                    sliceIndex = (rewardAngle / 45) + 2;
                    targetAngle = rewardAngle + (45 - rewardAngle % 45);
                }

                State = WheelState.Correction;
            }
        }
        if (State == WheelState.Correction)
        {

            transform.Rotate(0, 0, Time.deltaTime * correctionRotationSpeed);
            finalAngle = transform.rotation.eulerAngles.z;

            if (Mathf.Abs(finalAngle - targetAngle) < 1)

            {                
                State = WheelState.Stationary;
                rotationSpeed = 0f;
                GiveReward();

            }

        }

        // rotate items to always face up
        if (State != WheelState.Stationary)
        {
            foreach (GameObject item in items)
            {
                item.transform.rotation = Quaternion.identity;
            }
        }
    }

    private void GiveReward()
    {
        rewardAngle = (float)(transform.rotation.eulerAngles.z - 22.5);
        sliceIndex = (rewardAngle / 45) + 2 + (rewardAngle % 45 > 22.5 ? 1 : 0);


        int sliceIndexInteger = (int)(sliceIndex - 1);
        if (sliceIndexInteger < 0) sliceIndexInteger += 8;
        sliceIndexInteger = sliceIndexInteger % 8;

        GameObject selectedItemPrefab = items[sliceIndexInteger];

        string itemType = selectedItemPrefab.GetComponent<Item>().type.ToString();


        if (itemType == "Bomb")
        {
            // Activate bomb panel and deactivate reward panel
            bombPanel.SetActive(true);
            rewardPanel.SetActive(false);

            Debug.Log("Your rewards: ");
            rewardsText.text = "";
            for (int i = 0; i < GameManager.instance.inventory.Length; i++)
            {
                ItemType type = (ItemType)i;

                Debug.Log(type.ToString() + " " + GameManager.instance.inventory[i]);
                string s = type.ToString() + " " + GameManager.instance.inventory[i];

                rewardsText.text += s + "\n";
            }
        }
        else
        {
            // Activate reward panel and deactivate bomb panel
            bombPanel.SetActive(false);
            rewardPanel.SetActive(true);
            GameManager.instance.AddToInventory(selectedItemPrefab.GetComponent<Item>());
            string s = items[sliceIndexInteger].GetComponent<Item>().type.ToString();
            int x = items[sliceIndexInteger].GetComponent<Item>().amount;
            zoneReward.text = "You've won: " + x + "x" + s;
        }
        Debug.Log("Your reward is: " + items[sliceIndexInteger].GetComponent<Item>().type);

    }
    public void DeactivateRewardPanel()
    {
        rewardPanel.SetActive(false);
        GameManager.instance.zoneIndex++;
        PopulateItems();
    }

    public void DeactivateBombPanel()
    {
        bombPanel.SetActive(false);
        GameManager.instance.zoneIndex++;
        PopulateItems();

        if (GameManager.instance.playerMoney >= 50)
        {
            GameManager.instance.playerMoney -= 50;
            GameManager.instance.playerMoneyText.text = "Your credit: " + GameManager.instance.playerMoney.ToString();
        }
        else if(GameManager.instance.playerMoney == 0)
        {
            bombReviveButton.interactable = false;
            GameManager.instance.ClearInventory();
        }
    }

}

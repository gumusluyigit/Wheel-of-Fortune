using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    private int zoneIndex;

    private void Update()
    {
        int prevZoneIndex = zoneIndex;
        zoneIndex = GameManager.instance.zoneIndex;

        if (prevZoneIndex != zoneIndex)
        {
            DisplayWheel(zoneIndex);
        }
    }
    private void DisplayWheel(int zoneIndex)
    {
        if(zoneIndex % 30 == 0)
        {
            DisplayGoldenWheel();
        }
        else if(zoneIndex % 5 == 0)
        {
            DisplaySilverWheel();
        }
        else
        {
            DisplayBroneWheel();
        }
    }
    private void DisplayBroneWheel()
    {
        GameManager.instance.BronzeWheel.SetActive(true);

        GameManager.instance.SilverWheel.SetActive(false);
        GameManager.instance.GoldenWheel.SetActive(false);
    }

    private void DisplaySilverWheel()
    {
        GameManager.instance.SilverWheel.SetActive(true);

        GameManager.instance.BronzeWheel.SetActive(false);
        GameManager.instance.GoldenWheel.SetActive(false);
    }
    private void DisplayGoldenWheel()
    {
        GameManager.instance.GoldenWheel.SetActive(true);

        GameManager.instance.SilverWheel.SetActive(false);
        GameManager.instance.BronzeWheel.SetActive(false);
    }
}

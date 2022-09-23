using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdDying : MonoBehaviour
{
    // Place this code on the actual bird. Just maybe not the dying bit.
    public int maxEnergy = 10;
    public int currentEnergy;

    //reference to energy script
    public BirdEnergy energyBar;


    void Start()
    {
        currentEnergy = maxEnergy;
        energyBar.SetMaxEnergy(maxEnergy);
    }

    void Update() //Testing: Press spacebar to run energy out.
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EnergySpent(2);
        }
    }

    void EnergySpent(int tiring)
    {
        currentEnergy -= tiring;

        energyBar.SetEnergy(currentEnergy);
    }
}

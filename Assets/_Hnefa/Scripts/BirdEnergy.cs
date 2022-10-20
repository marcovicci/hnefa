using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdEnergy : MonoBehaviour
{   
    //nice green sliding energy bar for the bird
    public Slider slider;

    public void SetMaxEnergy(int energy)
    {
        slider.maxValue = energy;
        slider.value = energy;
    }


    public void SetEnergy(int energy)
    {
        slider.value = energy;
    }
}
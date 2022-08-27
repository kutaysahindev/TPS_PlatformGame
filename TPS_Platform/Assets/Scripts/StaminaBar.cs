using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider staminaBar;

    private float maxValue;

    public void SetMaxValue(float maxValue)
    {
        this.maxValue = maxValue;
        staminaBar.value = maxValue;
        staminaBar.maxValue = maxValue;
    }

    public void SetBarVisual(float value)
    {
        value = Mathf.Clamp(value, 0, maxValue);
        staminaBar.value = value;
    }


   

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public PlayerController playerController;
    public Image fillImage;
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        if(slider.value <= slider.minValue) 
        {
            fillImage.enabled = false;
        }
        if(slider.value > slider.minValue && !fillImage.enabled) 
        {
            fillImage.enabled = true;
        }

        float fillValue = playerController.playerCurrentHealth / playerController.playerMaxHealth;
        slider.value = fillValue;
    }
}

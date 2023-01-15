using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StatSlider : MonoBehaviour
{
    public Slider slider;
    public int statNumber;
    public PlayerStats myPlayer;
    public Image image;
    public TextMeshProUGUI statNumberDisplay;
    public Gradient gradient;
    public UIManager uiManager;
    public void OnCreation(PlayerStats player, UIManager manager)
    {
        uiManager = manager;
        statNumber = uiManager.GetStat(transform.GetSiblingIndex());
        float proportion = (float)statNumber / 100f;
        slider.SetValueWithoutNotify(statNumber);
        image.color = gradient.Evaluate(proportion);
        statNumberDisplay.text = "" + statNumber;
    }
    public void OnSliderUpdate()
    {
        statNumber = (int)slider.value;
        image.color = gradient.Evaluate((float)statNumber / 100f);
        statNumberDisplay.text = "" + statNumber;
        uiManager.SetStat(statNumber, transform.GetSiblingIndex());
        int newStat = (int)slider.value;
        
    }


}

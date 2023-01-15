using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public Transform playerDisplayLocation;
    public GameObject playerDisplayDefault;
    public GameObject playerDisplayClick;
    public List<StatSlider> sliders;
    public PlayerStats selectedPlayer;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerScore;

    public void DisplayPlayerStats(PlayerStats player)
    {
        selectedPlayer = player;
        playerDisplayDefault.SetActive(false);
        playerDisplayClick.SetActive(true);
        foreach (StatSlider slider in sliders)
        {
            slider.OnCreation(player, this);
        }
        playerName.text = player.name;
        playerScore.text = player.getScore().ToString();
    }
    public void PressRed()
    {
        gameManager.AddPlayerRed(selectedPlayer);
    }
    public void PressBlue()
    {
        gameManager.AddPlayerBlue(selectedPlayer);
    }
    public int GetStat(int variable)
    {
        switch (variable)
        {
            case 0:
                return selectedPlayer.farming;
                break;
            case 1:
                return selectedPlayer.warding;
                break;
            case 2:
                return selectedPlayer.trading;
                break;
            case 3:
                return selectedPlayer.allIn;
                break;
            case 4:
                return selectedPlayer.assassinMastery;
                break;
            case 5:
                return selectedPlayer.bruiserMastery;
                break;
            case 6:
                return selectedPlayer.marksmanMastery;
                break;
            case 7:
                return selectedPlayer.supportMastery;
                break;
            case 8:
                return selectedPlayer.mageMastery;
                break;
            case 9:
                return selectedPlayer.tankMastery;
                break;
            case 10:
                return selectedPlayer.leadership;
                break;
            case 11:
                return selectedPlayer.teamwork;
                break;
            case 12:
                return selectedPlayer.playmaking;
                break;
            case 13:
                return selectedPlayer.vision;
                break;
            case 14:
                return selectedPlayer.communication;
                break;
            case 15:
                return selectedPlayer.positioning;
                break;
            case 16:
                return selectedPlayer.composure;
                break;
            case 17:
                return selectedPlayer.concentration;
                break;
            case 18:
                return selectedPlayer.aggression;
                break;
            case 19:
                return selectedPlayer.flair;
                break;
            case 20:
                return selectedPlayer.decisionMaking;
                break;
            case 21:
                return selectedPlayer.determination;
                break;
            case 22:
                return selectedPlayer.consistency;
                break;
            default:
                return 0;
                break;
        }
    }
    public void SetStat(int stat, int variable)
    {
        switch (variable) 
        {
            case 0:
                selectedPlayer.farming = stat;
                break;
            case 1:
                selectedPlayer.warding = stat;
                break;
            case 2:
                selectedPlayer.trading = stat;
                break;
            case 3:
                selectedPlayer.allIn = stat;
                break;
            case 4:
                selectedPlayer.assassinMastery = stat;
                break;
            case 5:
                selectedPlayer.bruiserMastery = stat;
                break;
            case 6:
                selectedPlayer.marksmanMastery = stat;
                break;
            case 7:
                selectedPlayer.supportMastery = stat;
                break;
            case 8:
                selectedPlayer.mageMastery = stat;
                break;
            case 9:
                selectedPlayer.tankMastery = stat;
                break;
            case 10:
                selectedPlayer.leadership = stat;
                break;
            case 11:
                selectedPlayer.teamwork = stat;
                break;
            case 12:
                selectedPlayer.playmaking = stat;
                break;
            case 13:
                selectedPlayer.vision = stat;
                break;
            case 14:
                selectedPlayer.communication = stat;
                break;
            case 15:
                selectedPlayer.positioning = stat;
                break;
            case 16:
                selectedPlayer.composure = stat;
                break;
            case 17:
                selectedPlayer.concentration = stat;
                break;
            case 18:
                selectedPlayer.aggression = stat;
                break;
            case 19:
                selectedPlayer.flair = stat;
                break;
            case 20:
                selectedPlayer.decisionMaking = stat;
                break;
            case 21:
                selectedPlayer.determination = stat;
                break;
            case 22:
                selectedPlayer.consistency = stat;
                break;
        }
        playerScore.text = selectedPlayer.getScore().ToString();
    }
}

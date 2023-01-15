using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampDisplay : MonoBehaviour
{
    public ChampionStats stats;
    public Image championIcon;
    ChampionStatDisplay championStatDisplay;
    public void OnCreation(ChampionStats myStats, ChampionStatDisplay statDisplay)
    {
        stats = myStats;
        championIcon.sprite = stats.championIcon;
        championStatDisplay = statDisplay;
        if (stats.stats.Count == 0)
        {
            stats.CreateList();
        }
    }
    public void OnClick()
    {
        championStatDisplay.DisplayStats(transform.GetSiblingIndex());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    //TODO: Make an enum and put these values into a list that the enum references indexes of?
    public string name;
    //statblocks
    public int farming; // 1.5
    public int warding; //1.1
    public int trading; // 1
    public int allIn; // 1
    public int assassinMastery; // 0.5
    public int bruiserMastery; // 0.5
    public int marksmanMastery; // 0.5
    public int supportMastery; // 0.5
    public int mageMastery; // 0.5
    public int tankMastery; //0.5
    public int leadership; // 0
    public int teamwork; // 1.5
    public int playmaking; // 1
    public int vision; // 1
    public int communication; // 1.1
    public int positioning; // 1
    public int composure; // 1.5
    public int concentration; // 1
    public int aggression; // 0
    public int flair; // 0.5
    public int decisionMaking; // 1.75
    public int determination; // 1
    public int consistency; // 1.1
    public int championSelected;
    public float weightSums = 20.05f;

    public int getScore()
    {
        float score = (assassinMastery + bruiserMastery + marksmanMastery + mageMastery + tankMastery + flair);
        score *= 0.5f;
        score += trading + allIn + playmaking + vision + positioning + concentration + determination;
        score += (farming + composure + teamwork) * 1.5f;
        score += (communication + consistency + warding) * 1.1f;
        score += decisionMaking * 1.75f;

        score /= weightSums;

        int finalScore = Mathf.CeilToInt(score);
        return finalScore; 
    }
}

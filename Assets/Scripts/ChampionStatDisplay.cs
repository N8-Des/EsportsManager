using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChampionStatDisplay : MonoBehaviour
{
    public TextMeshProUGUI championName;
    public Image championIcon;
    public ChampionStatSlider[] statSliders;
    List<float> startingStats = new List<float>();
    public Gradient statColors;
    public GameManager gameManager;
    int championOn;
    public UIManager uiManager;
    public void DisplayStats(int champIndex)
    {
        ChampionStats champ = gameManager.champions[champIndex];
        championOn = champIndex;
        championIcon.sprite = champ.championIcon;
        championName.text = champ.championName;
        startingStats.Clear();
        for (int i = 0; i < statSliders.Length; i++)
        {
            startingStats.Add(statSliders[i].statBar.fillAmount * 10);
        }
        StopCoroutine(LerpDisplay(champ));
        StartCoroutine(LerpDisplay(champ));
    }
    public void OnConfirm()
    {
        uiManager.StopDisplayingChamps(championOn);
    }
    IEnumerator LerpDisplay(ChampionStats stats)
    {
        float duration = 0;
        while (duration <1f)
        {
            yield return null;
            duration += Time.deltaTime / 1f;
            for (int i = 0; i < statSliders.Length; i++)
            {
                float currentStat = Mathf.Lerp(startingStats[i], stats.stats[i], duration);
                statSliders[i].statValue.text = currentStat.ToString("0.0");
                statSliders[i].statBar.fillAmount = currentStat / 10f;
                statSliders[i].statBar.color = statColors.Evaluate(currentStat / 10f);
            }
        }
        duration = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class GameCanvas : MonoBehaviour
{
    public Animator anim;
    [Header("1v1 Battle Display")]
    public GameObject display1v1Object;
    public TextMeshProUGUI bluePlayerName;
    public TextMeshProUGUI redPlayerName;
    public TextMeshProUGUI bluePlayerScore;
    public TextMeshProUGUI redPlayerScore;
    public TextMeshProUGUI bluePlayerScoreTitle;
    public TextMeshProUGUI redPlayerScoreTitle;
    public TextMeshProUGUI escapeMessage;
    public RectTransform initiatorText;
    int combatDisplayStage = 0;

    public void Display1v1(string bluePlayer, string redPlayer, float blueScore, float redScore, Vector3 initPosition)
    {
        escapeMessage.transform.parent.gameObject.SetActive(false);
        display1v1Object.SetActive(true);
        initiatorText.gameObject.SetActive(true);
        bluePlayerScoreTitle.text = "Combat Score:";
        redPlayerScoreTitle.text = "Combat Score:";
        bluePlayerName.text = bluePlayer;
        redPlayerName.text = redPlayer;
        bluePlayerScore.text = blueScore.ToString("0.00");
        redPlayerScore.text = redScore.ToString("0.00");
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(initPosition) + new Vector3(0, 55, 0);
        initiatorText.position = screenPosition;
        anim.SetTrigger("StartIntro1v1");
    }

    public void DisplayEscape(string runningPlayer, string chasingPlayer, float blueScore, float redScore, bool bluePlayerChasing)
    {
        initiatorText.gameObject.SetActive(false);

        escapeMessage.transform.parent.gameObject.SetActive(true);

        combatDisplayStage = 1;
        escapeMessage.text = runningPlayer + " attempts to disengage...";
        if (bluePlayerChasing)
        {
            bluePlayerScoreTitle.text = "Pursuit Score:";
            redPlayerScoreTitle.text = "Disengage Score:";
            bluePlayerScore.text = blueScore.ToString("0.00");
            redPlayerScore.text = redScore.ToString("0.00");
        }
        else
        {
            bluePlayerScoreTitle.text = "Disengage Score:";
            redPlayerScoreTitle.text = "Pursuit Score:";
            bluePlayerScore.text = redScore.ToString("0.00");
            redPlayerScore.text = blueScore.ToString("0.00");
        }
    }

    public void DisplayResult(string result)
    {

        combatDisplayStage = 2;
        escapeMessage.transform.parent.gameObject.SetActive(true);
        escapeMessage.text = result;
    }

    public void Continue()
    {
        Time.timeScale = 1;
        if (combatDisplayStage == 0)
        {
            initiatorText.gameObject.SetActive(false);
        }
        else if(combatDisplayStage == 1)
        {
            escapeMessage.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            display1v1Object.SetActive(false);
            combatDisplayStage = 0;
        }
    }
}

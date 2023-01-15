using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDisplay : MonoBehaviour
{
    public PlayerStats myPlayer;
    public UIManager uiManager;
    public TextMeshProUGUI playerName;

    public void SetPlayer(PlayerStats p, UIManager manager)
    {
        myPlayer = p;
        playerName.text = p.name;
        uiManager = manager;
    }
    public void RemovePlayer()
    {
        myPlayer = null;
        playerName.text = "";
    }
    public void OnClick()
    {
        uiManager.DisplayPlayerStats(myPlayer);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class ReadPlayerData : MonoBehaviour
{
    public static void ReadAndLoadPlayers(GameManager gameManager)
    {
        string jsonString = File.ReadAllText(Application.streamingAssetsPath + "/players.json");

        gameManager.players = JsonHelper.getJsonArray<PlayerStats>(jsonString);
    }

    public static void SavePlayers(GameManager gameManager)
    {
        File.WriteAllText(Application.streamingAssetsPath + "/players.json", string.Empty);
        string finalString = "[";
        for(int i = 0; i < gameManager.players.Length; i++)
        {
            string jsonString = JsonUtility.ToJson(gameManager.players[i]);
            if (i < gameManager.players.Length - 1)
            {
                finalString += jsonString + ",";
            }
            else
            {
                finalString += jsonString;
            }
        }
        finalString += "]";
        File.WriteAllText(Application.streamingAssetsPath + "/players.json", finalString);

    }
}

public class JsonHelper
{
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}


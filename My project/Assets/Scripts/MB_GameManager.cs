using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct GameStatus
{
    public string playerName;
    public int currentLevel;
    public int ringsCollected;
    public List <Vector3> npcPositions;
    public Vector3 playerPosition;
    public int npcCount;
}

public class MB_GameManager : MonoBehaviour
{
    public GameStatus gameStatus;
    string filePath;
    const string FILE_NAME = "SaveStatus.json";


public void Start()
 { 
    filePath = Application.persistentDataPath; 
    gameStatus = new GameStatus();
    gameStatus.npcPositions = new List<Vector3>();
    Debug.Log(filePath); 
    LoadGameStatus();
 }

public void LoadGameStatus()
    {
        if (File.Exists(filePath + "/" + FILE_NAME))
        {
            string loadedJson = File.ReadAllText(filePath + "/" + FILE_NAME);
            gameStatus = JsonUtility.FromJson<GameStatus>(loadedJson);
            Debug.Log("File Loaded Successfully");
            
        }
        
    }

public void ResetGame()
    {
        gameStatus.playerName = "Frankie";
        gameStatus.currentLevel = 1;
        gameStatus.playerPosition = new Vector3(0,0,0);
        gameStatus.ringsCollected = 0;
        gameStatus.npcPositions = new List<Vector3>();

        SaveGameStatus();
    }

public void SaveGameStatus()
    {
        UpdateNPCs();
        string gameStatusJson = JsonUtility.ToJson(gameStatus);
        File.WriteAllText(filePath + "/" + FILE_NAME, gameStatusJson);
        Debug.Log("File Created and Saved");
    }

public string UpdateStatus()
    {
        string message = "";
        message += "Player Name: " + gameStatus.playerName + "\n";
        message += "Current Level: " + gameStatus.currentLevel + "\n";
        message += "Rings: " + gameStatus.ringsCollected + "\n";
        message += "NPC Count: " + gameStatus.npcCount + "\n";
        message += "NPC Positions: " + gameStatus.npcPositions + "\n";
        message += "Player Posiiton: " + gameStatus.playerPosition + "\n";
        return message;
    }

public void OnApplicationQuit()
    {
        SaveGameStatus();
        Debug.Log("Game Data Saved");
    }

public void UpdateNPCs()
    {
        if (gameStatus.npcPositions == null)
        gameStatus.npcPositions = new List<Vector3>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        gameStatus.npcPositions.Clear();
        foreach (GameObject enemy in enemies)
        {
            gameStatus.npcPositions.Add(enemy.transform.position);
        }
        gameStatus.npcCount = enemies.Length;
    }

}
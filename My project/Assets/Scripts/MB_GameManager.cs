using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.Netcode;

[Serializable]
public struct GameStatus
{
    public string playerName;
    public int currentLevel;
    public int ringsCollected;
    public List<Vector3> npcPositions;
    public Vector3 playerPosition;
    public int npcCount;
}

public class MB_GameManager : NetworkBehaviour 
{
    public GameStatus gameStatus;
    string filePath;
    const string FILE_NAME = "SaveStatus.json";

    public override void OnNetworkSpawn()
    { 
        filePath = Application.persistentDataPath;
        
        if (!IsServer) return; 

        if (gameStatus.npcPositions == null)
            gameStatus.npcPositions = new List<Vector3>();

        Debug.Log($"Save path: {Path.Combine(filePath, FILE_NAME)}"); 
        LoadGameStatus();
    }

    [ContextMenu("Force Save Now")]
    public void SaveGameStatus(bool bypassServerCheck = false)
    {
       
        if (!bypassServerCheck && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            if (!IsServer) 
            {
                Debug.LogWarning("Save aborted: Only the Server/Host can save game data.");
                return;
            }
        }

     
        UpdateNPCs();

        
        if (string.IsNullOrEmpty(filePath)) filePath = Application.persistentDataPath;
        string fullPath = Path.Combine(filePath, FILE_NAME);

       
        try {
            string gameStatusJson = JsonUtility.ToJson(gameStatus, true);
            File.WriteAllText(fullPath, gameStatusJson);
            Debug.Log("<b>SUCCESS:</b> Data saved to: " + fullPath);
        }
        catch (Exception e) {
            Debug.LogError("Save Failed: " + e.Message);
        }
    }

    public void UpdateNPCs()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) 
        {
            gameStatus.playerPosition = player.transform.position;
        }

        // Capture NPC Positions
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

    public void LoadGameStatus()
    {
        if (!IsServer) return;

        string fullPath = Path.Combine(filePath, FILE_NAME);
        if (File.Exists(fullPath))
        {
            string loadedJson = File.ReadAllText(fullPath);
            gameStatus = JsonUtility.FromJson<GameStatus>(loadedJson);
            Debug.Log("File Loaded Successfully");
        }
    }

    public void OnApplicationQuit()
    {
        if (IsServer || IsHost)
        {
            Debug.Log("Application quitting... forcing final save.");
            SaveGameStatus(true); 
        }
    }
}
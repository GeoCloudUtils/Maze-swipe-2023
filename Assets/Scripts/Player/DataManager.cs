using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private GameData savedData;

    private void Start()
    {
        Save(new Settings { sfx = true, sound = false }, level: 1, score: 1000);
        savedData = GetSavedData();
    }

    public int GetSavedLevel()
    {
        return GetSavedData().level;
    }

    public int GetSavedScore()
    {
        return GetSavedData().score;
    }

    public Settings GetSettings()
    {
        return GetSavedData().settings;
    }

    public void Save(Settings settings, int level, int score)
    {
        string filePath = $"{Application.persistentDataPath}/{SystemInfo.deviceUniqueIdentifier}";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        filePath += $"/Data";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        filePath += $"/4o0v-02-eeportpot113.dat";
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }
        GameData data = new GameData
        {
            settings = settings,
            score = score,
            level = level
        };
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        string key = DataEncryptoDecryptor.LoadSecretKey();
        DataEncryptoDecryptor.SaveEncryptedFile(jsonData, filePath, key);
    }

    private GameData GetSavedData()
    {
        string filePath = string.Format("{0}/{1}/Data/4o0v-02-eeportpot113.dat", Application.persistentDataPath, SystemInfo.deviceUniqueIdentifier);
        string key = DataEncryptoDecryptor.LoadSecretKey();
        GameData data = DataEncryptoDecryptor.LoadEncryptedFile<GameData>(filePath, key);
        if (data == null)
        {
            return savedData;
        }
        return data;
    }
}

[System.Serializable]
public class Settings
{
    public bool sfx;
    public bool sound;
}

[System.Serializable]
public class GameData
{
    public Settings settings;
    public int score;
    public int level;
}

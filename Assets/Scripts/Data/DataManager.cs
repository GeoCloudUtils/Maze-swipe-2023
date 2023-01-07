using System.IO;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public SavedGameData currentData;
    public ColorScheme ColorScheme => currentData.colorScheme;
    public Settings Settings => currentData.settings;
    public int Level => currentData.level;
    public int Score => currentData.score;

    private void Start()
    {
        Save(ColorScheme.LIGHT, new Settings { sfx = true, sound = true }, 1, 100);
        currentData = LoadData();
    }

    /// <summary>
    /// Get current level
    /// </summary>
    /// <returns></returns>
    public int GetLevel()
    {
        return currentData.level;
    }

    /// <summary>
    /// Get current score
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return currentData.score;
    }

    /// <summary>
    /// Get current settings
    /// </summary>
    /// <returns></returns>
    private Settings GetSettings()
    {
        return new Settings
        {
            sfx = currentData.settings.sfx,
            sound = currentData.settings.sound
        };
    }

    /// <summary>
    /// Encrypt and save new settings to file
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="level"></param>
    /// <param name="score"></param>
    public void Save(ColorScheme colorScheme = ColorScheme.LIGHT, Settings settings = null, int level = -1, int score = -1, int diamonds = -1)
    {
        string filePath = $"{Application.persistentDataPath}/{SystemInfo.deviceUniqueIdentifier}";

        if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }

        filePath += $"/Data";

        if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }

        filePath += $"/4o0v-02-eeportpot113.dat";

        if (!File.Exists(filePath)) { File.Create(filePath).Close(); }

        /// if settings is null, load last saved settings
        if (settings == null) { settings = GetSettings(); }

        /// if level is not defined or is less than 0, load last saved level
        if (level < 0) { level = currentData.level; }

        /// if score is not defined or is less than 0, load last saved score
        if (score < 0) { score = currentData.score; }

        if (diamonds < 0) { diamonds = currentData.diamonds; }

        Settings newSettings = new Settings
        {
            sfx = settings.sfx,
            sound = settings.sound
        };
        SavedGameData data = new SavedGameData
        {
            settings = newSettings,
            score = score,
            level = level,
            colorScheme = colorScheme,
            diamonds = diamonds
        };
        string key = DataEncryptoDecryptor.LoadSecretKey();
        DataEncryptoDecryptor.SaveEncryptedFile(data, filePath, key);
    }

    private new void OnApplicationQuit()
    {
        Save(currentData.colorScheme, currentData.settings, currentData.level, currentData.score);
    }

    /// <summary>
    /// Load data from encrypted file
    /// </summary>
    /// <returns></returns>
    private SavedGameData LoadData()
    {
        string filePath = string.Format("{0}/{1}/Data/4o0v-02-eeportpot113.dat", Application.persistentDataPath, SystemInfo.deviceUniqueIdentifier);
        string key = DataEncryptoDecryptor.LoadSecretKey();
        SavedGameData data = DataEncryptoDecryptor.LoadEncryptedFile<SavedGameData>(filePath, key);
        if (data == null)
        {
            return currentData;
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
public class SavedGameData
{
    public Settings settings = new Settings();

    public int score;
    public int level;
    public int diamonds;

    public ColorScheme colorScheme;
}
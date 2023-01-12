using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class LevelLoader
{
    public static Data Load(string levelName, int level)
    {
        string filePath = Application.dataPath + "/SavedLevels/";

        if (!Directory.Exists(filePath))
        {
            Debug.LogError("Directory not exists! Path: " + filePath);
            return null;
        }

        filePath += $"{levelName}.json";

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not exists! Path: " + filePath);
            return null;
        }

        string json = File.ReadAllText(filePath);
        LevelData data = JsonConvert.DeserializeObject<LevelData>(json);

        Data result = new Data()
        {
            level = level.ToString(),
            activableCellsCount = data.activableCellsCount,
            maxMoves = data.path.Count,
            gridLength = data.gridSize,
            levelData = data.levelData
        };

        return result;
    }
}

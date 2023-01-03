using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelBuilder", menuName = "ScriptableObjects/LevelBuilder", order = 1)]
public class LevelsDefinition : ScriptableObject
{
    [SerializeField] private LevelsDefinition levelData;
    public List<Data> AllLevels = new List<Data>();

    public bool LevelExist(string data)
    {
        return AllLevels.Exists(match: e => e.levelData == data);
    }
    public void AddLevel(int gridLength, int activableCells)
    {
        rows = gridLength;
        cols = gridLength;
        ResetGrid();
        Data newLevelData = new Data
        {
            levelData = data,
            gridLength = gridLength,
            activableCellsCount = activableCells
        };
        AllLevels.Add(newLevelData);
        OnValidate();
        MaxValue = 5;
    }

    public int GetLevel(string levelIndex)
    {
        if (AllLevels.Count < int.Parse(levelIndex))
        {
            Debug.LogWarning("No more levels. Generating last level!");
            return int.Parse(AllLevels[AllLevels.Count - 1].levelData);
        }
        return int.Parse(AllLevels[int.Parse(levelIndex)].levelData);
    }


    private void OnValidate()
    {
        for (int i = 0; i < AllLevels.Count; i++)
        {
            Data level = AllLevels[i];
            level.level = $"Level_{i + 1} ({level.gridLength}x{level.gridLength})";
        }
    }

    [SerializeField] private string data;

    [SerializeField] private int rows = 3;
    [SerializeField] private int cols = 3;

    [SerializeField] private int MaxValue = 5;

    public int ActivableCellsCount = 0;

    public string Data { get => data; private set => data = value; }
    public int Rows { get => rows; private set => rows = value; }
    public int Cols { get => cols; private set => cols = value; }
    public LevelsDefinition LevelData { get => levelData; private set => levelData = value; }

    public string GetCell(int x, int y)
    {
        int n = GetIndex(x, y);
        return Data.Substring(n, 1);
    }

#if UNITY_EDITOR
    public void ResetGrid()
    {
        data = "0";
        if (Data == null || Data.Length != (Rows * Cols))
        {
            Undo.RecordObject(this, "Resize");
            if (Rows < 1) { Rows = 1; }
            if (Cols < 1) { Cols = 1; }
            data = "";
            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    string cell = "0";
                    data += cell;
                }
            }
            EditorUtility.SetDirty(this);
        }
    }

    void Reset()
    {
        OnValidate();
    }
#endif

    public int GetIndex(int x, int y)
    {
        if (x < 0) return -1;
        if (y < 0) return -1;
        if (x >= Rows) return -1;
        if (y >= Cols) return -1;
        return x + y * Rows;
    }

    public void ToggleCell(int x, int y)
    {
        Debug.Log(MaxValue);
        int n = GetIndex(x, y);
        if (n >= 0)
        {
            var cell = Data.Substring(n, 1);
            int.TryParse(cell, out int c);
            c++;
            if (c >= MaxValue) { c = 0; }
            cell = c.ToString();
            Undo.RecordObject(this, "Toggle Cell");
            Data = Data.Substring(0, n) + cell + Data.Substring(n + 1);
            EditorUtility.SetDirty(this);
        }
    }
}

[System.Serializable]
public class Data
{
    public string level;

    public string levelData;

    public int gridLength;

    public int maxMoves;

    public int activableCellsCount;
}


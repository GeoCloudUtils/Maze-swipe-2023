#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// OLD
/// </summary>
public class MazeGridBuilder : MonoBehaviour
{
    [SerializeField] private LevelsDefinition levelData;

    [Header("Actual saved payload. Use GetCell(x,y) to read!")]
    [Header("WARNING: changing this will nuke your data!")]

    [SerializeField] private string data;

    [SerializeField] private int rows = 6;
    [SerializeField] private int cols = 4;

    [SerializeField] private int MaxValue = 4; // 0 - inactive cell, 1 - active cell, 2 - start cell, 3 - end cell
    public int CellSize = 200;
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
    void OnValidate()
    {
       // ResetGrid();
    }

    public void ResetGrid()
    {
        Data = "0";
        if (Data == null || Data.Length != (Rows * Cols))
        {
            Undo.RecordObject(this, "Resize");
            if (Rows < 1) { Rows = 1; }
            if (Cols < 1) { Cols = 1; }
            Data = "";
            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    string cell = "0";
                    Data += cell;
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
#endif
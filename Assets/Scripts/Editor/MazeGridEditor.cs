using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGridBuilder))]
public class MazeGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var grid = (MazeGridBuilder)target;

        EditorGUILayout.BeginVertical();

        GUILayout.Label("WARNING: 0 - INACTIVE CELL \n 1 - ACTIVE CELL \n 2 - PLAYER \n 3 - TARGET \n 4 - COLLECTABLE");

        for (int y = 0; y < grid.Cols; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < grid.Rows; x++)
            {
                int n = grid.GetIndex(x, y);

                var cell = grid.Data.Substring(n, 1);

                GUI.color = Color.gray;
                if (cell == "1") GUI.color = Color.white; //inactive cell
                if (cell == "2") GUI.color = Color.yellow; // player
                if (cell == "3") GUI.color = Color.green; // target
                if (cell == "4") GUI.color = Color.cyan; // collectable

                if (GUILayout.Button(cell, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    grid.ToggleCell(x, y);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUI.color = Color.yellow;

        GUILayout.Label("DANGER ZONE BELOW THIS AREA!");

        GUI.color = Color.white;

        EditorGUILayout.EndVertical();

        DrawDefaultInspector();
        bool gridOK = true;
        string message = "";
        if (!grid.Data.Contains("2") || !grid.Data.Contains("3"))
        {
            gridOK = false;
            message = "You need to have and start and end set!";
        }
        else if (grid.LevelData.LevelExist(grid.Data))
        {
            gridOK = false;
            message = "This level already exist!";
        }
        if (!gridOK)
        {
            GUI.color = Color.yellow;
            GUILayout.Label(message);
        }
        if (gridOK)
        {
            if (GUILayout.Button("Save level"))
            {
                grid.LevelData.AddLevel(grid.Rows, grid.ActivableCellsCount);
                grid.ResetGrid();
            }
        }
    }
}

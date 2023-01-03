using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelsDefinition))]
public class LevelsDefinitionEditor : Editor
{
    private bool[] _opened;

    private void OnEnable()
    {
        LevelsDefinition levelsDefinition = (LevelsDefinition)target;
        _opened = new bool[levelsDefinition.AllLevels.Count];
    }

    public override void OnInspectorGUI()
    {
        LevelsDefinition levelsDefinition = (LevelsDefinition)target;
        int counter = 0;
        if (levelsDefinition.AllLevels.Count > 0)
        {
            foreach (Data data in levelsDefinition.AllLevels)
            {
                if (counter >= _opened.Length)
                {
                    List<bool> newList = new List<bool>(_opened);
                    newList.Add(false);
                    _opened = newList.ToArray();
                    Debug.Log("Added!");
                    continue;
                }
                _opened[counter] = EditorGUILayout.Foldout(_opened[counter], data.level);
                bool opened = _opened[counter];

                if (opened)
                {
                    data.level = EditorGUILayout.TextField(nameof(data.level), data.level);
                    data.levelData = EditorGUILayout.TextField(nameof(data.levelData), data.levelData);
                    data.gridLength = EditorGUILayout.IntField(nameof(data.gridLength), data.gridLength);
                    data.maxMoves = EditorGUILayout.IntField(nameof(data.maxMoves), data.maxMoves);
                    data.activableCellsCount = EditorGUILayout.IntField(nameof(data.activableCellsCount), data.activableCellsCount);

                    int idx = 0;
                    for (int y = 0; y < data.gridLength; y++)
                    {
                        GUILayout.BeginHorizontal();
                        for (int x = 0; x < data.gridLength; x++)
                        {
                            string value;
                            if (idx < data.levelData.Length)
                            {
                                value = data.levelData[idx].ToString();
                            }
                            else
                            {
                                value = "--";
                            }
                            idx++;
                            int index = levelsDefinition.GetIndex(x, y);
                            if (index >= 0)
                            {
                                string cell = levelsDefinition.Data.Substring(index, 1);
                                GUI.color = Color.gray;
                                if (cell == "1")
                                {
                                    GUI.color = Color.white; //inactive cell
                                }
                                if (cell == "2")
                                {
                                    GUI.color = Color.yellow; // player
                                }
                                if (cell == "3")
                                {
                                    GUI.color = Color.green; // target
                                }
                                if (cell == "4")
                                {
                                    GUI.color = Color.cyan; // collectable
                                }
                                if (GUILayout.Button(cell, GUILayout.Width(50), GUILayout.Height(50)))
                                {
                                    levelsDefinition.ToggleCell(x, y);
                                }
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Remove level"))
                    {
                        levelsDefinition.AllLevels.Remove(data);
                    }
                }
                ++counter;
            }
        }

        GUILayout.BeginHorizontal();
        int startAt = 3;
        int endAt = 8;
        for (int i = startAt; i <= endAt; i++)
        {
            if (GUILayout.Button($"Add {i}x{i} level"))
            {
                levelsDefinition.AddLevel(i, 0);
            }
        }
        GUILayout.EndHorizontal();
    }
}

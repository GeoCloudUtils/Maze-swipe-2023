using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static PathFinder;
using Random = UnityEngine.Random;

public class LevelData
{
    public bool pathFound;
    public List<Vec2> path;
    public int randomizerSeed;
    public int inactiveCells;
    public int gridSize;
}

public class Vec2
{
    public int x, y;

    public Vec2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class LevelsEditor : EditorWindow
{
    [Serializable]
    private class EditorData
    {
        public int[] data;
        public int gridSize;
        public int inactiveCells;
        public Vector2Int targetPos;
        public Vector2Int playerPos;
        internal bool[,] walkMap;
    }

    [SerializeField] private EditorData _data;
    [SerializeField] private PathInfo _pathInfo;
    [SerializeField] private bool _showPositions;
    [SerializeField] private int _randomizerSeed;
    [SerializeField] private int _customSeed;
    [SerializeField] private string _loadPath;
    [SerializeField] private Vector2 _scrollPos;
    private System.Random _randomizer = new System.Random();

    // UI.
    private const int UI_CELL_SIZE = 40;
    private GUILayoutOption[] _smallButton = new GUILayoutOption[] { GUILayout.Width(80) };
    private GUILayoutOption[] _mediumButton = new GUILayoutOption[] { GUILayout.Width(140) };
    private GUILayoutOption[] _mediumText = new GUILayoutOption[] { GUILayout.Width(200) };

    // Cells types.
    private const int ACTIVE_CELL = 0;
    private const int INACTIVE_CELL = 1;
    private const int PLAYER_CELL = 2;
    private const int TARGET_CELL = 3;
    private const int COLLECTABLE_CELL = 4;


    [MenuItem("-- Overact Games --/Levels Editor")]
    public static void ShowWindow()
    {
        LevelsEditor _window = GetWindow<LevelsEditor>(false, "Levels Editor");
        _window.minSize = new Vector2(550, 300);
        _window.Init();
    }

    public void Init()
    {
        _data = new EditorData()
        {
            gridSize = 5,
            data = new int[9] {1,1,1,1,1,1,1,1,1},
            inactiveCells = 7
        };
    }

    private void DrawError(string err)
    {
        Color color = GUI.color;
        GUI.color = Color.red;
        EditorGUILayout.LabelField(err);
        GUI.color = color;
    }
    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        EditorGUILayout.BeginVertical();

        // Level info -----------
        _data.gridSize = EditorGUILayout.IntField("Grid Size", _data.gridSize);
        _data.inactiveCells = EditorGUILayout.IntField("Inactive Cells", _data.inactiveCells);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show Positions: ");
        _showPositions = EditorGUILayout.Toggle(_showPositions);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Seed: " + _randomizerSeed);
        //----------------
        
        // Path info -------------
        EditorGUILayout.LabelField("---- Path Info ----");
        if(_pathInfo == null)
        {
            DrawError("Path Found: unknown");
        }
        else
        {
            if (_pathInfo.found)
            {
                EditorGUILayout.LabelField($"Path Found: TRUE");
                string path = "";
                _pathInfo.path.ForEach(e => path += $"[{e.y},{e.x}], ");
                EditorGUILayout.LabelField($"   * Path: {path}");
            }
            else
            {
                DrawError("Path Found: FALSE");
            }

            if (_pathInfo.valid)
            {
                EditorGUILayout.LabelField($"Path Valid: TRUE");
            }
            else
            {
                DrawError("Path Valid: FALSE");
            }
        }
        EditorGUILayout.LabelField("----------");
        //----------------

        // Errors -----------
        EditorGUILayout.LabelField("---- Errors ----");
        if (_data != null)
        {
            int deltaX = Mathf.Abs(_data.playerPos.x - _data.targetPos.x);
            int deltaY = Mathf.Abs(_data.playerPos.y - _data.targetPos.y);
            if(deltaX < 2 && deltaY < 2)
            {
                DrawError($"Player and Targer too close to each other. Delta:{deltaX},{deltaY}");
            }
        }
        else
        {
            EditorGUILayout.LabelField($"Nothing found.");
        }
        EditorGUILayout.LabelField("----------");
        //----------------

        // Draw maps.
        EditorGUILayout.BeginHorizontal();
        DrawMap(_data);
        DrawWalkMap(_data);
        EditorGUILayout.EndHorizontal();

        // Draw Navigation --------
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate", _smallButton))
        {
            RegenerateUntilNotValid();
        }
        if (GUILayout.Button("Save Level", _smallButton))
        {
            SaveLevel();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _customSeed = DrawInt("Seed:", _customSeed);
        if (GUILayout.Button("Generate with Seed", _mediumButton))
        {
            GenerateLevel(_customSeed);
            SearchPath();
            _pathInfo.valid = CheckIfPathValid();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _loadPath = DrawText("Path:", _loadPath);
        if (GUILayout.Button("Load Level", _smallButton))
        {
            LoadLevel();
        }
        EditorGUILayout.EndHorizontal();
        //----------------

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private string DrawText(string label, string text)
    {
        EditorGUILayout.LabelField(label, _smallButton);
        return EditorGUILayout.TextField(text, _mediumText);
    }

    private int DrawInt(string label, int val)
    {
        EditorGUILayout.LabelField(label, _smallButton);
        return EditorGUILayout.IntField(val, _mediumText);
    }

    #region Draw Methods
    private void DrawMap(EditorData data)
    {
        Color oldGUIColor = GUI.color;

        EditorGUILayout.BeginVertical();
        GUILayout.Label("Cells");
        int idx = 0;
        for (int y = 0; y < data.gridSize; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < data.gridSize; x++)
            {
                string value;
                if (idx < data.data.Length)
                {
                    value = data.data[idx].ToString();
                }
                else
                {
                    value = "--";
                }

                idx++;

                GUI.color = Color.gray;
                if (value == "0")
                {
                    GUI.color = Color.grey; //active cell
                    value = "";
                }
                if (value == "1")
                {
                    GUI.color = Color.black; //inactive cell
                    value = "x";
                }
                if (value == "2")
                {
                    GUI.color = Color.yellow; // player
                    value = "Player";
                }
                if (value == "3")
                {
                    GUI.color = Color.green; // target
                    value = "Finish";
                }
                if (value == "4")
                {
                    GUI.color = Color.cyan; // collectable
                }

                if (_showPositions)
                {
                    value = $"{y},{x}";
                }

                if (GUILayout.Button(value, GUILayout.Width(50), GUILayout.Height(50)))
                {

                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        GUI.color = oldGUIColor;
    }

    private void DrawWalkMap(EditorData data)
    {
        if(_pathInfo == null || !_pathInfo.found)
        {
            return;
        }

        EditorGUILayout.BeginVertical();
        GUILayout.Label("Path Map");
        int idx = 0;
        for (int y = 0; y < data.gridSize; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < data.gridSize; x++)
            {
                string value;
                if (idx < data.data.Length)
                {
                    value = data.data[idx].ToString();
                }
                else
                {
                    value = "--";
                }

                idx++;

                Color oldGUIColor = GUI.color;

                int pathIdx = _pathInfo.path.IndexOf(new Vector2Int(x, y));
                if(pathIdx != -1)
                {
                    GUI.color = Color.red;
                    value = pathIdx.ToString();
                }
                else
                {
                    GUI.color = Color.gray;
                    value = "";
                }

                if (GUILayout.Button(value, GUILayout.Width(50), GUILayout.Height(50)))
                {

                }

                GUI.color = oldGUIColor;
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
    #endregion


    #region Internal API
    /// <summary>
    /// Regenerate the level until not found a valid one.
    /// </summary>
    private void RegenerateUntilNotValid()
    {
        int attempts = 100;
        _pathInfo.valid = false;
        do
        {
            GenerateLevel();
            SearchPath();
            _pathInfo.valid = CheckIfPathValid();
        } while (!_pathInfo.valid && attempts-- > 0);
        Debug.Log($"Generated. Attempts Remained:{attempts}/100");

    }

    /// <summary>
    /// Save the level into file.
    /// </summary>
    private void SaveLevel()
    {
        List<Vec2> path = new List<Vec2>();

        if (_pathInfo.found)
        {
            _pathInfo.path.ForEach(e => path.Add(new Vec2(e.x, e.y)));
        }

        LevelData data = new LevelData()
        {
            path = path,
            pathFound = _pathInfo.found,
            randomizerSeed = _randomizerSeed,
            inactiveCells = _data.inactiveCells,
            gridSize = _data.gridSize
        };

        string filePath = Application.dataPath + "/SavedLevels/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        filePath += $"{Guid.NewGuid()}.json";

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);

        Debug.Log("Saved as: " + filePath);
    }

    /// <summary>
    /// Load the level from file.
    /// </summary>
    private void LoadLevel()
    {
        string filePath = Application.dataPath + "/SavedLevels/";

        if (!Directory.Exists(filePath))
        {
            Debug.LogError("Directory not exists! Dir: " + filePath);
            return;
        }

        filePath += $"{_loadPath}.json";

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not exists! Dir: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        LevelData data = JsonConvert.DeserializeObject<LevelData>(json);

        _data = new EditorData()
        {
            data = null,
            gridSize = data.gridSize,
            inactiveCells = data.inactiveCells,
            playerPos = default,
            targetPos = default,
            walkMap = null
        };

        _pathInfo = new PathInfo();

        GenerateLevel(data.randomizerSeed);
        SearchPath();
        _pathInfo.valid = CheckIfPathValid();

        Debug.Log("Loaded from: " + filePath);
    }

    /// <summary>
    /// Search path fro the current map.
    /// </summary>
    private void SearchPath()
    {
        // Create Walk Map.
        bool[,] walkMap = new bool[_data.gridSize, _data.gridSize];
        int idx = 0;
        for (int y = 0; y < _data.gridSize; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < _data.gridSize; x++)
            {
                walkMap[x, y] = _data.data[idx] != INACTIVE_CELL;
                ++idx;
            }
        }
        _data.walkMap = walkMap;

        // Find player and target position.
        _data.playerPos = default;
        _data.targetPos = default;
        idx = 0;
        for (int y = 0; y < _data.gridSize; y++)
        {
            for (int x = 0; x < _data.gridSize; x++)
            {
                int value = _data.data[idx];
                if (value == PLAYER_CELL)
                {
                    _data.playerPos = new Vector2Int(y, x);
                }
                else if (value == TARGET_CELL)
                {
                    _data.targetPos = new Vector2Int(y, x);
                }
                ++idx;
            }
        }

        // Search path.
        PathFinder finder = new PathFinder();
        _pathInfo = finder.SearchPath(_data.playerPos, _data.targetPos, walkMap);
    }

    /// <summary>
    /// Check if made path is valid.
    /// </summary>
    /// <returns>If valid</returns>
    private bool CheckIfPathValid()
    {
        // Path not found.
        if(!_pathInfo.found || _pathInfo.path == null)
        {
            return false;
        }

        bool[,] walkMap = _data.walkMap;
        
        // Transform points to lines.
        List<Vector2Int> path = _pathInfo.path;
        List<Vector2Int> lines = MathHelper.TransformLinesToPoints(path);

        // Check if lines are valid, so player can arrive from Start to Finish.
        for (int a = 0; a < lines.Count; a += 2)
        {
            Vector2Int p1 = lines[a];
            Vector2Int p2 = lines[a + 1];
            Vector2Int next;

            if (p1.x == p2.x)//Vertical.
            {
                if (p1.y < p2.y)// To Bottom.
                {
                    next = p2 + new Vector2Int(0, 1);
                }
                else // To Top.
                {
                    next = p2 + new Vector2Int(0, -1);
                }
            }
            else // Horizontal
            {
                if (p1.x < p2.x)// To Right.
                {
                    next = p2 + new Vector2Int(1, 0);
                }
                else // To Left.
                {
                    next = p2 + new Vector2Int(-1, 0);
                }
            }

            if (IsPointValid(next) && walkMap[next.x, next.y])
            {
                //Debug.LogError($"-> Point is invalid. Start:{Invert(p1)}; End:{Invert(p2)}; Next: {Invert(next)}");
                return false;
            }
            else
            {
                //Debug.Log($"-> Point valid. Start:{Invert(p1)}; End:{Invert(p2)}; Next: {Invert(next)}");
            }
        }

        return true;
    }

    /// <summary>
    ///  Generate the level.
    /// </summary>
    /// <param name="seed">If -1 then choose a random seed. If not -1, use the given seed.</param>
    private void GenerateLevel(int seed = -1)
    {
        if(seed == -1)
        {
            _randomizerSeed = _randomizer.Next(0, 100000000);
        }
        else
        {
            _randomizerSeed = seed;
        }
        Random.InitState(_randomizerSeed);

        int rows = _data.gridSize;
        int cols = _data.gridSize;
        int cellsCount = rows * cols;

        int[] values = new int[cellsCount];
        int idx, cellIdx;

        // Set all cells active.
        for (int a = 0; a < cellsCount; a++)
        {
            values[a] = ACTIVE_CELL;
        }

        // Get all cells.
        List<Vector2Int> cells = new List<Vector2Int>();
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                cells.Add(new Vector2Int(y, x));
            }
        }

        // Set some cells inactive.
        for(int a=0; a < _data.inactiveCells; ++a)
        {
            idx = Random.Range(0, cells.Count);
            cellIdx = PosToIndex(cells[idx]);
            SetAt(values, cellIdx, INACTIVE_CELL);
            cells.RemoveAt(idx);
        }

        // Set player cell.
        idx = Random.Range(0, cells.Count);
        cellIdx = PosToIndex(cells[idx]);
        SetAt(values, cellIdx, PLAYER_CELL);
        cells.RemoveAt(idx);

        // Set target cell.
        idx = Random.Range(0, cells.Count);
        cellIdx = PosToIndex(cells[idx]);
        SetAt(values, cellIdx, TARGET_CELL);
        cells.RemoveAt(idx);

        _data.data = values;
    }
    #endregion


    #region Helpers
    private void SetAt(int[] arr, int index, int value)
    {
        arr[index] = value;
    }

    private int PosToIndex(Vector2Int pos)
    {
        int rows = _data.gridSize;
        return pos.y * rows + pos.x;
    }

    private Vector2Int Invert(Vector2Int pos)
    {
        return new Vector2Int(pos.y, pos.x);
    }

    bool IsPointValid(Vector2Int point)
    {
        if (point.x < 0 || point.y < 0 || point.x >= _data.gridSize || point.y >= _data.gridSize)
        {
            return false;
        }
        return true;
    }
    #endregion
}

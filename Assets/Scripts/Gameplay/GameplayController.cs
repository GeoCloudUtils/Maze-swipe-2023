using DG.Tweening;
using GooglePlayGames;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour
{
    private static GameplayController _instance;
    public static GameplayController Instance { get { return _instance; } }

    public int ActivableCellsCount { get => activableCellsCount; private set => activableCellsCount = value; }

    public List<Cell> LevelCells => levelCells;

    public bool SpawnComplete { get => spawnComplete;private set => spawnComplete = value; }

    [SerializeField] private GameObject rewardAdPanel;

    [SerializeField] private GameViewController gameViewController;

    [SerializeField] private InterstitialAdButton reloadButton;

    [SerializeField] private Button debugReset;

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text movesText;

    [SerializeField] private GameObject activableCellPrefab;

    [SerializeField] private Transform activableCellContainer;

    [SerializeField] private MazeSpawner spawner;

    [SerializeField] private LevelsDefinition levelDefinition;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform spawnContainer;

    [SerializeField] private bool canSwipe = false;

    [Header("Testing")]
    public bool testMode = false;

    [SerializeField] private int mazeIndex = 0;
    [SerializeField] private int collectablesCount = 0;
    [SerializeField] private int activableCellsCount = 0;
    [SerializeField] private int movesLeft;

    [SerializeField] private Data mazeData;

    [SerializeField] private List<GameObject> activableCellsList = new List<GameObject>();

    private readonly List<string> achievementsList = new List<string>();

    private List<Cell> levelCells = new List<Cell>();

    private int totalMoves = 0;

    private int reloadCount = 0;

    private Player player;

    private bool spawnComplete = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void AddMoves(int count)
    {
        movesLeft += count;
        movesText.SetText(movesLeft.ToString());
    }

    private void Start()
    {
        spawner.SpawnComplete += OnSpawnComplete;
        canSwipe = false;

        achievementsList.Add(GPGSIds.achievement_a_good_start);
        achievementsList.Add(GPGSIds.achievement_enthusiast);
        achievementsList.Add(GPGSIds.achievement_trainee);
        achievementsList.Add(GPGSIds.achievement_brainy);
        achievementsList.Add(GPGSIds.achievement_advanced);
        achievementsList.Add(GPGSIds.achievement_expert);
        achievementsList.Add(GPGSIds.achievement_master);

        reloadButton.OnInterstitialAdButtonClick += DoReload;
        debugReset.onClick.AddListener(DoReset);
        if (!testMode)
        {
            if (!PlayerPrefs.HasKey("MAZE_INDEX"))
            {
                PlayerPrefs.SetInt("MAZE_INDEX", 0);
            }
            mazeIndex = PlayerPrefs.GetInt("MAZE_INDEX");
        }
        RunNextLevel();
        reloadCount = PlayerPrefs.GetInt("ReloadCount");
    }

    ///debug
    private void DoReset()
    {
        PlayerPrefs.DeleteAll();
        DoReload();
    }

    private void DoReload()
    {
        reloadCount++;
        if (reloadCount >= 5)
        {
            reloadCount = 0;
            PlayerPrefs.SetInt("ReloadCount", reloadCount);
            reloadButton.LoadAd();
            return;
        }
        PlayerPrefs.SetInt("ReloadCount", reloadCount);
        Reload();
    }

    public void Reload()
    {
        SceneManager.LoadScene(0);
    }

    private void RunNextLevel()
    {
        if (mazeIndex > levelDefinition.AllLevels.Count - 1)
        {
            Debug.LogWarning("NO MORE LEVELS!");
            return;
        }
        mazeData = levelDefinition.AllLevels[mazeIndex];
        movesLeft = mazeData.maxMoves + 1;

        Debug.Log("Level data: " + mazeData.levelData);
        spawner.Init(mazeData);
        mazeIndex++;
        moves = 0;

        if (activableCellsList.Count > 0)
        {
            foreach (var activableCell in activableCellsList)
            {
                Destroy(activableCell);
            }
        }

        activableCellsList = new List<GameObject>();
        for (int i = 0; i < mazeData.activableCellsCount; i++)
        {
            activableCellsList.Add(Instantiate(activableCellPrefab, activableCellContainer));
            Transform cell = activableCellsList[i].transform;
            cell.DOScale(1f, 0.25f).From(0f).SetDelay(i * 0.01f);
        }
        activableCellsCount = mazeData.activableCellsCount;
        levelText.SetText($"Level {mazeIndex}");
        totalMoves = 0;
        LevelTimer.Instance.SetTimerRunning(true);
    }

    private void OnSpawnComplete(Player player, Cell[] cells)
    {
        inputManager.OnSwipe += OnSwipe;
        this.player = player;
        this.player.MoveComplete += OnPlayerMoveCompleted;
        this.player.Init();
        levelCells = cells.ToList();
        foreach (var cell in cells)
        {
            cell.OnCellEnabled += CellEnabled;
        }
        SpawnComplete = true;
    }

    private void CellEnabled()
    {
        activableCellsCount--;
        activableCellsList[activableCellsCount].transform.DOScale(0f, 0.25f).From(1.1f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            activableCellsList[activableCellsCount].SetActive(false);
        });
    }

    private void OnPlayerMoveCompleted(bool reachFinish)
    {
        movesLeft--;
        if (movesLeft < 0)
        {
            movesLeft = 0;
        }
        movesText.SetText(movesLeft.ToString());
        if (reachFinish)
        {
            Debug.Log("END LEVEL!");
            SoundController.Instance.PlaySfx(SoundController.SoundType.WIN);
#if UNITY_EDITOR
            int defMoves = levelDefinition.AllLevels[mazeIndex - 1].maxMoves;
            if (defMoves > 0)
            {
                if (defMoves > moves)
                {
                    levelDefinition.AllLevels[mazeIndex - 1].maxMoves = moves;
                }
            }
            else
            {
                levelDefinition.AllLevels[mazeIndex - 1].maxMoves = moves;
            }
#endif
            foreach (var achievement in achievementsList)
            {
                IncrementAchievement(achievement);
            }
            Social.LoadScores(GPGSIds.leaderboard_players, scores =>
            {
                if (scores.Length > 0)
                {
                    Debug.Log("Got " + scores.Length + " scores");
                    string myScores = "Leaderboard:\n";
                    foreach (IScore score in scores)
                    {
                        myScores += "\t" + score.userID + " " + score.formattedValue + " " + score.date + "\n";
                    }
                    Debug.Log(myScores);
                }
                else
                    Debug.Log("No scores loaded");
            });
            LevelTimer.Instance.SetTimerRunning(false);
            int score = LevelTimer.Instance.GetPoints();
            Social.ReportScore(score * 100, GPGSIds.leaderboard_players, (succes) =>
            {

            });
            PlayerPrefs.SetInt("MAZE_INDEX", mazeIndex);
            Invoke(nameof(RunNextLevel), 1.0f);
            return;
        }
        else
        {
            if (!testMode)
            {
                if (movesLeft <= 0)
                {
                    Debug.Log("LEVEL FAIL!");
                    SoundController.Instance.PlaySfx(SoundController.SoundType.LOST);
                    rewardAdPanel.SetActive(true);
                    canSwipe = false;
                }
            }
        }
        canSwipe = true;
    }

    private void IncrementAchievement(string achievement)
    {
        PlayGamesPlatform platform = (PlayGamesPlatform)Social.Active;
        platform.IncrementAchievement(achievement, 1, (bool succes) =>
        {

        });
    }

    int moves = 0;
    private void OnSwipe(InputManager.Direction swipeDirection)
    {
        if (!canSwipe)
        {
            return;
        }
        if (!testMode)
        {
            if (movesLeft <= 0)
            {
                rewardAdPanel.SetActive(true);
                return;
            }
        }
        switch (swipeDirection)
        {
            case InputManager.Direction.Left:
                RotatePanel(-90f);
                break;
            case InputManager.Direction.Right:
                RotatePanel(90f);
                break;
            case InputManager.Direction.Up:
                gameViewController.ShowMenuPanel();
                break;
            default:
                break;
        }
    }

    private void RotatePanel(float angle)
    {
        SoundController.Instance.PlaySfx(SoundController.SoundType.SWIPE);
        canSwipe = false;
        Vector3 currentRotation = spawnContainer.eulerAngles;
        Vector3 newRotation = new Vector3(currentRotation.x, currentRotation.y, Mathf.RoundToInt(currentRotation.z + angle));
        spawnContainer.DORotate(newRotation, .5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            player.Init();
        });
        moves++;
    }
}

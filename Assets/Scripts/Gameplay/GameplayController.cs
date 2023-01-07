using DG.Tweening;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class GameplayController : MonoBehaviour
{
    private static GameplayController _instance;
    public static GameplayController Instance { get { return _instance; } }

    public int ActivableCellsCount { get => activableCellsCount; private set => activableCellsCount = value; }

    public List<Cell> LevelCells => levelCells;

    public bool SpawnComplete { get => spawnComplete; private set => spawnComplete = value; }

    [SerializeField] private GameObject rewardAdPanel;

    [SerializeField] private UIFinishScreen finishScreen;

    [SerializeField] private GameViewController gameViewController;

    [SerializeField] private InterstitialAdButton reloadButton;

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text movesText;

    [SerializeField] private GameObject activableCellPrefab;

    [SerializeField] private Transform activableCellContainer;

    [SerializeField] private MazeSpawner spawner;

    [SerializeField] private LevelsDefinition levelDefinition;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform spawnContainer;

    [SerializeField] private bool canSwipe = false;

    [SerializeField] private int mazeIndex = 0;
    [SerializeField] private int collectablesCount = 0;
    [SerializeField] private int activableCellsCount = 0;
    [SerializeField] private int movesLeft;

    [SerializeField] private Data mazeData;

    [SerializeField] private List<GameObject> activableCellsList = new List<GameObject>();

    private readonly List<string> achievementsList = new List<string>();

    private List<Cell> levelCells = new List<Cell>();

    private int reloadCount = 0;

    private Player player;

    private bool spawnComplete = false;

    private bool levelComplete = false;

    private int currentLevelIndex;
    private int score;
    private int diamonds;
    private int totalMoves = 0;
    private int rewardCount;

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

        RunNextLevel();

        reloadCount = PlayerPrefs.GetInt("ReloadCount");
        finishScreen.OnNext += RunNextLevel;
    }

    /// <summary>
    /// Reload callback but saving reload count
    /// Used to show interstitial ads
    /// </summary>
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

    /// <summary>
    /// Reload current level
    /// </summary>
    public void Reload()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Run next level
    /// </summary>
    private void RunNextLevel()
    {
        finishScreen.Hide();
        gameViewController.Show();
        inputManager.gameObject.SetActive(true);

        if (mazeIndex > levelDefinition.AllLevels.Count - 1)
        {
            Debug.LogWarning("NO MORE LEVELS!");
            //to do
            return;
        }
        mazeData = levelDefinition.AllLevels[mazeIndex];
        movesLeft = mazeData.maxMoves + 1;

        spawner.Init(mazeData);
        mazeIndex++;

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
        levelComplete = false;
    }

    /// <summary>
    /// Level spawn complete callback
    /// Activate all cells events
    /// </summary>
    /// <param name="player"></param>
    /// <param name="cells"></param>
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

    /// <summary>
    /// Callback on maze cell enabled
    /// Remove activable cells if exist
    /// </summary>
    private void CellEnabled()
    {
        activableCellsCount--;
        activableCellsList[activableCellsCount].transform.DOScale(0f, 0.25f).From(1.1f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            activableCellsList[activableCellsCount].SetActive(false);
        });
    }

    /// <summary>
    /// Callback on player move complete
    /// Check's if player reach the end 
    /// </summary>
    /// <param name="reachFinish"></param>
    private void OnPlayerMoveCompleted(bool reachFinish)
    {
        if (levelComplete) { return; }
        movesLeft--;
        if (movesLeft < 0)
        {
            movesLeft = 0;
        }
        movesText.SetText(movesLeft.ToString());
        if (reachFinish)
        {
            levelComplete = true;
            Debug.Log("END LEVEL!");
            SoundController.Instance.PlaySfx(SoundController.SoundType.WIN);
#if UNITY_EDITOR
            int defMoves = levelDefinition.AllLevels[mazeIndex - 1].maxMoves;
            if (defMoves > 0)
            {
                if (defMoves > totalMoves)
                {
                    levelDefinition.AllLevels[mazeIndex - 1].maxMoves = totalMoves;
                }
            }
            else
            {
                levelDefinition.AllLevels[mazeIndex - 1].maxMoves = totalMoves;
            }
#endif
            foreach (var achievement in achievementsList)
            {
                IncrementAchievement(achievement);
            }

            ///to remove
            ///not in use
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
            ///------------------
            ///

            StartCoroutine(OnFinishCoroutine());
            return;
        }
        else
        {
            if (movesLeft <= 0)
            {
                SoundController.Instance.PlaySfx(SoundController.SoundType.LOST);
                rewardAdPanel.SetActive(true);
                canSwipe = false;
                return;
            }
        }
        canSwipe = true;
    }


    /// <summary>
    /// Coroutine on level complete
    /// Show's finish screen and calculate results
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnFinishCoroutine()
    {
        yield return new WaitForSeconds(1f);
        inputManager.gameObject.SetActive(false);
        gameViewController.Hide();
        finishScreen.Show(GetLevelResults());
        Social.ReportScore(score, GPGSIds.leaderboard_players, (succes) =>
        {
            //need to do something? =/
        });
    }

    /// <summary>
    /// Calculate level results
    /// </summary>
    /// <returns></returns>
    private LevelResults GetLevelResults()
    {
        currentLevelIndex = mazeIndex;
        score = 100; // need formula
        diamonds = UnityEngine.Random.Range(1, 4);// get 1 , 2 or 3 diamonds per level
        rewardCount = UnityEngine.Random.Range(2, 4);// get 2x or 3x more diamonds 

        return new LevelResults
        {
            level = currentLevelIndex,
            score = score,
            diamonds = diamonds,
            moves = totalMoves,
            reward = rewardCount
        };
    }

    /// <summary>
    /// Callback for increasing achievements by name
    /// </summary>
    /// <param name="achievement"></param>
    private void IncrementAchievement(string achievement)
    {
        PlayGamesPlatform platform = (PlayGamesPlatform)Social.Active;
        platform.IncrementAchievement(achievement, 1, (bool succes) =>
        {
            //need to do something? =/
        });
    }

    /// <summary>
    /// Callback on maze swipe
    /// Check's direction and rotate maze by angle
    /// </summary>
    /// <param name="swipeDirection"></param>
    private void OnSwipe(InputManager.Direction swipeDirection)
    {
        if (!canSwipe) { return; }

        if (movesLeft <= 0)
        {
            rewardAdPanel.SetActive(true);
            return;
        }
        switch (swipeDirection)
        {
            case InputManager.Direction.Left:
                RotateMaze(-90f);
                break;
            case InputManager.Direction.Right:
                RotateMaze(90f);
                break;
            case InputManager.Direction.Up:
                gameViewController.ShowMenuPanel();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Main maze rotate function
    /// Rotate maze using smooth tween
    /// </summary>
    /// <param name="angle"></param>
    private void RotateMaze(float angle)
    {
        SoundController.Instance.PlaySfx(SoundController.SoundType.SWIPE);
        canSwipe = false;
        Vector3 currentRotation = spawnContainer.eulerAngles;
        Vector3 newRotation = new Vector3(currentRotation.x, currentRotation.y, Mathf.RoundToInt(currentRotation.z + angle));
        spawnContainer.DORotate(newRotation, .5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            player.Init();
            totalMoves++;
        });
    }
}

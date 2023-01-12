using DG.Tweening;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class GameplayController : Singleton<GameplayController>
{
    [SerializeField] private UIRewardRequestScreen rewardRequestScreen;
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

    public bool SpawnComplete { get => spawnComplete; private set => spawnComplete = value; }
    public int ActivableCellsCount { get => activableCellsCount; private set => activableCellsCount = value; }
    public int cellDestroyCost = 2;
    public List<Cell> LevelCells => levelCells;

    private readonly List<string> achievementsList = new List<string>();
    private List<Cell> levelCells = new List<Cell>();

    private int reloadCount = 0;
    private Player player;
    private Cell clickedCell;
    private bool spawnComplete = false;
    private bool levelComplete = false;

    private int currentLevelIndex;
    private int score;
    private int diamonds;
    private int totalMoves = 0;
    [SerializeField] private int levelScore = 0;
    private readonly int diamondsRewardCount = 3;

    private int levelTotalPoints = 720;

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
        rewardRequestScreen.OnRewardAdComplete += GetReward;

        RunNextLevel();

        reloadCount = PlayerPrefs.GetInt("ReloadCount");
        finishScreen.OnNext += RunNextLevel;
    }

    /// <summary>
    /// Callback on reward ad complete
    /// Give's moves or diamonds based on reward type
    /// </summary>
    /// <param name="rewardType"></param>
    private void GetReward(RewardType rewardType)
    {
        if (rewardType == RewardType.MOVES)
        {
            movesLeft += 3; // ??
            movesText.SetText(movesLeft.ToString());
        }
        else
        {
            DataManager.Instance.currentData.diamonds += diamondsRewardCount;
            clickedCell.isElementActive = false;
            clickedCell.SetState(0.1f);
        }
        rewardRequestScreen.Hide();
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
        levelScore = levelTotalPoints;
        mazeData = levelDefinition.AllLevels[mazeIndex];
        movesLeft = mazeData.maxMoves + 1;
        movesText.SetText(movesLeft.ToString());

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
        canSwipe = true;
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
        this.player.captureInputEvents = true;
        levelCells = cells.ToList();
        foreach (var cell in cells)
        {
            cell.OnCellClickDelegate += OnCellClick;
        }
        SpawnComplete = true;
    }

    /// <summary>
    /// Callback on maze cell enabled
    /// Remove activable cells if exist
    /// </summary>
    private void OnCellClick(ActionType actionType, Cell clickedCell)
    {
        this.clickedCell = clickedCell;
        if (actionType == ActionType.ACTIVABLE_CELL_REQUEST)
        {
            activableCellsCount--;
            activableCellsList[activableCellsCount].transform.DOScale(0f, 0.25f).From(1.1f).SetEase(Ease.InOutBack).OnComplete(() =>
            {
                activableCellsList[activableCellsCount].SetActive(false);
            });
        }
        else if (actionType == ActionType.DIAMONDS_REQUEST)
        {
            rewardRequestScreen.Show(RewardType.DIAMONDS);
        }
        else
        {
            DataManager.Instance.currentData.diamonds -= cellDestroyCost;
            if (DataManager.Instance.currentData.diamonds < 0)
            {
                DataManager.Instance.currentData.diamonds = 0;
            }
        }
    }

    /// <summary>
    /// Callback on player move complete
    /// Check's if player reach the end 
    /// </summary>
    /// <param name="reachFinish"></param>
    private void LevelWin()
    {
        if (levelComplete)
        {
            return;
        };
        foreach (Cell cell in LevelCells)
        {
            cell.captureEvents = false;
        }
        player = null;
        levelComplete = true;
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

        int guarantedPoints = 25;
        score = Mathf.Max(levelScore, guarantedPoints);

        diamonds = UnityEngine.Random.Range(1, 4);// get 1 , 2 or 3 diamonds per level
        return new LevelResults
        {
            level = currentLevelIndex,
            score = score,
            diamonds = diamonds,
            moves = totalMoves,
            reward = diamondsRewardCount
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
        if (!canSwipe || player.isMoving) { return; }
        if (swipeDirection == InputManager.Direction.Left || swipeDirection == InputManager.Direction.Right)
        {
            if (movesLeft <= 0)
            {
                rewardRequestScreen.Show(RewardType.MOVES);
                return;
            }
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
        player.captureInputEvents = false;
        SoundController.Instance.PlaySfx(SoundController.SoundType.SWIPE);
        canSwipe = false;
        Vector3 currentRotation = spawnContainer.eulerAngles;
        Vector3 newRotation = new Vector3(currentRotation.x, currentRotation.y, Mathf.RoundToInt(currentRotation.z + angle));
        movesLeft--;
        movesText.SetText(movesLeft.ToString());
        spawnContainer.DORotate(newRotation, .5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            totalMoves++;
            canSwipe = true;
            player.captureInputEvents = true;
            int decreasePerMove = 25;
            if (totalMoves > mazeData.maxMoves)
            {
                levelScore -= decreasePerMove;
            }
        });
    }

    private void Update()
    {
        if (player != null)
        {
            if (player.isMoving)
            {
                return;
            }
            if (player.isOnEndCell)
            {
                canSwipe = false;
                LevelWin();
            }
        }
    }
}


using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFinishScreen : MonoBehaviour
{
    [SerializeField] private Transform _root;

    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _diamondsText;
    [SerializeField] private TMP_Text _moveText;
    [SerializeField] private TMP_Text _getMoreDiamondsText;

    [SerializeField] private RewardedAdsButton _getRewardButton;
    [SerializeField] private Button _nextButton;

    private int rewardCount;

    public event Action OnNext;

    private void Start()
    {
        _getRewardButton.RewardAdComplete += GetReward;
        _nextButton.onClick.AddListener(() => OnNext?.Invoke());
    }

    private void GetReward()
    {
        DataManager.Instance.currentData.diamonds += rewardCount;
        _diamondsText.SetText($"Diamonds +<color=#2980B9>{DataManager.Instance.currentData.diamonds}</color>");
        _getRewardButton._showAdButton.interactable = false;
    }

    public void Show(LevelResults results)
    {
        _root.gameObject.SetActive(true);
        _levelText.SetText($"Level\n{results.level}");
        _scoreText.SetText($"Score <color=#2980B9>{results.score}</color>");
        _diamondsText.SetText($"Diamonds <color=#2980B9>+{results.diamonds}</color>");
        _moveText.SetText($"in <color=#2980B9>{results.moves}</color> moves");
        _getMoreDiamondsText.SetText($"Get <color=#2980B9>{results.reward}x</color> more");

        this.rewardCount = results.reward;

        _getRewardButton._showAdButton.interactable = true;

        DataManager.Instance.currentData.score += results.score;
        DataManager.Instance.currentData.level = results.level;
        DataManager.Instance.currentData.diamonds += results.diamonds;
    }

    public void Hide()
    {
        _root.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class LevelResults
{
    public int level;
    public int score;
    public int diamonds;
    public int moves;
    public int reward;
}

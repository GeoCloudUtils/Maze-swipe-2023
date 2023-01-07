using System;
using TMPro;
using UnityEngine;

public enum RewardType
{
    MOVES = 0,
    DIAMONDS = 1
}
public class UIRewardRequestScreen : MonoBehaviour
{
    [SerializeField] private GameObject _root;

    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _buttonText;

    [SerializeField] private RewardedAdsButton _rewardButton;

    public RewardType rewardRequestType;

    public event Action<RewardType> OnRewardAdComplete;

    private const string diamondTitleText = "No more diamonds?";
    private const string movesTitleText = "No more moves?";
    private const string diamondButtonDescriptionText = "Get 3 more diamonds?";
    private const string moveButtonDescriptionText = "Get 3 more moves?";

    private void Start()
    {
        _rewardButton.RewardAdComplete += () => OnRewardAdComplete?.Invoke(rewardRequestType);
    }

    public void Show(RewardType rewardRequestType)
    {
        this.rewardRequestType = rewardRequestType;
        _root.SetActive(true);
        string title = rewardRequestType == RewardType.DIAMONDS ? diamondTitleText : movesTitleText;
        string description = rewardRequestType == RewardType.DIAMONDS ? diamondButtonDescriptionText : moveButtonDescriptionText;
        _titleText.SetText(title);
        _buttonText.SetText(description);
    }

    public void Hide()
    {
        _root.SetActive(false);
    }
}

using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class InterstitialAdButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] Button _targetButton;

    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";

    private string _adUnitId;

    public event Action OnInterstitialAdButtonClick;

    void Awake()
    {
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOsAdUnitId : _androidAdUnitId;
    }

    private void Start()
    {
        _targetButton.onClick.AddListener(() => OnInterstitialAdButtonClick?.Invoke());
    }

    public void LoadAd()
    {
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Showing Ad: " + adUnitId);
        ShowAd();
        loadAttempts = 0;
    }

    public void ShowAd()
    {
        Advertisement.Show(_adUnitId, this);
    }

    int loadAttempts = 0;
    int showAttempts = 0;
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error} - {message}");
        if (loadAttempts > 5)
        {
            loadAttempts = 0;
            return;
        }
        LoadAd();
        loadAttempts++;
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error} - {message}");
        if (showAttempts > 5)
        {
            showAttempts = 0;
            return;
        }
        ShowAd();
        showAttempts++;
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        showAttempts = 0;
        GameplayController.Instance.Reload();
    }
}

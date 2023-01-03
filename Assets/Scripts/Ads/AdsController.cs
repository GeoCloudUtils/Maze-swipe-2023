using UnityEngine;
using UnityEngine.Advertisements;
public class AdsController : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameID;
    [SerializeField] string _iOSGameID;

    [SerializeField] bool _testMode = true;

    private string _gameID;
    private void Awake()
    {
        _gameID = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSGameID : _androidGameID;
        Advertisement.Initialize(_gameID, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Advertisment initialization complete!");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log("Advertisment initialization  failed. Reason: " + error + ". Message: " + message);
    }
}

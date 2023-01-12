using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

/// <summary>
/// GPGS Controller
/// Used for LogIn into google play game services
/// </summary>
public class GPGS_Controller : MonoBehaviour
{
    public PlayGamesClientConfiguration clientConfiguration;

    private void Start()
    {
        Configure();
        SignIntoGPGS(SignInInteractivity.CanPromptAlways, clientConfiguration);
    }

    /// <summary>
    /// Client configuration callback
    /// </summary>
    internal void Configure()
    {
        clientConfiguration = new PlayGamesClientConfiguration.Builder().Build();
    }

    /// <summary>
    /// Sign In callback into GPGS
    /// </summary>
    /// <param name="interactivity"></param>
    /// <param name="configuration"></param>
    internal void SignIntoGPGS(SignInInteractivity interactivity, PlayGamesClientConfiguration configuration)
    {
        configuration = clientConfiguration;
        PlayGamesPlatform.InitializeInstance(configuration);
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(interactivity, (code) =>
        {
            if (code == SignInStatus.Success)
            {
                Debug.Log($"Authentification complete. Data: {Social.localUser.userName}. {Social.localUser.id}");
            }
            else
            {
                Debug.LogWarning($"Authentification failed! Reason: {code}");
            }
        });
    }
}

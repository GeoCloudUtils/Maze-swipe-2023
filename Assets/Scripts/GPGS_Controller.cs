using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGS_Controller : MonoBehaviour
{
    public PlayGamesClientConfiguration clientConfiguration;

    private void Start()
    {
        Configure();
        SignIntoGPGS(SignInInteractivity.CanPromptAlways, clientConfiguration);
    }

    internal void Configure()
    {
        clientConfiguration = new PlayGamesClientConfiguration.Builder().Build();
    }

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

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameViewController : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    [SerializeField] private Button menuButton;

    [SerializeField] private Image sfxImage;
    [SerializeField] private Image musicImage;

    [SerializeField] private Sprite sfxInactiveSprite;
    [SerializeField] private Sprite sfxActiveSprite;
    [SerializeField] private Sprite musicInactiveSprite;
    [SerializeField] private Sprite musicActiveSprite;

    [SerializeField] private Button soundButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button iapButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button achievementsButton;

    [SerializeField] private float hideDelay = 5f;


    private void Start()
    {
        soundButton.onClick.AddListener(() => SetSound(true));
        sfxButton.onClick.AddListener(() => SetSfx(true));
        iapButton.onClick.AddListener(SetIap);
        leaderboardButton.onClick.AddListener(ShowLeaderboard);
        achievementsButton.onClick.AddListener(ShowAchievements);
        if (!PlayerPrefs.HasKey("SFX"))
        {
            PlayerPrefs.SetInt("SFX", 1);
        }
        SetSfx(false);
        SetSound(false);
        menuButton.onClick.AddListener(OpenMenu);
    }

    private Coroutine menuCoroutine;
    private void OpenMenu()
    {
        menuButton.gameObject.SetActive(false);
        if (menuCoroutine != null)
        {
            StopCoroutine(menuCoroutine);
        }
        menu.SetActive(true);
        menuCoroutine = StartCoroutine(MenuHideCoroutine());
    }

    private IEnumerator MenuHideCoroutine()
    {
        yield return new WaitForSeconds(hideDelay);
        menu.SetActive(false);
        menuCoroutine = null;
        menuButton.gameObject.SetActive(true);
    }

    private void CancelMenuHide()
    {
        if (menuCoroutine != null)
        {
            StopCoroutine(menuCoroutine);
        }
        menuCoroutine = StartCoroutine(MenuHideCoroutine());
    }

    private void ShowAchievements()
    {
        Social.ShowAchievementsUI();
        CancelMenuHide();
    }

    private void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
        CancelMenuHide();
    }

    private void SetIap()
    {
        CancelMenuHide();
    }

    private void SetSfx(bool save)
    {
        if (save)
        {
            if (!PlayerPrefs.HasKey("SFX"))
            {
                PlayerPrefs.SetInt("SFX", 1);
            }
            if (PlayerPrefs.GetInt("SFX") == 0)
            {
                PlayerPrefs.SetInt("SFX", 1);
            }
            else
            {
                PlayerPrefs.SetInt("SFX", 0);
            }
        }
        bool isActive = PlayerPrefs.GetInt("SFX") == 1;
        sfxImage.sprite = isActive ? sfxActiveSprite : sfxInactiveSprite;
        CancelMenuHide();
    }

    private void SetSound(bool save)
    {
        if (save)
        {
            if (!PlayerPrefs.HasKey("MUSIC"))
            {
                PlayerPrefs.SetInt("MUSIC", 1);
            }
            if (PlayerPrefs.GetInt("MUSIC") == 1)
            {
                SoundController.Instance.StopMusic();
                PlayerPrefs.SetInt("MUSIC", 0);
            }
            else
            {
                PlayerPrefs.SetInt("MUSIC", 1);
                SoundController.Instance.PlayMusic();
            }
        }
        bool isActive = PlayerPrefs.GetInt("MUSIC") == 1;
        musicImage.sprite = isActive ? musicActiveSprite : musicInactiveSprite;
        CancelMenuHide();
    }
}

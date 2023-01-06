using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public enum ColorScheme
{
    LIGHT = 0,
    DARK = 1
}

public class GameViewController : MonoBehaviour
{
    [SerializeField] private Color colorOnLightScheme;
    [SerializeField] private Color colorOnDarkScheme;

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
    [SerializeField] private Button colorSchemeChangeButton;

    [SerializeField] private float hideDelay = 5f;

    private Coroutine menuCoroutine;

    public ColorScheme colorScheme = ColorScheme.LIGHT;

    private ColorScheme lastSavedColorScheme;

    private List<GameViewElement> gameViewElements;

    private IEnumerator Start()
    {
        soundButton.onClick.AddListener(() => OnSoundButtonClick(true));
        sfxButton.onClick.AddListener(() => OnSfxButtonClick(true));
        iapButton.onClick.AddListener(OnIapButtonClick);
        leaderboardButton.onClick.AddListener(ShowLeaderboard);
        achievementsButton.onClick.AddListener(ShowAchievements);
        colorSchemeChangeButton.onClick.AddListener(() => ChangeColorScheme(true));
        if (!PlayerPrefs.HasKey("SFX"))
        {
            PlayerPrefs.SetInt("SFX", 1);
        }
        OnSfxButtonClick(false);
        OnSoundButtonClick(false);
        menuButton.onClick.AddListener(ShowMenuPanel);
        if (!PlayerPrefs.HasKey("COLOR_SCHEME"))
        {
            PlayerPrefs.SetInt("COLOR_SCHEME", 0);
        }
        colorScheme = (ColorScheme)PlayerPrefs.GetInt("COLOR_SCHEME");
        lastSavedColorScheme = colorScheme;
        while (!GameplayController.Instance.SpawnComplete)
        {
            yield return null;
        }
        gameViewElements = FindObjectsOfType<GameViewElement>().ToList();
        ChangeColorScheme(false);
    }

    private void ChangeColorScheme(bool flag)
    {
        if (flag)
        {
            if (colorScheme == ColorScheme.LIGHT)
            {
                colorScheme = ColorScheme.DARK;
            }
            else
            {
                colorScheme = ColorScheme.LIGHT;
            }
        }
        foreach (var cell in GameplayController.Instance.LevelCells)
        {
            if (!cell.IsEnabled)
            {
                cell.ChangeColor(colorScheme);
            }
        }
        lastSavedColorScheme = colorScheme;
        PlayerPrefs.SetInt("COLOR_SCHEME", (int)colorScheme);
        CancelMenuHide();
    }

    public void ShowMenuPanel()
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

    private void OnIapButtonClick()
    {
        CancelMenuHide();
        //to do
    }

    private void OnSfxButtonClick(bool save)
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

    private void OnSoundButtonClick(bool save)
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

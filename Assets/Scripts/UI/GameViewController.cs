using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public enum ColorScheme
{
    LIGHT = 0,
    DARK = 1
}

public class GameViewController : Singleton<GameViewController>
{
    [Header("General references")]
    [SerializeField] private GameObject menu;

    [SerializeField] private Button menuButton;

    [SerializeField] private Image sfxImage;
    [SerializeField] private Image musicImage;
    [SerializeField] private Image colorSchemeImage;

    [SerializeField] private Sprite sfxInactiveSprite;
    [SerializeField] private Sprite sfxActiveSprite;
    [SerializeField] private Sprite musicInactiveSprite;
    [SerializeField] private Sprite musicActiveSprite;

    [SerializeField] private Sprite colorSchemeDarkSprite;
    [SerializeField] private Sprite colorSchemeLightSprite;

    [SerializeField] private Button soundButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button iapButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button achievementsButton;
    [SerializeField] private Button colorSchemeChangeButton;

    [Header("General settings")]
    [SerializeField] private float menuHideDelay = 5f;

    [SerializeField] private float colorSwitchSpeed = 0.5f;


    private Coroutine menuCoroutine;

    public event Action<ColorScheme, float> OnColorSchemeChange;

    private IEnumerator Start()
    {
        soundButton.onClick.AddListener(() => OnSoundButtonClick());

        sfxButton.onClick.AddListener(() => OnSfxButtonClick());

        iapButton.onClick.AddListener(OnIapButtonClick);

        leaderboardButton.onClick.AddListener(ShowLeaderboard);

        achievementsButton.onClick.AddListener(ShowAchievements);

        colorSchemeChangeButton.onClick.AddListener(() => ChangeColorScheme());

        menuButton.onClick.AddListener(ShowMenuPanel);

        while (!GameplayController.Instance.SpawnComplete)
        {
            yield return null;
        }

        musicImage.sprite = DataManager.Instance.Settings.sound == true ? musicActiveSprite : musicInactiveSprite;
        sfxImage.sprite = DataManager.Instance.Settings.sfx == true ? sfxActiveSprite : sfxInactiveSprite;

        colorSchemeImage.sprite = DataManager.Instance.ColorScheme == ColorScheme.LIGHT ? colorSchemeLightSprite : colorSchemeDarkSprite;
        SoundController.Instance.PlayMusic();
    }

    /// <summary>
    /// Callback on color scheme changed
    /// </summary>
    private void ChangeColorScheme()
    {
        ColorScheme currentScheme = DataManager.Instance.ColorScheme;

        ColorScheme newColorScheme = currentScheme == ColorScheme.LIGHT ? ColorScheme.DARK : ColorScheme.LIGHT;

        OnColorSchemeChange?.Invoke(newColorScheme, colorSwitchSpeed);

        DataManager.Instance.currentData.colorScheme = newColorScheme;

        colorSchemeImage.sprite = newColorScheme == ColorScheme.LIGHT ? colorSchemeLightSprite : colorSchemeDarkSprite;
        CancelMenuHide();
    }

    /// <summary>
    /// Callback to show bottom menu
    /// </summary>
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

    /// <summary>
    /// Coroutine to hide bottom menu after some delay
    /// </summary>
    /// <returns></returns>
    private IEnumerator MenuHideCoroutine()
    {
        yield return new WaitForSeconds(menuHideDelay);
        menu.SetActive(false);
        menuCoroutine = null;
        menuButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Callback to cancel hide bottom menu
    /// </summary>
    private void CancelMenuHide()
    {
        if (menuCoroutine != null)
        {
            StopCoroutine(menuCoroutine);
        }
        menuCoroutine = StartCoroutine(MenuHideCoroutine());
    }

    /// <summary>
    /// Callback to show achievements screen
    /// Also cancel menu hide on click
    /// </summary>
    private void ShowAchievements()
    {
        Social.ShowAchievementsUI();
        CancelMenuHide();
    }

    /// <summary>
    /// Callback to show leaderboard screen
    /// Also cancel menu hide on click
    /// </summary>
    private void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
        CancelMenuHide();
    }

    /// <summary>
    /// Callback for IAP purchases
    /// Also cancel menu hide on click
    /// </summary>
    private void OnIapButtonClick()
    {
        CancelMenuHide();
        //to do
    }

    /// <summary>
    /// Callback for change and save sfx setting
    /// Also cancel menu hide on click
    /// </summary>
    /// <param name="save"></param>
    private void OnSfxButtonClick()
    {
        if (DataManager.Instance.Settings.sfx == true)
        {
            DataManager.Instance.Settings.sfx = false;
            sfxImage.sprite = sfxInactiveSprite;
        }
        else
        {
            DataManager.Instance.Settings.sfx = true;
            sfxImage.sprite = sfxActiveSprite;
        }
        CancelMenuHide();
    }

    /// <summary>
    /// Callback for change and save sound setting
    /// Also cancel menu hide on click
    /// </summary>
    /// <param name="save"></param>
    private void OnSoundButtonClick()
    {
        if (DataManager.Instance.Settings.sound == true)
        {
            DataManager.Instance.Settings.sound = false;
            musicImage.sprite = musicInactiveSprite;
        }
        else
        {
            DataManager.Instance.Settings.sound = true;
            musicImage.sprite = musicActiveSprite;
        }
        SoundController.Instance.PlayMusic();
        CancelMenuHide();
    }
}

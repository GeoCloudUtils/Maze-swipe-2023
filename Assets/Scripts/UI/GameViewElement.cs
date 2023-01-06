using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameViewElement : MonoBehaviour, IGameViewElement
{
    public Image image;

    public bool isElementActive = true;

    [SerializeField] private Color darkSchemeColor;
    [SerializeField] private Color lightSchemeColor;

    private void Start()
    {
        GameViewController.Instance.OnColorSchemeChange += ChangeColor;
        ChangeColor(DataManager.Instance.ColorScheme, 0f);
    }

    public void ChangeColor(ColorScheme colorScheme, float transitionSpeed = 0f)
    {
        if (image != null && isElementActive)
        {
            Color newColor = colorScheme == ColorScheme.LIGHT ? lightSchemeColor : darkSchemeColor;
            image.DOKill();
            image.DOColor(newColor, transitionSpeed);
        }
    }
}

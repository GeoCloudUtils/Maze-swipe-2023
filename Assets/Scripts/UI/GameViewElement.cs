using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameViewElement : MonoBehaviour, IGameViewElement
{
    [SerializeField] private Image image;

    [SerializeField] private Color darkSchemeColor;
    [SerializeField] private Color lightSchemeColor;

    public void Activate(ColorScheme colorScheme)
    {
        ChangeColor(colorScheme);
    }

    public void ChangeColor(ColorScheme colorScheme, float transitionSpeed = 0f)
    {
        if (image != null)
        {
            Color newColor = colorScheme == ColorScheme.LIGHT ? lightSchemeColor : darkSchemeColor;
            image.DOKill();
            image.DOColor(newColor, transitionSpeed);
        }
    }
}

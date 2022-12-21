using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Test : MonoBehaviour
{
    public Transform _panel;
    public Cell cell;
    public InputManager InputManager;

    private bool _isRotating = false;
    private void Start()
    {
        InputManager.OnSwipe += OnSwipe;
        cell.GetTopBorder();
    }

    private void OnSwipe(InputManager.Direction swipeDirection)
    {
        if (_isRotating)
        {
            return;
        }
        switch (swipeDirection)
        {
            case InputManager.Direction.Left:
                RotatePanel(90f);
                break;
            case InputManager.Direction.Right:
                RotatePanel(-90f);
                break;
            default:
                break;
        }
    }

    private void RotatePanel(float angle)
    {
        _isRotating = true;
        Vector3 currentRotation = _panel.eulerAngles;
        Vector3 newRotation = new Vector3(currentRotation.x, currentRotation.y, Mathf.RoundToInt(currentRotation.z + angle));
        _panel.DORotate(newRotation, .5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            _isRotating = false;
            cell.GetTopBorder();
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Transform _grid;
    public InputManager InputManager;

    public Cell[] Cells;

    private bool _isRotating = false;
    private IEnumerator Start()
    {
        InputManager.OnSwipe += OnSwipe;
        InitClonePoints();
        yield return new WaitForEndOfFrame();
        foreach (Cell cell in Cells)
        {
            cell.m_Started = true;
        }
    }

    private void InitClonePoints()
    {
        Transform pointsGrid = Instantiate(_grid, _grid.parent);
        pointsGrid.name = "Points";
        pointsGrid.GetComponent<Image>().enabled = false;
        for (int i = 0; i < pointsGrid.childCount; i++)
        {
            GameObject child = pointsGrid.transform.GetChild(i).gameObject;
            Destroy(child.GetComponent<Cell>());
            Destroy(child.transform.Find("txt").gameObject);
            child.GetComponent<Image>().enabled = false;
            BoxCollider bx = child.GetComponent<BoxCollider>();
            bx.size = new Vector2(bx.size.x / 2, bx.size.y / 2);
            int x = int.Parse(child.name.ToString().Split('_')[0]);
            int y = int.Parse(child.name.ToString().Split('_')[1]);
            int layer = LayerMask.NameToLayer("Point");
            child.layer = layer;
            child.AddComponent<Point>().Init(x, y);
        }
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
                RotatePanel(-90f);
                break;
            case InputManager.Direction.Right:
                RotatePanel(90f);
                break;
            default:
                break;
        }
    }

    private void RotatePanel(float angle)
    {
        _isRotating = true;
        Vector3 currentRotation = _grid.eulerAngles;
        Vector3 newRotation = new Vector3(currentRotation.x, currentRotation.y, Mathf.RoundToInt(currentRotation.z + angle));
        _grid.DORotate(newRotation, .5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            _isRotating = false;
        });
    }
}


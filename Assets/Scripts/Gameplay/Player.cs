using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Player : MonoBehaviour
{
    public Cell CurrentCell;

    public Transform cat;

    public RectTransform Rect;
    public Vector2Int Position => CurrentCell.Position;

    public List<Cell> gridCells = new List<Cell>();

    public event Action<bool> MoveComplete;

    private bool captureEvents = true;
    public void Init()
    {
        StartCoroutine(MovePlayer());
    }

    private IEnumerator MovePlayer()
    {
        if (!captureEvents)
        {
            yield break;
        }
        bool canMove = true;
        while (canMove)
        {
            Cell playerCell = CurrentCell;
            Cell belowCell = GetBellow(playerCell);
            if (belowCell == null || belowCell.IsEnabled)
            {
                canMove = false;
            }
            else
            {
                transform.SetParent(belowCell.transform);
                CurrentCell = belowCell;
                yield return transform.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.Linear).WaitForCompletion();
                if (CurrentCell.IsEnd)
                {
                    canMove = false;
                }
                if (CurrentCell.HasCollectable)
                {
                    if (CurrentCell.transform.childCount > 0)
                    {
                        GameObject cellChild = CurrentCell.transform.GetChild(0).gameObject;
                        if (cellChild != gameObject)
                        {
                            Destroy(cellChild);
                        }
                        CurrentCell.HasCollectable = false;
                    }
                }
            }
        }
        yield return new WaitForEndOfFrame();
        MoveComplete?.Invoke(CurrentCell.IsEnd);
        if (CurrentCell.IsEnd)
        {
            captureEvents = false;
            StopAllCoroutines();
        }
    }

    private Cell GetBellow(Cell targetCell)
    {
        Vector2Int targetPosition = targetCell.Position;
        Vector2Int nextPosition = new Vector2Int(targetPosition.x + 1, targetPosition.y);
        Cell cell = gridCells.Find(e => e.Position == nextPosition);
        return cell;
    }

    private void Update()
    {
        cat.transform.rotation = Quaternion.Euler(0.0f, 0.0f, gameObject.transform.parent.rotation.z * -1.0f);
    }
}

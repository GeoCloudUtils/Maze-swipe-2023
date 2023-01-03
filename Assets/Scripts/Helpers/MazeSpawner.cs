using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class MazeSpawner : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private Transform collectablePrefab;

    [SerializeField] private Transform target;

    [SerializeField] private Cell cellPrefab;

    [SerializeField] private Transform spawnContainer;

    [SerializeField] private List<Cell> gridCells = new List<Cell>();

    public event Action<Player, Cell[]> SpawnComplete;

    private Transform pointsGrid;

    private Data mazeData;

    public void Init(Data mazeData)
    {
        Clear();
        this.mazeData = mazeData;
        StartCoroutine(CreateMaze());
    }

    private void Clear()
    {
        foreach (var cell in gridCells)
        {
            Destroy(cell.gameObject);
        }
        for (int i = 0; i < spawnContainer.transform.childCount; i++)
        {
            GameObject child = spawnContainer.transform.GetChild(i).gameObject;
            Destroy(child);
        }
        gridCells = new List<Cell>();
        spawnContainer.eulerAngles = Vector3.zero;
        if (pointsGrid != null)
        {
            Destroy(pointsGrid.gameObject);
        }
    }
    private IEnumerator CreateMaze()
    {
        int numCells = mazeData.levelData.Length;
        int gridSize = mazeData.gridLength;
        Player playerInstance = null;
        for (int i = 0; i < numCells; i++)
        {
            Cell cell = Instantiate(cellPrefab, spawnContainer);
            int cellData = int.Parse(mazeData.levelData[i].ToString());
            cell.Init(cellData == 1, cellData == 2, cellData == 3, cellData == 4);
            if (cellData == 2)
            {
                playerInstance = Instantiate(player, cell.transform);
                playerInstance.CurrentCell = cell;
                playerInstance.transform.localPosition = Vector3.zero;
                playerInstance.transform.DOScale(1f, 0.5f).From(0f).SetDelay(0.5f).SetEase(Ease.OutExpo);
            }
            else if (cellData == 3)
            {
                RectTransform targetInstance = Instantiate(target, cell.transform) as RectTransform;
                targetInstance.localPosition = Vector3.zero;
                targetInstance.DOScale(1f, 0.5f).From(0f).SetDelay(0.5f).SetEase(Ease.OutExpo);
            }
            else if (cellData == 4)
            {
                RectTransform collectable = Instantiate(collectablePrefab, cell.transform) as RectTransform;
                collectable.localPosition = Vector3.zero;
                collectable.DOScale(1f, 0.5f).From(0f).SetDelay(0.55f).SetEase(Ease.OutExpo);
            }
            gridCells.Add(cell);
            cell.Rect.DOScale(Vector3.one, 0.5f).SetDelay(i * 0.01f).SetEase(Ease.OutExpo).From(Vector3.zero);
        }

        List<string> cellNames = new List<string>();
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                cellNames.Add($"{i}_{j}");
            }
        }

        for (int i = 0; i < gridCells.Count; i++)
        {
            Cell cell = gridCells[i];
            int x = int.Parse(cellNames[i].Split('_')[0]);
            int y = int.Parse(cellNames[i].Split('_')[1]);
            cell.Position = new Vector2Int(x, y);
            cell.name = cellNames[i];
        }

        yield return new WaitForEndOfFrame();

        SaveGridPoints();

        yield return new WaitForEndOfFrame();

        foreach (Cell cell in gridCells)
        {
            cell.enableCheck = true;
        }
        playerInstance.gridCells = gridCells;
        SpawnComplete?.Invoke(playerInstance, gridCells.ToArray());
    }

    private void SaveGridPoints()
    {
        pointsGrid = Instantiate(spawnContainer, spawnContainer.parent);

        pointsGrid.name = "Points";

        pointsGrid.GetComponent<Image>().enabled = false;

        for (int i = 0; i < pointsGrid.childCount; i++)
        {
            Cell child = pointsGrid.transform.GetChild(i).GetComponent<Cell>();
            child.transform.localScale = Vector3.one;
            child.Image.enabled = false;
            if (child.transform.childCount > 0)
            {
                if (child.transform.GetChild(0) != null)
                {
                    Destroy(child.transform.GetChild(0).gameObject);
                }
            }

            int layer = LayerMask.NameToLayer("Point");
            child.gameObject.layer = layer;
            child.gameObject.AddComponent<CellPoint>().Init(child.Position.x, child.Position.y);

            Destroy(child);
        }
    }
}


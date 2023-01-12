using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Player logic
/// </summary>
public class Player : MonoBehaviour
{
    public Cell currentCell;

    public RectTransform playerIconTransform;
    public Vector2Int PlayerPosition => currentCell.Position;

    public List<Cell> gridCells = new List<Cell>();

    public bool captureInputEvents = true;
    public bool isMoving = false;
    public bool isOnEndCell = false;

    [SerializeField] private float elapsedMoveTime = 0f;
    [SerializeField] private float moveDuration = 0.15f;

    private Vector3 startMovePosition;
    private Vector3 endMovePosition;

    private void Awake()
    {
        captureInputEvents = false;
    }

    private void Update()
    {
        playerIconTransform.transform.rotation = Quaternion.Euler(0.0f, 0.0f, gameObject.transform.parent.rotation.z * -1.0f);

        if (!captureInputEvents)
        {
            return;
        }
        Cell bellowCell = GetBellow(currentCell);
        if (bellowCell == null || bellowCell.isElementActive) { return; }

        if (isMoving)
        {
            elapsedMoveTime += Time.deltaTime;
            if (elapsedMoveTime >= moveDuration)
            {
                transform.localPosition = endMovePosition;
                isMoving = false;
                currentCell = bellowCell;
            }
            else
            {
                transform.localPosition = Vector3.Lerp(startMovePosition, endMovePosition, elapsedMoveTime / moveDuration);
            }
        }
        else
        {
            transform.SetParent(bellowCell.transform);
            startMovePosition = transform.localPosition;
            endMovePosition = Vector3.zero;
            elapsedMoveTime = 0f;
            isMoving = true;
        }

        if (currentCell.IsEnd)
        {
            captureInputEvents = false;
            isOnEndCell = true;
            transform.localPosition = endMovePosition;
        }
        else if (currentCell.HasCollectable)
        {
            if (currentCell.transform.childCount > 0)
            {
                GameObject cellChild = currentCell.transform.GetChild(0).gameObject;
                if (cellChild != gameObject)
                {
                    Destroy(cellChild);
                }
                currentCell.HasCollectable = false;
            }
        }
    }

    /// <summary>
    /// Get's bellow cell
    /// </summary>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    private Cell GetBellow(Cell targetCell)
    {
        Vector2Int targetPosition = targetCell.Position;
        Vector2Int nextPosition = new Vector2Int(targetPosition.x + 1, targetPosition.y);
        Cell cell = gridCells.Find(e => e.Position == nextPosition);
        return cell;
    }
}

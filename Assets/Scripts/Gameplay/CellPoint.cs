using UnityEngine;

public class CellPoint : MonoBehaviour
{
    /// The position of the CellPoint on the grid
    public Vector2Int Position;

    /// Initializes the CellPoint with the specified coordinates
    public void Init(int x, int y)
    {
        /// Set the position to a new Vector2Int with the provided coordinates
        Position = new Vector2Int(x, y);
    }
}

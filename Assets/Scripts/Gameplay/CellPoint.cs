using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPoint : MonoBehaviour
{
    public Vector2Int Position;

    public void Init(int x, int y)
    {
        Position = new Vector2Int(x, y);
    }
}

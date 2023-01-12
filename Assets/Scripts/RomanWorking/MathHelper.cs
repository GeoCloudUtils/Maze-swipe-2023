using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MathHelper
{
    public static List<Vector2Int> TransformLinesToPoints(List<Vector2Int> path)
    {
        List<Vector2Int> linePoints = new List<Vector2Int>();

        if (path.Count == 2)
        {
            Debug.Log($"Line. Size:{2}; Points:({path[0].y},{path[0].x})->({path[1].y},{path[1].x})");
            linePoints.Add(path[0]);
            linePoints.Add(path[1]);
            return linePoints;
        }

        Vector2Int start = path[0];
        Vector2Int curr;
        int size;
        for (int a = 1; a < path.Count; ++a)
        {
            curr = path[a];
            size = 1;
            bool horizontal = IsHorizontalLine(start, curr);
            bool vertical = !horizontal;

            //Debug.Log($"Line started. Horiz:{horizontal}; Vert:{vertical}; Curr:({curr.y},{curr.x}); Start: ({start.y},{start.x})");

            if (horizontal)
            {
                while (IsHorizontalLine(curr, start))
                {
                    ++size;
                    ++a;
                    if (a >= path.Count)
                        break;
                    curr = path[a];
                }

                var end = path[Mathf.Min(path.Count - 1, a - 1)];
                Debug.Log($"Line Horiz. Size:{size}; Points:({start.y},{start.x})->({end.y},{end.x})");

                linePoints.Add(start);
                linePoints.Add(end);

                a -= 1;
                start = end;//30672446
            }

            if (vertical)
            {
                while (IsVerticalLine(curr, start))
                {
                    ++size;
                    ++a;
                    if (a >= path.Count)
                        break;
                    curr = path[a];
                }

                var end = path[Mathf.Min(path.Count - 1, a - 1)];
                Debug.Log($"Line Vert. Size:{size}; Points:({start.y},{start.x})->({end.y},{end.x})");

                linePoints.Add(start);
                linePoints.Add(end);

                a -= 1;
                start = end;
            }

           // start = curr;
        }

        return linePoints;
    }

    private static bool IsHorizontalLine(Vector2Int p1, Vector2Int p2)
    {
        return p2.x != p1.x && p2.y == p1.y;
    }

    private static bool IsVerticalLine(Vector2Int p1, Vector2Int p2)
    {
        return p2.x == p1.x && p2.y != p1.y;
    }

}

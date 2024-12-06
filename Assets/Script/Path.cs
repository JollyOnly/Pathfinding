using UnityEngine;

public class Path
{
    public readonly Vector3[] lookPointsArray;
    public readonly Line[] turnBoundariesArray;
    public readonly int finishLineIndex;
    public readonly int slowDownIndex;

    public Path(Vector3[] waypointsArray, Vector3 startPos, float turnDst, float stoppingDistance)
    {
        lookPointsArray = waypointsArray;
        turnBoundariesArray = new Line[lookPointsArray.Length];
        finishLineIndex = turnBoundariesArray.Length - 1;

        Vector2 previousPoint = V3ToV2(startPos);

        for (int i = 0; i < lookPointsArray.Length; i++)
        {
            Vector2 currentPoint = V3ToV2(lookPointsArray[i]);
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundariesPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;
            turnBoundariesArray[i] = new Line(turnBoundariesPoint, previousPoint - dirToCurrentPoint * turnDst);
            previousPoint = turnBoundariesPoint;
        }

        float dstFromEndPoint = 0;

        for (int i = lookPointsArray.Length - 1; i > 0; i--)
        {
            dstFromEndPoint += Vector3.Distance(lookPointsArray[i], lookPointsArray[i - 1]);
            if (dstFromEndPoint > stoppingDistance)
            {
                slowDownIndex = i;
                break;
            }
        }
    }

    private Vector2 V3ToV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Vector3 p in lookPointsArray)
        {
            Gizmos.DrawCube(p + Vector3.up, Vector3.one);
        }

        Gizmos.color = Color.white;
        foreach (Line l in turnBoundariesArray)
        {
            l.DrawWithGizmos(10);
        }
    }
}

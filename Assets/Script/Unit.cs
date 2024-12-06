using System.Collections;

using UnityEngine;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;

    [SerializeField] private Transform target;
    [SerializeField] private float speed = 20;
    [SerializeField] private float turnDst = 5;
    [SerializeField] private float stoppingDst = 10;

    private Path path;

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }

    private void OnPathFound(Vector3[] waypoints, bool pathSuccessfull)
    {
        if (pathSuccessfull)
        {
            path = new Path(waypoints, transform.position, turnDst, stoppingDst);
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }

    private IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }

        PathRequestManager.RequestPath(new PathRequest (transform.position, target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
            }
        }
    }
    private IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

            while (pathIndex < path.turnBoundariesArray.Length && path.turnBoundariesArray[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex >= path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundariesArray[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    if (speedPercent < .01f)
                        followingPath = false;
                }
                pathIndex = Mathf.Min(pathIndex, path.lookPointsArray.Length - 1);
                Vector3 waypoint = path.lookPointsArray[pathIndex] - transform.position;
                transform.Translate(waypoint.normalized * Time.deltaTime * speed * speedPercent, Space.Self);

                yield return null;
            }
        }
    }
}

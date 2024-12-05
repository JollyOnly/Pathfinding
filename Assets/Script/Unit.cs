using System.Collections;

using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 20;
    private Vector3[] path;
    private int targetIndex;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    private void OnPathFound(Vector3[] newPath, bool pathSuccessfull)
    {
        if (pathSuccessfull)
        {
            path = newPath;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;

        }
    }
}

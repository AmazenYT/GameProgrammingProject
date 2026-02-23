using UnityEngine;

public class PathScript : MonoBehaviour
{

    public enum PathType
    {
        Loop,
        ReverseWhenComplete
    }

    public Transform[] wayPoints;
    public PathType pathType = PathType.Loop;

    private int direction = 1;
    int index;

    public Vector3 GetCurrentWayPoint()
    {
        return wayPoints[index].position;
    }

    public Vector3 GetNextWayPoint()
    {
        if (wayPoints.Length == 0) return transform.position;
        index = GetNextWayPointIndex();
        Vector3 nextWayPoint = wayPoints[index].position;
        return nextWayPoint;
    }

    private int GetNextWayPointIndex()
    {
        index += direction;

        if (pathType == PathType.Loop)
        {
            index %= wayPoints.Length;
        }
        else if (pathType == PathType.ReverseWhenComplete)
        {
            if (index >= wayPoints.Length || index < 0)
            {
                direction *= -1;
                index += direction * 2;
            }
            
        }
        return index;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

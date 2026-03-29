using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class GridPathFinder
{
    private static readonly float _lineCutStep = 0.5f;
    private static readonly float _raycastHeight = 0.4f;

    public static bool CalculatePath(ref PathData pathData, Vector3 agentPoisition, Vector3 targetPosition)
    {
        pathData = new PathData();
        NavMeshPath path = new NavMeshPath();

        if (NavMesh.CalculatePath(agentPoisition, targetPosition, NavMesh.AllAreas, path))
        {
            pathData.Points = new List<Vector3>();
            pathData.Points.AddRange(path.corners);

            pathData.Points = SetPathDetails(pathData.Points);
            pathData.Points = SmoothPath(pathData.Points);

            pathData.Distance = 0;

            for (int i = 0; i < pathData.Points.Count - 1; i++)
            {
                pathData.Distance += Vector3.Distance(pathData.Points[i], pathData.Points[i + 1]);
            }

            return true;
        }

        pathData = null;
        return false;
    }

    private static List<Vector3> SetPathDetails(List<Vector3> path)
    {
        List<Vector3> newPath = new List<Vector3>();

        newPath.Add(SetPointHeight(path[0]));

        for (int i = 0; i < path.Count - 1; i++)
        {
            if (Vector3.Distance(path[i], path[i + 1]) <= _lineCutStep)
            {
                newPath.Add(SetPointHeight(path[i + 1]));
                continue;
            }

            int newDotsCount = Mathf.FloorToInt(Vector3.Distance(path[i], path[i + 1]) / _lineCutStep);

            while (Vector3.Distance(newPath[newPath.Count - 1], path[i + 1]) >= _lineCutStep)
            {
                Vector3 p1 = newPath[newPath.Count - 1];
                Vector3 p2 = path[i + 1];
                float t = _lineCutStep / Vector3.Distance(p1, p2);
                Vector3 newDot = p1 + (p2 - p1) * t;
                newPath.Add(SetPointHeight(newDot));
            }

            newPath.Add(SetPointHeight(path[i + 1]));
        }

        return newPath;
    }

    private static List<Vector3> SmoothPath(List<Vector3> path)
    {
        List<Vector3> smoothed = new List<Vector3>();
        smoothed.Add(path[0]);

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector3 prev = path[i - 1];
            Vector3 current = path[i];
            Vector3 next = path[i + 1];

            Vector3 smooth = (prev + current + next) / 3f;
            smoothed.Add(smooth);
        }

        smoothed.Add(path[path.Count - 1]);
        return smoothed;
    }

    private static Vector3 SetPointHeight(Vector3 point)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(point, out hit, _raycastHeight, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return point;
    }
}

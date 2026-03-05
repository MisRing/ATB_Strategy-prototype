using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public static class GridPathFinder
{
    public static bool CalculatePath(ref PathData pathData, Vector3 fromPos, Vector3 toPos, GridMap gridMap)
    {
        NavMeshPath path = new NavMeshPath();

        bool found = NavMesh.CalculatePath(
            fromPos,
            toPos,
            NavMesh.AllAreas,
            path);

        if (!found || path.status != NavMeshPathStatus.PathComplete)
            return false;

        pathData.Cover = TileCover.None;
        GridTile tile = gridMap.GetTileByWorldPosition(toPos.x, toPos.z);
        for (int i = 0; i < 4; i++)
        {
            if (tile.Covers[i] == TileCover.Full)
            {
                pathData.finalDirection = GridMapExtansion._directions[i];
                pathData.Cover = TileCover.Full;
                break;
            }
            if (tile.Covers[i] == TileCover.Low && pathData.Cover == TileCover.None)
            {
                pathData.finalDirection = GridMapExtansion._directions[i];
                pathData.Cover = TileCover.Low;
            }
        }

        pathData.Points = path.corners.ToList();

        pathData.Distance = 0;

        for(int i = 0; i < pathData.Points.Count - 1; i++)
        {
            pathData.Distance += Vector3.Distance(pathData.Points[i], pathData.Points[i + 1]);
        }

        return true;
    }
}

public struct PathData
{
    public List<Vector3> Points;
    public float Distance;
    public bool IsReacheble;
    public TileCover Cover;
    public Vector3 finalDirection;
}

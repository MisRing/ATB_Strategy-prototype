using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public static class GridPathFinder
{
    public static bool CalculatePath(ref PathData pathData, Vector3 fromPos, Vector3 toPos)
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
        GridTile tile = new GridTile();
        GridParameters.LevelGrid.GetTileByWorldPos(ref tile, toPos);
        for (int i = 0; i < 4; i++)
        {
            if (tile.Covers[i] == TileCover.Full)
            {
                pathData.finalDirection = GridParameters.COVER_DIRECTIONS[i];
                pathData.Cover = TileCover.Full;
                break;
            }
            if (tile.Covers[i] == TileCover.Low && pathData.Cover == TileCover.None)
            {
                pathData.finalDirection = GridParameters.COVER_DIRECTIONS[i];
                pathData.Cover = TileCover.Low;
            }
        }

        pathData.Points = path.corners.ToList();

        float lastDist = Vector3.Distance(pathData.Points[pathData.Points.Count - 2], pathData.Points[pathData.Points.Count - 1]);
        if (lastDist >= 3f)
        {
            Vector3 additionalPoint;
            float additionalPointDistPercent = (lastDist - 3) / lastDist;
            additionalPoint = (pathData.Points[pathData.Points.Count - 1] - pathData.Points[pathData.Points.Count - 2])
                * additionalPointDistPercent + pathData.Points[pathData.Points.Count - 2];

            pathData.Points.Insert(pathData.Points.Count - 1, additionalPoint);
        }

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

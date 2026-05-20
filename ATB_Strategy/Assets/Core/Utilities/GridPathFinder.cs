using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public static class GridPathFinder
{
    private const float LINE_CUT_STEP = 0.5f;
    private const float RAYCAST_HEIGHT = 0.4f;
    private const float MAX_COVER_ANGLE = 90f;

    public static TileCover GetTileCover(ref Vector3 direction, out int coverLook, GridTile tile, List<CombatObject> targets)
    {
        if (targets != null && targets.Count > 0)
        {
            Vector3 tilePos = tile.WorldPosition;
            
            CombatObject closestTarget = targets[0];
            float closestTargetDistance = Vector3.Distance(tilePos, closestTarget.Position);
            for (int i = 1; i < targets.Count; i++)
            {
                float distance = Vector3.Distance(tilePos, targets[i].Position);
                if (distance < closestTargetDistance)
                {
                    closestTargetDistance = distance;
                    closestTarget = targets[i];
                }
            }
            
            Vector3 directionToTarget = closestTarget.Position - tilePos;
            directionToTarget = new Vector3(directionToTarget.x, 0f, directionToTarget.z).normalized;

            int closestCoverID = -1;
            float closestCoverAngle = float.MaxValue;
            for (int i = 0; i < tile.Covers.Length; i++)
            {
                if(tile.Covers[i] == 0) continue;
                
                float angle = Vector3.Angle(directionToTarget, GridParameters.COVER_DIRECTIONS[i]);
                
                if (angle >= MAX_COVER_ANGLE) continue;

                if (angle < closestCoverAngle)
                {
                    closestCoverAngle = angle;
                    closestCoverID = i;
                }
            }

            if (closestCoverID == -1)
            {
                direction = directionToTarget; // MAYBE NOT NEEDED
                coverLook = 0;

                return TileCover.None;
            }
            
            float cross = Vector3.Cross(GridParameters.COVER_DIRECTIONS[closestCoverID], directionToTarget).y;
            
            coverLook = cross > 0 ? +1 : -1;
            direction = -GridParameters.COVER_DIRECTIONS[closestCoverID];
            return tile.Covers[closestCoverID];
        }

        List<int> bestCovers = GetBestCoversID(tile.Covers);
        if (bestCovers.Count > 0)
        {
            int closestCover = -1;
            float closestCoverAngle = float.MaxValue;

            for (int i = 0; i < bestCovers.Count; i++)
            {
                float angle = Vector3.Angle(direction, GridParameters.COVER_DIRECTIONS[bestCovers[i]]);
                if (angle < closestCoverAngle)
                {
                    closestCover = bestCovers[i];
                    closestCoverAngle = angle;
                }
            }
            int coverLeftID = (closestCover - 1 + tile.Covers.Length) % tile.Covers.Length;
            int coverRightID = (closestCover + 1 + tile.Covers.Length) % tile.Covers.Length;

            if (tile.Covers[coverLeftID] == tile.Covers[coverRightID])
            {
                float angleLeft = Vector3.Angle(direction, GridParameters.COVER_DIRECTIONS[coverLeftID]);
                float angleRight = Vector3.Angle(direction, GridParameters.COVER_DIRECTIONS[coverRightID]);

                coverLook = angleLeft < angleRight ? -1 : 1;
            }
            else
            {
                coverLook = tile.Covers[coverLeftID] < tile.Covers[coverRightID] ? -1 : 1;
            }

            direction = -GridParameters.COVER_DIRECTIONS[closestCover];
            return tile.Covers[closestCover];
        }

        coverLook = 0;
        return TileCover.None;
    }

    private static List<int> GetBestCoversID(TileCover[] covers)
    {
        List<int> bestCovers = new List<int>();
        for(int i = 0; i < covers.Length; i++)
        {
            if (covers[i] > 0)
            {
                if(bestCovers.Count == 0 || covers[i] > covers[bestCovers[0]])
                {
                    bestCovers.Clear();
                    bestCovers.Add(i);
                    continue;
                }

                if(covers[i] == covers[bestCovers[0]])
                {
                    bestCovers.Add(i);
                }
            }
        }
        return bestCovers;
    }

    public static bool CalculatePath(out PathData pathData, Vector3 agentPosition, Vector3 targetPosition)
    {
        pathData = new PathData();
        NavMeshPath path = new NavMeshPath();

        if (NavMesh.CalculatePath(agentPosition, targetPosition, NavMesh.AllAreas, path))
        {
            pathData.Points = new List<Vector3>();
            pathData.Points.AddRange(path.corners);

            pathData.Points = SetPathDetails(pathData.Points);
            pathData.Points = SmoothPath(pathData.Points);

            pathData.Distance = 0;

            int pointsCount = pathData.Points.Count;

            for (int i = 0; i < pointsCount - 1; i++)
            {
                pathData.Distance += Vector3.Distance(pathData.Points[i], pathData.Points[i + 1]);
            }

            pathData.FinalDirection = (pathData.Points[pointsCount - 1] - pathData.Points[pointsCount - 2]).normalized;

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
            if (Vector3.Distance(path[i], path[i + 1]) <= LINE_CUT_STEP)
            {
                newPath.Add(SetPointHeight(path[i + 1]));
                continue;
            }

            int newDotsCount = Mathf.FloorToInt(Vector3.Distance(path[i], path[i + 1]) / LINE_CUT_STEP);

            while (Vector3.Distance(newPath[^1], path[i + 1]) >= LINE_CUT_STEP)
            {
                Vector3 p1 = newPath[^1];
                Vector3 p2 = path[i + 1];
                float t = LINE_CUT_STEP / Vector3.Distance(p1, p2);
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

        smoothed.Add(path.Last());
        return smoothed;
    }

    private static Vector3 SetPointHeight(Vector3 point)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(point, out hit, RAYCAST_HEIGHT, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return point;
    }
}

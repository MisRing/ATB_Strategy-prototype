using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class GridPathFinder
{
    private static readonly float _lineCutStep = 0.5f;
    private static readonly float _raycastHeight = 0.4f;

    public static TileCover GetTileCover(ref Vector3 direction, ref int coverLook, GridTile tile, float visionRange, UnitOwner ally)
    {
        coverLook = 0;
        // ❌ нет укрытия вообще
        if (tile.Covers[0] == 0 && tile.Covers[1] == 0 && tile.Covers[2] == 0 && tile.Covers[3] == 0)
            return TileCover.None;

        Vector3 position = GridParameters.LevelGrid.GetTileWorldPos(tile);

        ICombat enemy = CombatService.GetNearestCombat(position, visionRange, ally);

        int bestIndex = -1;
        TileCover bestCoverValue = 0;
        float bestDot = -1f;

        // 🔥 1. Пытаемся выбрать относительно врага
        if (enemy != null)
        {
            Vector3 toEnemy = (enemy.Position - position).normalized;

            for (int i = 0; i < 4; i++)
            {
                TileCover coverValue = tile.Covers[i];

                Vector3 coverDir = GridParameters.COVER_DIRECTIONS[i];

                float dot = Vector3.Dot(coverDir, toEnemy);

                // приоритет:
                // 1) ближе к направлению
                // 2) если почти одинаково — большее укрытие
                if (dot > bestDot || (Mathf.Approximately(dot, bestDot) && coverValue > bestCoverValue))
                {
                    bestDot = dot;
                    bestCoverValue = coverValue;
                    bestIndex = i;
                }
            }

            if (bestIndex != -1)
            {
                Vector3 coverDirection = GridParameters.COVER_DIRECTIONS[bestIndex];
                direction = -coverDirection;
                coverLook = GetLookSide(position, coverDirection, enemy.Position);
                
                return bestCoverValue;
            }
        }

        // 🔥 2. Просто лучшее укрытие
        bestIndex = -1;
        bestCoverValue = 0;

        for (int i = 0; i < 4; i++)
        {
            TileCover coverValue = tile.Covers[i];
            if (coverValue > bestCoverValue)
            {
                bestCoverValue = coverValue;
                bestIndex = i;
            }
        }

        if (bestIndex != -1)
        {
            direction = -GridParameters.COVER_DIRECTIONS[bestIndex];
            return bestCoverValue;
        }

        // 🔥 3. fallback — направление движения (direction уже задан)
        return TileCover.None;
    }
    
    private static int GetLookSide(Vector3 position, Vector3 look, Vector3 target)
    {
        Vector3 dirToTarget = (target - position).normalized;

        float cross = Vector3.Cross(look, dirToTarget).y;

        if (Mathf.Abs(cross) < 0.1f)
            return 0;

        return cross > 0 ? 1 : -1;
    }

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

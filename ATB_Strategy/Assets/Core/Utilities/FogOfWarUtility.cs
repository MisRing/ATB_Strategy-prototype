using System;
using System.Collections.Generic;
using UnityEngine;

public static class FogOfWarUtility
{
    public static FogOfWarRenderer Renderer;

    // =====================================================
    // EVENTS
    // =====================================================

    private static bool _needToResetVisibility;

    public static event Action OnResetVisibility;
    public static event Action OnVisibilityChanged;

    // =====================================================
    // DATA
    // =====================================================

    public static readonly List<UnitCombat> UnitsOfVisionReset =
        new();

    private static readonly HashSet<GridTile> _visibleTiles =
        new();

    private static readonly HashSet<GridTile> _exploredTiles =
        new();

    // =====================================================
    // RESET
    // =====================================================

    public static void TriggerVisibilityReset()
    {
        _needToResetVisibility = true;
    }

    public static void ResetVisibility()
    {
        if (!_needToResetVisibility)
            return;

        _needToResetVisibility = false;

        OnResetVisibility?.Invoke();

        // =============================================
        // OLD VISIBLE -> EXPLORED
        // =============================================

        foreach (GridTile tile in _visibleTiles)
        {
            if (tile == null)
                continue;

            tile.Visibility = TileVisibility.Explored;
            _exploredTiles.Add(tile);
        }

        _visibleTiles.Clear();

        // =============================================
        // NEW VISIBLE
        // =============================================

        foreach (UnitCombat combat in UnitsOfVisionReset)
        {
            if (combat == null)
                continue;

            foreach (GridTile tile in combat.VisibleTiles)
            {
                if (tile == null)
                    continue;

                _visibleTiles.Add(tile);
            }
        }

        // =============================================
        // APPLY
        // =============================================

        foreach (GridTile tile in _visibleTiles)
        {
            tile.Visibility = TileVisibility.Visible;
        }

        Renderer.UpdateFog(
            GridParameters.LevelGrid.GetGrid());

        OnVisibilityChanged?.Invoke();
    }

    // =====================================================
    // VISIBILITY
    // =====================================================

    public static List<GridTile> GetVisibleTiles(
        Vector3 worldPosition,
        float range)
    {
        List<GridTile> result = new();

        GridTile origin =
            GridParameters.LevelGrid.GetTileByWorldPos(worldPosition);

        if (origin == null)
            return result;

        int maxDistance = Mathf.CeilToInt(range);

        Queue<GridTile> queue = new();
        Dictionary<GridTile, int> distanceMap = new();

        queue.Enqueue(origin);
        distanceMap[origin] = 0;

        // =============================================
        // BFS
        // =============================================

        while (queue.Count > 0)
        {
            GridTile current = queue.Dequeue();

            int currentDistance = distanceMap[current];

            // =========================================
            // REAL LOS CHECK
            // =========================================

            if (HasLineOfSight(origin, current, range))
            {
                result.Add(current);
            }

            if (currentDistance >= maxDistance)
                continue;

            // =========================================
            // PROPAGATE
            // =========================================

            foreach (Vector3Int dir in GridParameters.TILE_SIDES)
            {
                int nx = current.PositionX + dir.x;
                int nz = current.PositionZ + dir.z;
                int nf = current.Floor + dir.y;
                

                if (!IsInsideGrid(nx, nz, nf))
                    continue;

                GridTile next =
                    GridParameters.LevelGrid.GetTile(nx, nz, nf);

                if (next == null)
                    continue;
                
                if (dir.y != 0)
                {
                    if (!CanPropagateBetween(current, next))
                        continue;

                    if (!next.IsEmpty)
                        continue;
                }

                if (distanceMap.ContainsKey(next))
                    continue;

                // =====================================
                // FLOOR RULES
                // =====================================

                if (!CanPropagateBetween(current, next))
                    continue;

                distanceMap[next] = currentDistance + 1;

                // =====================================
                // STOP AT WALL
                // =====================================

                if (!next.IsEmpty)
                    continue;

                queue.Enqueue(next);
            }
        }

        return result;
    }

    // =====================================================
    // LOS
    // =====================================================

    private static bool HasLineOfSight(
        GridTile from,
        GridTile to,
        float maxRange)
    {
        Vector3 fromPos = from.WorldPosition;
        Vector3 toPos = to.WorldPosition;

        // RANGE
        if ((toPos - fromPos).sqrMagnitude > maxRange * maxRange)
            return false;

        int x0 = from.PositionX;
        int z0 = from.PositionZ;
        int x1 = to.PositionX;
        int z1 = to.PositionZ;

        int dx = Mathf.Abs(x1 - x0);
        int dz = Mathf.Abs(z1 - z0);

        int sx = x0 < x1 ? 1 : -1;
        int sz = z0 < z1 ? 1 : -1;

        int steps = Mathf.Max(dx, dz);

        int err = dx - dz;

        int currentX = x0;
        int currentZ = z0;

        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 0f : (float)i / steps;

            // 🔥 интерполяция "высоты взгляда"
            float floorF = Mathf.Lerp(from.Floor, to.Floor, t);
            int floor = Mathf.RoundToInt(floorF);

            GridTile tile =
                GridParameters.LevelGrid.GetTile(currentX, currentZ, floor);

            if (tile != null && tile != from && tile != to)
            {
                if (!tile.IsEmpty)
                    return false;
            }

            // Bresenham step
            int e2 = err * 2;

            if (e2 > -dz)
            {
                err -= dz;
                currentX += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                currentZ += sz;
            }
        }

        return true;
    }

    // =====================================================
    // PROPAGATION RULES
    // =====================================================

    private static bool CanPropagateBetween(
        GridTile from,
        GridTile to)
    {
        // same floor
        if (from.Floor == to.Floor)
            return true;

        // =============================================
        // FLOOR CHECK
        // =============================================

        GridTile highest =
            from.Floor > to.Floor
                ? from
                : to;

        // blocked by ceiling/floor
        if (highest.IsGround)
            return false;

        return true;
    }

    // =====================================================
    // HELPERS
    // =====================================================

    private static bool IsInsideGrid(
        int x,
        int z,
        int floor)
    {
        return
            x >= 0 &&
            z >= 0 &&
            floor >= 0 &&
            x < GridParameters.LevelGrid.SizeX &&
            z < GridParameters.LevelGrid.SizeZ &&
            floor < GridParameters.LevelGrid.Floors;
    }
}
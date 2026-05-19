using System.Collections.Generic;
using UnityEngine;
using System;

public static class FogOfWarUtility
{
    public static FogOfWarRenderer Renderer;
    
    private static bool _needToResetVisibility;

    public static event Action OnResetVisibility;
    public static event Action OnVisibilityChanged;

    public static readonly List<UnitCombat> UnitsOfVisionReset = new List<UnitCombat>();

    private static readonly HashSet<GridTile> _exploredTiles = new HashSet<GridTile>();
    private static readonly HashSet<GridTile> _visibleTiles = new HashSet<GridTile>();

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

        // =====================================================
        // OLD VISIBLE -> EXPLORED
        // =====================================================

        foreach (GridTile tile in _visibleTiles)
        {
            tile.Visibility = TileVisibility.Explored;
            _exploredTiles.Add(tile);
        }

        _visibleTiles.Clear();

        // =====================================================
        // COLLECT NEW VISIBLE
        // =====================================================

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

        // =====================================================
        // APPLY NEW VISIBLE
        // =====================================================

        foreach (GridTile tile in _visibleTiles)
        {
            tile.Visibility = TileVisibility.Visible;
        }
        
        Renderer.UpdateFog(GridParameters.LevelGrid.GetFloorData(0));
        
        OnVisibilityChanged?.Invoke();
    }
    
    // =========================================================
    // PUBLIC
    // =========================================================

    public static List<GridTile> GetVisibleTiles(Vector3 worldPosition, float range)
    {
        List<GridTile> visibleTiles = new List<GridTile>();

        GridTile origin = GridParameters.LevelGrid.GetTileByWorldPos(worldPosition);

        if (origin == null)
            return visibleTiles;

        int radius = Mathf.CeilToInt(range);
        float sqrRange = range * range;

        // =====================================================
        // SCAN CIRCLE
        // =====================================================

        for (int x = origin.PositionX - radius; x <= origin.PositionX + radius; x++)
        {
            for (int z = origin.PositionZ - radius; z <= origin.PositionZ + radius; z++)
            {
                // =============================================
                // BOUNDS
                // =============================================

                if (!IsInsideGrid(x, z, origin.Floor))
                    continue;

                GridTile target = GridParameters.LevelGrid.GetTile(x, z, origin.Floor);

                if (target == null)
                    continue;

                // =============================================
                // RANGE CHECK
                // =============================================

                Vector3 delta = target.WorldPosition - worldPosition;

                if (delta.sqrMagnitude > sqrRange)
                    continue;

                // =============================================
                // LOS CHECK
                // =============================================

                if (HasLineOfSight(origin, target))
                {
                    visibleTiles.Add(target);
                }
            }
        }

        return visibleTiles;
    }

    // =========================================================
    // LOS
    // =========================================================

    public static bool HasLineOfSight(GridTile from, GridTile to)
    {
        List<GridTile> line = GetLine(from, to);

        // skip first tile (origin)
        for (int i = 1; i < line.Count; i++)
        {
            GridTile tile = line[i];

            if (tile == null)
                return false;

            // =============================================
            // HIT WALL
            // =============================================

            if (!tile.IsEmpty)
            {
                // target wall itself is visible
                return tile == to;
            }
        }

        return true;
    }

    // =========================================================
    // BRESENHAM LINE
    // =========================================================

    public static List<GridTile> GetLine(GridTile from, GridTile to)
    {
        List<GridTile> line = new List<GridTile>();

        int x0 = from.PositionX;
        int z0 = from.PositionZ;

        int x1 = to.PositionX;
        int z1 = to.PositionZ;

        int dx = Mathf.Abs(x1 - x0);
        int dz = Mathf.Abs(z1 - z0);

        int sx = x0 < x1 ? 1 : -1;
        int sz = z0 < z1 ? 1 : -1;

        int err = dx - dz;

        while (true)
        {
            if (IsInsideGrid(x0, z0, from.Floor))
            {
                GridTile tile = GridParameters.LevelGrid.GetTile(x0, z0, from.Floor);

                if (tile != null)
                    line.Add(tile);
            }

            if (x0 == x1 && z0 == z1)
                break;

            int e2 = err * 2;

            if (e2 > -dz)
            {
                err -= dz;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                z0 += sz;
            }
        }

        return line;
    }

    // =========================================================
    // HELPERS
    // =========================================================

    private static bool IsInsideGrid(int x, int z, int floor)
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
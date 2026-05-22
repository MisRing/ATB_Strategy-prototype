using System;
using System.Collections.Generic;
using UnityEngine;

public static class FogOfWarUtility
{
    public static FogOfWarRenderer Renderer;

    private static bool _needToResetVisibility;

    public static event Action OnResetVisibility;
    public static event Action OnVisibilityChanged;

    public static readonly List<UnitCombat> UnitsOfVisionReset = new List<UnitCombat>();

    private static readonly HashSet<GridTile> _visibleTiles = new HashSet<GridTile>();

    private static readonly HashSet<GridTile> _exploredTiles = new HashSet<GridTile>();

    private static readonly List<ForcedVisibility> _forcedVisibility = new List<ForcedVisibility>();
    
    private struct ForcedVisibility
    {
        public GridTile Tile;
        public float Time;
    }

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

        foreach (GridTile tile in _visibleTiles)
        {
            if (tile == null)
                continue;

            tile.Visibility = TileVisibility.Explored;
            _exploredTiles.Add(tile);
        }

        _visibleTiles.Clear();

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

        foreach (GridTile tile in _visibleTiles)
        {
            tile.Visibility = TileVisibility.Visible;
        }
        
        foreach (ForcedVisibility forcedTile in _forcedVisibility)
        {
            forcedTile.Tile.Visibility = TileVisibility.Visible;
        }

        Renderer.UpdateFog(
            GridParameters.LevelGrid.GetGrid());

        OnVisibilityChanged?.Invoke();
    }

    public static void CheckForced()
    {
        for(int i = 0; i < _forcedVisibility.Count; i++)
        {
            ForcedVisibility forcedTile = _forcedVisibility[i];
            forcedTile.Time -= TimeService.TimeSpeedDelta;

            if (forcedTile.Time <= 0)
            {
                _forcedVisibility.RemoveAt(i);
                i--;
                continue;
            }
            
            _forcedVisibility[i] = forcedTile;
        }
    }
    
    public static void ForceVisibility(List<GridTile> tiles, float time)
    {
        foreach (GridTile tile in tiles)
        {
            ForceVisibility(tile, time);
        }
    }

    public static void ForceVisibility(GridTile tile, float time)
    {
        ForcedVisibility forced = new ForcedVisibility()
        {
            Tile = tile,
            Time = time
        };
        _forcedVisibility.Add(forced);
    }

    public static List<GridTile> GetVisibleTiles(
        Vector3 worldPosition,
        float range)
    {
        List<GridTile> result = new List<GridTile>();

        GridTile origin =
            GridParameters.LevelGrid.GetTileByWorldPos(worldPosition);

        if (origin == null)
            return result;

        //int maxDistance = Mathf.CeilToInt(range);
        float sqrRange = range * range;

        Queue<GridTile> queue = new Queue<GridTile>();
        //Dictionary<GridTile, int> distanceMap = new Dictionary<GridTile, int>();
        HashSet<GridTile> visited = new HashSet<GridTile>();
        queue.Enqueue(origin);
        //distanceMap[origin] = 0;
        
        while (queue.Count > 0)
        {
            GridTile current = queue.Dequeue();

            //int currentDistance = distanceMap[current];
            float currentSqrDistance = (origin.WorldPosition - current.WorldPosition).sqrMagnitude;

            if (HasLineOfSight(origin, current, range))
            {
                result.Add(current);
            }

            //if (currentDistance >= maxDistance)
            //    continue;
            if(currentSqrDistance > sqrRange) continue;

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

                //if (distanceMap.ContainsKey(next))
                //    continue;
                if(!visited.Add(next)) continue;

                if (!CanPropagateBetween(current, next))
                    continue;

                //distanceMap[next] = currentDistance + 1;
                
                if (!next.IsEmpty)
                    continue;

                queue.Enqueue(next);
            }
        }

        return result;
    }
    
    private static bool HasLineOfSight(
        GridTile from,
        GridTile to,
        float maxRange)
    {
        Vector3 fromPos = from.WorldPosition;
        Vector3 toPos = to.WorldPosition;
        
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
            
            float floorF = Mathf.Lerp(from.Floor, to.Floor, t);
            int floor = Mathf.RoundToInt(floorF);

            GridTile tile =
                GridParameters.LevelGrid.GetTile(currentX, currentZ, floor);

            if (tile != null && tile != from && tile != to)
            {
                if (!tile.IsEmpty)
                    return false;
            }
            
            int e2 = err * 2;

            if (e2 > -dz)
            {
                err -= dz;
                currentX += sx;
            }

            if (e2 >= dx) continue;
            err += dx;
            currentZ += sz;
        }

        return true;
    }
    
    private static bool CanPropagateBetween(
        GridTile from,
        GridTile to)
    {
        if (from.Floor == to.Floor)
            return true;

        GridTile highest =
            from.Floor > to.Floor
                ? from
                : to;

        return !highest.IsGround;
    }

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
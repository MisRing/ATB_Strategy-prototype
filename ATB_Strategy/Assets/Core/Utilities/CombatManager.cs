using System;
using UnityEngine;
using System.Collections.Generic;

public static class CombatManager
{
    private static readonly List<CombatObject> COMBATS_ON_LEVEL = new List<CombatObject>();
    private static readonly LayerMask ENVIRONMENT_MASK = LayerMask.GetMask("Grid Environment");
    
    private static bool _needToResetVisibility = false;

    public static event Action OnVisibilityChanged;
    
    public static void TriggerVisibilityReset()
    {
        _needToResetVisibility = true;
    }

    public static void ResetVisibility()
    {
        if (!_needToResetVisibility) return;

        _needToResetVisibility = false;
        OnVisibilityChanged?.Invoke();
    }
    
    public static void RegisterCombat(CombatObject comb)
    {
        if (COMBATS_ON_LEVEL.Contains(comb)) return;
        
        COMBATS_ON_LEVEL.Add(comb);
    }
    
    public static void UnregisterCombat(CombatObject comb)
    {
        if (!COMBATS_ON_LEVEL.Contains(comb)) return;
        
        COMBATS_ON_LEVEL.Remove(comb);
    }

    public static List<CombatObject> GetAllCombats(UnitOwner ally)
    {
        List<CombatObject> combats = new List<CombatObject>();
        foreach (CombatObject comb in COMBATS_ON_LEVEL)
        {
            if (comb.Owner == ally) continue;

            combats.Add(comb);
        }
        return combats;
    }


    public static List<CombatObject> GetCombats(CombatObject unitCombat, float range, UnitOwner ally)
    {
        float sqrRange = range * range;
        Vector3 unitPosition = unitCombat.Position;

        List<CombatObject> combatsOnRange = new List<CombatObject>();

        foreach (CombatObject comb in COMBATS_ON_LEVEL)
        {
            float sqrDist = (comb.Position - unitPosition).sqrMagnitude;

            if (sqrDist > sqrRange || comb.Owner == ally) continue;

            //add visibility
            
            combatsOnRange.Add(comb);

        }

        combatsOnRange.Sort((a, b) =>
        {
            float distA = (a.Position - unitPosition).sqrMagnitude;
            float distB = (b.Position - unitPosition).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        return combatsOnRange;
    }
    
    private struct VisibilityNode
    {
        public GridTile Tile;
        public bool PeekUsed;

        public VisibilityNode(GridTile tile, bool peekUsed)
        {
            Tile = tile;
            PeekUsed = peekUsed;
        }
    }

    // =========================================================

    public static List<GridTile> GetVisibleTiles(GridTile startTile, List<GridTile> oldTiles, float range, UnitOwner team)
    {
        foreach (GridTile tile in oldTiles)
        {
            tile.Visibility[(int)team] = TileVisibility.Explored;
        }
        
        List<GridTile> visibleTiles = new List<GridTile>();

        if (startTile == null)
            return visibleTiles;

        HashSet<GridTile> visited = new HashSet<GridTile>();

        Queue<VisibilityNode> queue = new Queue<VisibilityNode>();

        float sqrRange = range * range;
        Vector3 center = startTile.WorldPosition;

        queue.Enqueue(new VisibilityNode(startTile, false));

        while (queue.Count > 0)
        {
            VisibilityNode node = queue.Dequeue();

            GridTile tile = node.Tile;

            if (tile == null)
                continue;

            if (!visited.Add(tile))
                continue;

            float sqrDistance =
                (tile.WorldPosition - center).sqrMagnitude;

            if (sqrDistance > sqrRange)
                continue;

            visibleTiles.Add(tile);
            

            // =========================================
            // STOP AFTER PEEK
            // =========================================

            if (!tile.IsEmpty && node.PeekUsed)
                continue;

            bool nextPeekUsed =
                node.PeekUsed || !tile.IsEmpty;

            // =========================================
            // PROPAGATE
            // =========================================

            int x = tile.PositionX;
            int y = tile.Floor;
            int z = tile.PositionZ;

            for (int i = 0; i < GridParameters.TILE_SIDES.Length; i++)
            {
                Vector3Int dir = GridParameters.TILE_SIDES[i];

                int nx = x + dir.x;
                int ny = y + dir.y;
                int nz = z + dir.z;

                if (nx < 0 || ny < 0 || nz < 0)
                    continue;

                if (nx >= GridParameters.LevelGrid.SizeX ||
                    ny >= GridParameters.LevelGrid.Floors ||
                    nz >= GridParameters.LevelGrid.SizeZ)
                    continue;

                GridTile nextTile =
                    GridParameters.LevelGrid.GetTile(nx, nz, ny);

                if (nextTile == null)
                    continue;

                queue.Enqueue(
                    new VisibilityNode(
                        nextTile,
                        nextPeekUsed
                    )
                );
            }
        }

        foreach (GridTile tile in visibleTiles)
        {
            tile.Visibility[(int)team] = TileVisibility.Visible;
        }

        return visibleTiles;
    }

    // =========================================================

    private static bool HasFullCover(GridTile tile)
    {
        for (int i = 0; i < tile.Covers.Length; i++)
        {
            if (tile.Covers[i] == TileCover.Full)
                return true;
        }

        return false;
    }
}
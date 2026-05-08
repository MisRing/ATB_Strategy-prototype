using System;
using System.Collections.Generic;
using TArrayExtensions;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] private FloorData<RowData<GridTileDemo>>[] _demogrid;
    [SerializeField] private List<TArray<GridTile>> _grid;
    [SerializeField] private List<Vector3Int> _tilesWithCover;

    public int Floors { get { return _grid.Count; } }
    public int SizeX { get { return _grid[0].Size.x; } }
    public int SizeZ { get { return _grid[0].Size.y; } }

    private void Awake()
    {
        GridParameters.LevelGrid = this;
    }

    public void BuildGrid(int sizeX, int sizeZ, int floors)
    {
        _grid = new List<TArray<GridTile>>();
        _tilesWithCover =  new List<Vector3Int>();
        GridMapExtension.BuildGrid(ref _grid, ref _tilesWithCover, sizeX, sizeZ, floors, transform.position, this);
        GridParameters.LevelGrid = this;
    }

    public GridTile GetTile(int x, int z, int floor)
    {
        return _grid[floor][x, z];
    }

    public bool CheckTile(int x, int z, int floor)
    {
        if (x < 0 || z < 0 || x >= SizeX || z >= SizeZ || floor < 0 || floor >= _grid.Count) return false;

        if (!_grid[floor][x, z].IsGround || !_grid[floor][x, z].IsEmpty) return false;

        return true;
    }
    
    public void SetTileOwner(int x, int z, int floor, UnitController newOwner)
    {
        TArray<GridTile> floorGrid = _grid[floor];
        GridTile tile = _grid[floor][x, z];
        tile.Owner = newOwner;
        floorGrid[x, z] = tile;
        _grid[floor] = floorGrid;
    }

    public bool GetTileByWorldPos(ref GridTile tile, Vector3 worldPos)
    {
        worldPos -= transform.position;

        int x = Mathf.RoundToInt(worldPos.x / GridParameters.TILE_SIZE);
        int z = Mathf.RoundToInt(worldPos.z / GridParameters.TILE_SIZE);
        int floor = Mathf.FloorToInt(worldPos.y / GridParameters.LEVEL_HEIGHT);

        if (x < 0 || z < 0 || x >= SizeX || z >= SizeZ || floor < 0 || floor >= _grid.Count) return false;

        tile = _grid[floor][x, z];

        return true;
    }
    
    // public Vector3 GetTileWorldPos(GridTile tile)
    // {
    //     Vector3 worldPos = new Vector3(tile.PositionX * GridParameters.TILE_SIZE,
    //         tile.DeltaY,
    //         tile.PositionZ * GridParameters.TILE_SIZE);
    //
    //     worldPos += transform.position;
    //
    //     return worldPos;
    // }

    public Vector3 GetTileWorldPos(int x, int z, int floor)
    {
        GridTile tile = _grid[floor][x, z];

        return tile.WorldPosition;
    }

    public List<GridTile> GetTilesWithCover(Vector3 position, float range)
    {
        List<GridTile> tiles = new List<GridTile>();

        foreach (Vector3Int tilePos in _tilesWithCover)
        {
            GridTile tile = _grid[tilePos.y][tilePos.x, tilePos.z];

            if ((position - tile.WorldPosition).sqrMagnitude <= range * range)
            {
                tiles.Add(tile);
            }
        }

        return tiles;
    }

    private List<GridTile> _tilesAround = new List<GridTile>();
    public List<GridTile> GetTilesAround(Vector3 position, float range)
    {
        _tilesAround.Clear();
        position -= transform.position;

        int xCenter = Mathf.RoundToInt(position.x / GridParameters.TILE_SIZE);
        int zCenter = Mathf.RoundToInt(position.z / GridParameters.TILE_SIZE);
        int floorCenter = Mathf.FloorToInt(position.y / GridParameters.LEVEL_HEIGHT);

        int rangeInTiles = Mathf.CeilToInt(range / GridParameters.TILE_SIZE);
        int rangeInFloors = Mathf.CeilToInt(range / GridParameters.LEVEL_HEIGHT);

        float sqrtRange = range * range;
        
        int minX = Mathf.Max(0, xCenter - rangeInTiles);
        int maxX = Mathf.Min(_grid[0].Size.x - 1, xCenter + rangeInTiles);

        int minZ = Mathf.Max(0, zCenter - rangeInTiles);
        int maxZ = Mathf.Min(_grid[0].Size.y - 1, zCenter + rangeInTiles);

        int minF = Mathf.Max(0, floorCenter - rangeInFloors);
        int maxF = Mathf.Min(_grid.Count - 1, floorCenter + rangeInFloors);

        for(int f = minF; f <= maxF; f++)
        {
            for(int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    GridTile tile =  _grid[f][x, z];
                    if(!tile.IsGround) continue;
                    if(tile.Owner != null) continue;
                    float sqrtDistance = (position - tile.WorldPosition).sqrMagnitude;

                    if(sqrtDistance <= sqrtRange)
                    {
                        _tilesAround.Add(tile);
                    }
                }
            }
        }
        Debug.Log(_tilesAround.Count);

        return _tilesAround;
    }
}

[Serializable]
public struct RowData<T>
{
    public T[] Columns;
}

[Serializable]
public struct FloorData<T>
{
    public T[] Rows;
}
using TArrayExtensions;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] private List<TArray<GridTile>> _grid;
    [SerializeField] private List<GridTile> _tilesWithCover;

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
        _tilesWithCover =  new List<GridTile>();
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

    public Vector3 GetTileWorldPos(GridTile tile)
    {
        Vector3 worldPos = new Vector3(tile.PositionX * GridParameters.TILE_SIZE,
            tile.DeltaY,
            tile.PositionZ * GridParameters.TILE_SIZE);

        worldPos += transform.position;

        return worldPos;
    }

    public Vector3 GetTileWorldPos(int x, int z, int floor)
    {
        GridTile tile = _grid[floor][x, z];

        Vector3 worldPos = new Vector3(tile.PositionX* GridParameters.TILE_SIZE,
            tile.DeltaY,
            tile.PositionZ * GridParameters.TILE_SIZE);

        worldPos += transform.position;

        return worldPos;
    }

    public List<GridTile> GetTilesWithCover(Vector3 position, float range)
    {
        List<GridTile> tiles = new List<GridTile>();

        foreach (GridTile tile in _tilesWithCover)
        {
            Vector3 tilePos = GetTileWorldPos(tile);

            if (Vector3.Distance(position, tilePos) <= range)
            {
                tiles.Add(tile);
            }
        }

        return tiles;
    }

    public List<GridTile> GetTilesAround(Vector3 position, float range)
    {
        position -= transform.position;

        int xCenter = Mathf.RoundToInt(position.x / GridParameters.TILE_SIZE);
        int zCenter = Mathf.RoundToInt(position.z / GridParameters.TILE_SIZE);
        int floorCenter = Mathf.FloorToInt(position.y / GridParameters.LEVEL_HEIGHT);

        int rangeInTiles = Mathf.CeilToInt(range / GridParameters.TILE_SIZE);
        int rangeInFloors = Mathf.CeilToInt(range / GridParameters.LEVEL_HEIGHT);

        float sqrtRange = range * range;

        List<GridTile> tiles = new List<GridTile>();

        for(int f = floorCenter - rangeInFloors; f <= floorCenter + rangeInFloors; f++)
        {
            if (f < 0) f = 0;
            if (f >= _grid.Count) break;

            for(int x = xCenter - rangeInTiles; x <= xCenter + rangeInTiles; x++)
            {
                if (x < 0) x = 0;
                if (x >= _grid[f].Size.x) break;

                for (int z = zCenter - rangeInTiles; z <= zCenter + rangeInTiles; z++)
                {
                    if (z < 0) z = 0;
                    if (z >= _grid[f].Size.y) break;

                    Vector3 tileWorldPos = GetTileWorldPos(x, z, f);
                    float sqrtDistance = (position - tileWorldPos).sqrMagnitude;

                    if(sqrtDistance <= sqrtRange)
                    {
                        tiles.Add(GetTile(x, z, f));
                    }
                }
            }
        }

        return tiles;
    }
}

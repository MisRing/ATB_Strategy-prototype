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
        List<GridTile> tiles = new List<GridTile>();

        foreach (TArray<GridTile> floor in _grid)
        {
            for(int x = 0; x < floor.Size.x; x++)
            {
                for (int z = 0; z < floor.Size.y; z++)
                {
                    Vector3 tilePos = GetTileWorldPos(floor[x, z]);

                    if (Vector3.Distance(position, tilePos) <= range)
                    {
                        tiles.Add(floor[x, z]);
                    }
                }
            }
        }



        return tiles;
    }
}

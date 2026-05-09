using System;
using System.Collections.Generic;
using TArrayExtensions;
using UnityEngine;
using UnityEngine.UIElements;

public class GridMap : MonoBehaviour
{
    [SerializeField] private FloorData[] _grid;
    //[SerializeField] private List<TArray<GridTile>> _grid;
    //[SerializeField] private List<Vector3Int> _tilesWithCover;

    public int Floors { get { return _grid.Length; } }
    public int SizeX { get { return _grid[0].Length; } }
    public int SizeZ { get { return _grid[0][0].Length; } }

    private void Awake()
    {
        GridParameters.LevelGrid = this;
    }

    public void BuildGrid(int sizeX, int sizeZ, int floors)
    {
        //_grid = new FloorData[floors];
        GridMapExtension.BuildGrid(ref _grid, sizeX, sizeZ, floors, transform.position, this);
        GridParameters.LevelGrid = this;
    }

    public GridTileDemo GetTile(int x, int z, int floor)
    {
        return _grid[floor][x][z];
    }

    public bool CheckTile(int x, int z, int floor)
    {
        if(_grid.Length == 0 || _grid[0].Length == 0 || _grid[0][0].Length == 0) return false;
        if (x < 0 || z < 0 || x >= SizeX || z >= SizeZ || floor < 0 || floor >= Floors) return false;

        if (!_grid[floor][x][z].IsGround || !_grid[floor][x][z].IsEmpty) return false;

        return true;
    }

    public GridTileDemo GetTileByWorldPos(Vector3 worldPos)
    {
        worldPos -= transform.position;

        int x = Mathf.RoundToInt(worldPos.x / GridParameters.TILE_SIZE);
        int z = Mathf.RoundToInt(worldPos.z / GridParameters.TILE_SIZE);
        int floor = Mathf.FloorToInt(worldPos.y / GridParameters.LEVEL_HEIGHT);

        if (x < 0 || z < 0 || x >= SizeX || z >= SizeZ || floor < 0 || floor >= Floors) return null;

        return _grid[floor][x][z];
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
}

[Serializable]
public class RowData
{
    public GridTileDemo[] Columns;

    public GridTileDemo this[int index]
    {
        get => Columns[index];
        set => Columns[index] = value;
    }

    public int Length { get => Columns.Length; }

    public RowData(int count)
    {
        Columns = new GridTileDemo[count];
    }
}

[Serializable]
public class FloorData
{
    public RowData[] Rows;

    public RowData this[int index]
    {
        get => Rows[index];
        set => Rows[index] = value;
    }

    public int Length { get => Rows.Length; }

    public FloorData(int count)
    {
        Rows = new RowData[count];
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] private FloorData[] _grid;


    public int Floors { get { return _grid.Length; } }
    public int SizeX { get { return _grid[0].Length; } }
    public int SizeZ { get { return _grid[0][0].Length; } }

    private void Awake()
    {
        GridParameters.LevelGrid = this;
    }
#if UNITY_EDITOR
    public void BuildGrid(int sizeX, int sizeZ, int floors)
    {
        GridMapExtension.BuildGrid(ref _grid, sizeX, sizeZ, floors, transform.position, this);
        GridParameters.LevelGrid = this;
    }
#endif

    public FloorData[] GetGrid()
    {
        return _grid;
    }

    public GridTile GetTile(int x, int z, int floor)
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

    public GridTile GetTileByWorldPos(Vector3 worldPos)
    {
        worldPos -= transform.position;

        int x = Mathf.RoundToInt(worldPos.x / GridParameters.TILE_SIZE);
        int z = Mathf.RoundToInt(worldPos.z / GridParameters.TILE_SIZE);
        int floor = Mathf.FloorToInt(worldPos.y / GridParameters.LEVEL_HEIGHT);

        if (x < 0 || z < 0 || x >= SizeX || z >= SizeZ || floor < 0 || floor >= Floors) return null;

        return _grid[floor][x][z];
    }

    public List<GridTile> GetTilesAround(GridTile startTile, float range, bool onlyGrounded)
    {
        List<GridTile> result = new List<GridTile>();
        HashSet<GridTile> visited = new HashSet<GridTile>();

        float sqrRange = range * range;
        Vector3 center = startTile.WorldPosition;

        Stack<GridTile> stack = new Stack<GridTile>();
        stack.Push(startTile);

        while (stack.Count > 0)
        {
            GridTile tile = stack.Pop();

            if (tile == null)
                continue;

            if (onlyGrounded && !tile.IsGround)
                continue;

            if ((tile.WorldPosition - center).sqrMagnitude > sqrRange)
                continue;

            if (!visited.Add(tile))
                continue;

            result.Add(tile);

            int x = tile.PositionX;
            int y = tile.Floor;
            int z = tile.PositionZ;

            for (int i = 0; i < GridParameters.TILE_SIDES.Length; i++)
            {
                Vector3Int dir = GridParameters.TILE_SIDES[i];

                int nx = x + dir.x;
                int ny = y + dir.y;
                int nz = z + dir.z;

                if (nx < 0 || nz < 0 || ny < 0)
                    continue;

                if (nx >= SizeX || nz >= SizeZ || ny >= Floors)
                    continue;

                stack.Push(_grid[ny][nx][nz]);
            }
        }

        return result;
    }
}

[Serializable]
public class RowData
{
    public GridTile[] Columns;

    public GridTile this[int index]
    {
        get => Columns[index];
        set => Columns[index] = value;
    }

    public int Length { get => Columns.Length; }

    public RowData(int count)
    {
        Columns = new GridTile[count];
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
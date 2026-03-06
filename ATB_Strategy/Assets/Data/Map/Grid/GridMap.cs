using Extensions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridMap : MonoBehaviour
{
    [SerializeField] private TArray<GridTile> _grid;

    public int SizeX { get { return _grid.Size.x; } }
    public int SizeZ { get { return _grid.Size.y; } }

    private void Awake()
    {
        GridParameters.LevelGrid = this;
    }

    public void BuildGrid(int sizeX, int sizeZ)
    {
        GridMapExtansion.BuildGrid(ref _grid, sizeX, sizeZ, transform.position, this);
        GridParameters.LevelGrid = this;
    }

    public GridTile GetTile(int x, int z)
    {
        return _grid[x, z];
    }

    public bool GetTileByWorldPos(ref GridTile tile, Vector3 worldPos)
    {
        worldPos -= transform.position;

        int x = Mathf.FloorToInt(worldPos.x / GridParameters.TILE_SIZE);
        int z = Mathf.FloorToInt(worldPos.z / GridParameters.TILE_SIZE);

        if (x < 0 || z < 0 || x >= SizeX || z >= SizeZ) return false;

        tile = _grid[x, z];

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

    public Vector3 GetTileWorldPos(int x, int z)
    {
        GridTile tile = _grid[x, z];

        Vector3 worldPos = new Vector3(tile.PositionX * GridParameters.TILE_SIZE,
            tile.DeltaY,
            tile.PositionZ * GridParameters.TILE_SIZE);

        worldPos += transform.position;

        return worldPos;
    }
}

public static class GridMapExtansion
{
    public static void BuildGrid(ref TArray<GridTile> grid, int newX, int newZ, Vector3 gridOffset, GridMap gridMapObject)
    {
        grid = new GridTile[newX, newZ];

        for (int x = 0; x < newX; x++)
        {
            for (int z = 0; z < newZ; z++)
            {
                GridTile newTile = new GridTile();

                newTile.Floor = 0;

                newTile.PositionX = x;
                newTile.PositionZ = z;

                SetTileGround(ref newTile, gridOffset);

                SetTileObstacles(ref newTile, gridOffset);

                SetTileCovers(ref  newTile, gridOffset);

                grid[x,z] = newTile;
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.SetDirty(gridMapObject);
    }

    private static void SetTileGround(ref GridTile tile, Vector3 gridOffset)
    {
        Vector3 rayOrigin = new Vector3(tile.PositionX, GridParameters.LEVEL_HEIGHT, tile.PositionZ) + gridOffset;
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin,
            Vector3.down,
            out hit,
            GridParameters.LEVEL_HEIGHT,
            GridParameters.ENVIRONMENT_MASK,
            QueryTriggerInteraction.Ignore))
        {
            float height = hit.point.y;

            for (int i = 0; i < 4; i++)
            {
                rayOrigin = new Vector3(tile.PositionX, GridParameters.LEVEL_HEIGHT, tile.PositionZ) + gridOffset;
                rayOrigin = rayOrigin + (GridParameters.COVER_DIRECTIONS[i] + GridParameters.COVER_DIRECTIONS[(i + 1) % 4]) * GridParameters.TILE_SIZE * 0.45f;

                if (Physics.Raycast(rayOrigin,
                    Vector3.down,
                    out hit,
                    GridParameters.LEVEL_HEIGHT,
                    GridParameters.ENVIRONMENT_MASK,
                    QueryTriggerInteraction.Ignore))
                {
                    if(hit.point.y > height)
                    {
                        height = hit.point.y;
                    }
                }
            }

            tile.DeltaY = height - gridOffset.y;

            tile.IsGround = true;
        }
        else
        {
            tile.DeltaY = 0f;

            tile.IsGround = false;
        }
    }

    private static void SetTileObstacles(ref GridTile tile, Vector3 gridOffset)
    {
        Vector3 tileSize = new Vector3(GridParameters.TILE_SIZE, GridParameters.LEVEL_HEIGHT, GridParameters.TILE_SIZE) * 0.8f;

        Vector3 castCenter = new Vector3(tile.PositionX, tile.DeltaY + tileSize.y * 0.5f + 0.1f, tile.PositionZ) + gridOffset;

        if (Physics.CheckBox(castCenter, tileSize * 0.5f, Quaternion.identity, GridParameters.ENVIRONMENT_MASK))
        {
            tile.IsEmpty = false;
        }
        else
        {
            tile.IsEmpty = true;
        }
    }
    
    private static void SetTileCovers(ref GridTile tile, Vector3 gridOffset)
    {
        tile.Covers = new TileCover[4];
        
        if (!tile.IsEmpty || !tile.IsGround) return;
        
        Vector3 position = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + gridOffset;

        for (int i = 0; i < 4; i++)
        {
            if (Physics.Raycast(position + Vector3.up * GridParameters.LOW_COVER_HEIGHT, GridParameters.COVER_DIRECTIONS[i], GridParameters.TILE_SIZE * 0.55f))
            {

                if (Physics.Raycast(position + Vector3.up * GridParameters.FULL_COVER_HEIGHT, GridParameters.COVER_DIRECTIONS[i], GridParameters.TILE_SIZE * 0.55f))
                {
                    tile.Covers[i] = TileCover.Full;
                }
                else
                {
                    tile.Covers[i] = TileCover.Low;
                }
            }
        }
    }
}

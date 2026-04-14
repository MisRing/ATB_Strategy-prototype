using System.Collections.Generic;
using UnityEngine;
using TArrayExtensions;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class GridMapExtension
{
    public static void BuildGrid(ref List<TArray<GridTile>> grid, int newX, int newZ, int floorsCount, Vector3 gridOffset, GridMap gridMapObject)
    {
        for (int f = 0; f < floorsCount; f++)
        {
            TArray<GridTile> floorGrid = new GridTile[newX, newZ];

            for (int x = 0; x < newX; x++)
            {
                for (int z = 0; z < newZ; z++)
                {
                    GridTile newTile = new GridTile();

                    newTile.Floor = f;

                    newTile.PositionX = x;
                    newTile.PositionZ = z;

                    SetTileGround(ref newTile, gridOffset);

                    SetTileObstacles(ref newTile, gridOffset);

                    SetTileCovers(ref newTile, gridOffset);

                    floorGrid[x, z] = newTile;
                }
            }

            grid.Add(floorGrid);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.SetDirty(gridMapObject);
    }

    private static void SetTileGround(ref GridTile tile, Vector3 gridOffset)
    {
        Vector3 rayOrigin = new Vector3(tile.PositionX * GridParameters.TILE_SIZE,
            GridParameters.LEVEL_HEIGHT * (tile.Floor + 1),
            tile.PositionZ * GridParameters.TILE_SIZE)
            + gridOffset;

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
                rayOrigin = new Vector3(tile.PositionX * GridParameters.TILE_SIZE,
                    GridParameters.LEVEL_HEIGHT * (tile.Floor + 1),
                    tile.PositionZ * GridParameters.TILE_SIZE)
                    + gridOffset;

                rayOrigin = rayOrigin
                    + (GridParameters.COVER_DIRECTIONS[i] + GridParameters.COVER_DIRECTIONS[(i + 1) % 4])
                    * GridParameters.TILE_SIZE * 0.45f;

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
            tile.DeltaY = GridParameters.LEVEL_HEIGHT * tile.Floor;

            tile.IsGround = false;
        }
    }

    private static void SetTileObstacles(ref GridTile tile, Vector3 gridOffset)
    {
        Vector3 tileSize = new Vector3(GridParameters.TILE_SIZE, GridParameters.LEVEL_HEIGHT, GridParameters.TILE_SIZE) * 0.8f;

        Vector3 castCenter = new Vector3(tile.PositionX * GridParameters.TILE_SIZE,
            tile.DeltaY + tileSize.y * 0.5f + 0.1f,
            tile.PositionZ * GridParameters.TILE_SIZE) + gridOffset;

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
        
        Vector3 position = new Vector3(tile.PositionX * GridParameters.TILE_SIZE,
            tile.DeltaY,
            tile.PositionZ * GridParameters.TILE_SIZE)
            + gridOffset;

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
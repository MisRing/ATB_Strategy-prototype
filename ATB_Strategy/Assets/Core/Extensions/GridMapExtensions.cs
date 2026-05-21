using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class GridMapExtension
{
    public static void BuildGrid(ref FloorData[] grid, int newX, int newZ, int floorsCount, Vector3 gridOffset, GridMap gridMapObject)
    {
        grid = new FloorData[floorsCount];
        for (int f = 0; f < floorsCount; f++)
        {
            grid[f] = new FloorData(newX);

            for (int x = 0; x < newX; x++)
            {
                grid[f][x] = new RowData(newZ);
                for (int z = 0; z < newZ; z++)
                {
                    GridTile newTile = new GridTile(x, z, f);

                    SetTileGround(ref newTile, gridOffset);

                    SetTileObstacles(ref newTile);

                    SetTileCovers(ref newTile);

                    grid[f][x][z] = newTile;
                }
            }
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

        float tileY = 0;

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

                rayOrigin += (GridParameters.COVER_DIRECTIONS[i] + GridParameters.COVER_DIRECTIONS[(i + 1) % 4])
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

            tileY = height - gridOffset.y;

            tile.IsGround = true;
        }
        else
        {
            tileY = GridParameters.LEVEL_HEIGHT * tile.Floor;

            tile.IsGround = false;
        }
        
        Vector3 worldPos = new Vector3(tile.PositionX * GridParameters.TILE_SIZE,
            tileY,
            tile.PositionZ * GridParameters.TILE_SIZE);

        worldPos += gridOffset;

        tile.WorldPosition = worldPos;
    }

    private static void SetTileObstacles(ref GridTile tile)
    {
        Vector3 tileSize = new Vector3(GridParameters.TILE_SIZE, GridParameters.LEVEL_HEIGHT, GridParameters.TILE_SIZE) * 0.8f;

        Vector3 castCenter = tile.WorldPosition + Vector3.up * (GridParameters.LEVEL_HEIGHT * 0.5f + 0.1f);

        if (Physics.CheckBox(castCenter, tileSize * 0.5f, Quaternion.identity, GridParameters.ENVIRONMENT_MASK))
        {
            tile.IsEmpty = false;
        }
        else
        {
            tile.IsEmpty = true;
        }
    }
    
    private static void SetTileCovers(ref GridTile tile)
    {
        tile.Covers = new TileCover[4];
        
        if (!tile.IsEmpty || !tile.IsGround) return;

        for (int i = 0; i < 4; i++)
        {
            if (Physics.Raycast(tile.WorldPosition + Vector3.up * GridParameters.LOW_COVER_HEIGHT, GridParameters.COVER_DIRECTIONS[i], GridParameters.TILE_SIZE * 0.55f))
            {

                if (Physics.Raycast(tile.WorldPosition + Vector3.up * GridParameters.FULL_COVER_HEIGHT, GridParameters.COVER_DIRECTIONS[i], GridParameters.TILE_SIZE * 0.55f))
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
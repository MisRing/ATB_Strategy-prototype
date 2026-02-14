using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] public GridTile[] _grid;

    [SerializeField] private int _sizeX;
    [SerializeField] private int _sizeZ;

    public int SizeX { get { return _sizeX; } }
    public int SizeZ { get { return _sizeZ; } }

    private Transform _floorCollection;
    private Transform _obstacleCollection;

    public Vector3 GetTilePos(float worldX, float worldZ)
    {
        int x = (int)MathF.Round(worldX);
        int z = (int)MathF.Round(worldZ);

        if (x < 0 || z < 0 || x >= _sizeX || z >= _sizeZ) return -Vector3.one;

        GridTile tile = _grid[GridMapGenerator.GetIndex(this, x, z)];

        Vector3 tilePos = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + transform.position;
        return tilePos;
    }

    public void BuildGrid(int sizeX, int sizeZ)
    {
        if (!_floorCollection)
        {
            _floorCollection = new GameObject("Floors").transform;
            _floorCollection.parent = transform;
            _floorCollection.localPosition = Vector3.zero;
        }

        if (!_obstacleCollection)
        {
            _obstacleCollection = new GameObject("Obstacles").transform;
            _obstacleCollection.parent = transform;
            _obstacleCollection.localPosition = Vector3.zero;
        }

        GridMapGenerator.BuildGrid(this, sizeX, sizeZ);
        _sizeX = sizeX;
        _sizeZ = sizeZ;
    }

    public void SetGrid()
    {
        GridMapGenerator.SetGrid(this, _floorCollection, _obstacleCollection);
    }
}

public static class GridMapGenerator
{
    public static void BuildGrid(GridMap gridMap, int newX, int newZ)
    {
        gridMap._grid = new GridTile[newX * newZ];

        for (int x = 0; x < newX; x++)
        {
            for (int z = 0; z < newZ; z++)
            {
                GridTile newTile = new GridTile();

                newTile.PositionX = x;
                newTile.PositionZ = z;

                gridMap._grid[GetIndex(gridMap, x, z)] = newTile;
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.SetDirty(gridMap);
    }

    public static int GetIndex(GridMap gridMap, int x, int z)
    {
        return x + z * gridMap.SizeX;
    }

    public static void SetGrid(GridMap gridMap, Transform groundsTransform, Transform obstaclesTransform)
    {
        for (int x = 0; x < gridMap.SizeX; x++)
        {
            for (int z = 0; z < gridMap.SizeZ; z++)
            {
                SetTileGround(gridMap, ref gridMap._grid[GetIndex(gridMap, x, z)], groundsTransform);

                SetTileObstacles(gridMap, ref gridMap._grid[GetIndex(gridMap, x, z)], obstaclesTransform);
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.SetDirty(gridMap);
    }

    private static void SetTileGround(GridMap gridMap, ref GridTile tile, Transform groundsTransform)
    {
        LayerMask layer = LayerMask.GetMask("Ground");
        Vector3 rayOrigin = new Vector3(tile.PositionX, 3.5f, tile.PositionZ) + gridMap.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 10f, layer, QueryTriggerInteraction.Ignore))
        {
            float height = hit.point.y;
            GameObject floor = hit.collider.gameObject;
            if (!floor.transform.parent)
            {
                floor.transform.parent = groundsTransform;
            }
            tile.DeltaY = height - gridMap.transform.position.y;

            tile.IsGround = true;
        }
        else
        {
            tile.DeltaY = 0;

            tile.IsGround = false;
        }
    }

    private static void SetTileObstacles(GridMap gridMap, ref GridTile tile, Transform obstaclesTransform)
    {
        LayerMask layer = LayerMask.GetMask("Obstacle");

        Vector3 castCenter = new Vector3(tile.PositionX, tile.DeltaY + 1.5f, tile.PositionZ) + gridMap.transform.position;

        Collider[] colliders = Physics.OverlapBox(castCenter, new Vector3(0.4f, 1.5f, 0.4f), Quaternion.identity, layer);

        if(colliders.Length > 0 )
        {
            tile.IsEmpty = false;

            for(int i = 0; i < colliders.Length; i++)
            {
                Transform obstacle = colliders[i].transform;
                if(!obstacle.parent)
                {
                    obstacle.parent = obstaclesTransform;
                }
            }
        }
        else
        {
            tile.IsEmpty = true;
        }
    }
}

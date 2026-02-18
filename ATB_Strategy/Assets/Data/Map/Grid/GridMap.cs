using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class GridMap : MonoBehaviour
{
    [SerializeField] public GridTile[] _grid;

    [SerializeField] private int _sizeX;
    [SerializeField] private int _sizeZ;

    public int SizeX { get { return _sizeX; } }
    public int SizeZ { get { return _sizeZ; } }

    private Transform _floorCollection;
    private Transform _obstacleCollection;

    public bool HasTile(float worldX, float worldZ)
    {
        int x = (int)Math.Round(worldX);
        int z = (int)Math.Round(worldZ);

        if (x < 0 || z < 0 || x >= _sizeX || z >= _sizeZ) return false;

        return true;
    }

    public Vector3 GetTileWorldPosition(float worldX, float worldZ)
    {
        int x = (int)Math.Round(worldX);
        int z = (int)Math.Round(worldZ);

        GridTile tile = _grid[GridMapExtansion.GetIndex(this, x, z)];

        Vector3 tileWorldPos = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + transform.position;

        return tileWorldPos;
    }

    public Vector3[] GetPath(Vector3 start, Vector3 end)
    {
        NavMeshPath path = new NavMeshPath();

        bool found = NavMesh.CalculatePath(
            start,
            end,
            NavMesh.AllAreas,
            path);

        if (!found || path.status != NavMeshPathStatus.PathComplete)
            return null;

        return path.corners;
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

        GridMapExtansion.BuildGrid(this, sizeX, sizeZ);
        _sizeX = sizeX;
        _sizeZ = sizeZ;
    }

    public void SetGrid()
    {
        GridMapExtansion.SetGrid(this, _floorCollection, _obstacleCollection);
    }
}

public static class GridMapExtansion
{
    private static float GROUND_RAY_DISTANCE = 10f;
    private static float FLOOR_HEIGHT = 3.5f;
    private static Vector3 TILE_SIZE = new Vector3(0.8f, 3f, 0.8f);

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
                newTile.GridOffset = gridMap.transform.position;

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
                SetTileGround(ref gridMap._grid[GetIndex(gridMap, x, z)], groundsTransform);

                SetTileObstacles(ref gridMap._grid[GetIndex(gridMap, x, z)], obstaclesTransform);
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.SetDirty(gridMap);
    }

    private static void SetTileGround(ref GridTile tile, Transform groundsTransform)
    {
        LayerMask layer = LayerMask.GetMask("Ground");
        Vector3 rayOrigin = new Vector3(tile.PositionX, FLOOR_HEIGHT, tile.PositionZ) + tile.GridOffset;
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, GROUND_RAY_DISTANCE, layer, QueryTriggerInteraction.Ignore))
        {
            float height = hit.point.y;
            GameObject floor = hit.collider.gameObject;
            if (!floor.transform.parent)
            {
                floor.transform.parent = groundsTransform;
            }
            tile.DeltaY = height - tile.GridOffset.y;

            tile.IsGround = true;
        }
        else
        {
            tile.DeltaY = 0;

            tile.IsGround = false;
        }
    }

    private static void SetTileObstacles(ref GridTile tile, Transform obstaclesTransform)
    {
        LayerMask layer = LayerMask.GetMask("Obstacle");

        Vector3 castCenter = new Vector3(tile.PositionX, tile.DeltaY + TILE_SIZE.y * 0.5f, tile.PositionZ) + tile.GridOffset;

        Collider[] colliders = Physics.OverlapBox(castCenter, TILE_SIZE * 0.5f, Quaternion.identity, layer);

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

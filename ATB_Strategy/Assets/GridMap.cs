using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public GridTile[,] _grid;

    private Transform _tileCollection;
    private Transform _floorCollection;
    private Transform _obstacleCollection;

    public void RecreateGrid(int newX, int newZ)
    {
        while(transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        _tileCollection = new GameObject("Tiles").transform;
        _tileCollection.parent = transform;
        _tileCollection.localPosition = Vector3.zero;

        _floorCollection = new GameObject("Floors").transform;
        _floorCollection.parent = transform;
        _floorCollection.localPosition = Vector3.zero;

        _obstacleCollection = new GameObject("Obstacles").transform;
        _obstacleCollection.parent = transform;
        _obstacleCollection.localPosition = Vector3.zero;

        _grid = new GridTile[newX, newZ];

        for(int x = 0; x < newX; x++)
        {
            for (int z = 0; z < newZ; z++)
            {
                GameObject newTileObject = new GameObject($"Tile [{x},{z}]");
                newTileObject.AddComponent<GridTile>();
                GridTile newTile = newTileObject.GetComponent<GridTile>();

                newTile.transform.parent = _tileCollection;
                newTile.transform.position = new Vector3(x, 0, z);
                newTile.positionX = x;
                newTile.positionZ = z;

                _grid[x,z] = newTile;
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.SetDirty(this);
    }

    public void BakeGrid()
    {
        for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for (int z = 0; z < _grid.GetLength(1); z++)
            {
                BakeTileFloor(_grid[x, z]);
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.SetDirty(this);
    }

    private void BakeTileFloor(GridTile tile)
    {
        LayerMask layer = LayerMask.GetMask("Ground");
        Vector3 rayOrigin = new Vector3(tile.positionX, transform.position.y + 3.5f, tile.positionZ);
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 10f, layer, QueryTriggerInteraction.Ignore))
        {
            float height = hit.point.y;
            GameObject floor = hit.collider.gameObject;
            if(!floor.transform.parent)
            {
                floor.transform.parent = _floorCollection;
            }
            tile.transform.position = new Vector3(tile.transform.position.x, height, tile.transform.position.z);

            tile.floor = floor;
        }
        else
        {
            tile.transform.position = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z);
            tile.floor = null;
        }
    }
}

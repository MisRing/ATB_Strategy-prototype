using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class GridMapDebug
{
    static float debugOffset = 0.4f;
    static float lineThickdess = 3f;


    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawGizmo(GridMap gridMap, GizmoType gizmoType)
    {
        if (!GridMapEditor.DrawDebug) return;
        if (gridMap._grid == null) return;


        for (int x = 0; x < gridMap._grid.GetLength(0); x++)
        {
            for (int z = 0; z < gridMap._grid.GetLength(1); z++)
            {
                GridTile tile = gridMap._grid[x, z];

                if (!tile) continue;

                if(!tile.floor)
                {
                    Handles.color = GridMapEditor.EmptyColor;
                }
                else if(tile.objects.Count > 0)
                {
                    Handles.color = GridMapEditor.NotEmptyColor;
                }
                else
                {
                    Handles.color = GridMapEditor.DefaultColor;
                }

                Vector3 tilePos = tile.transform.position;

                Vector3 point1 = new Vector3(debugOffset, 0, debugOffset) + tilePos;
                Vector3 point2 = new Vector3(debugOffset, 0, -debugOffset) + tilePos;
                Vector3 point3 = new Vector3(-debugOffset, 0, -debugOffset) + tilePos;
                Vector3 point4 = new Vector3(-debugOffset, 0, debugOffset) + tilePos;

                Handles.DrawLine(point1, point2, lineThickdess);
                Handles.DrawLine(point2, point3, lineThickdess);
                Handles.DrawLine(point3, point4, lineThickdess);
                Handles.DrawLine(point4, point1, lineThickdess);

                if (x >= gridMap._grid.GetLength(0) - 1 || z >= gridMap._grid.GetLength(1) - 1) continue;

                if(x < gridMap._grid.GetLength(0) - 1 && tile.floor && gridMap._grid[x + 1, z].floor)
                {
                    Handles.color = Color.blue;

                    Vector3 from = (gridMap._grid[x + 1, z].transform.position - tile.transform.position) * 0.3f + tile.transform.position;
                    Vector3 to = (gridMap._grid[x + 1, z].transform.position - tile.transform.position) * 0.7f + tile.transform.position;

                    Handles.DrawLine(from, to, lineThickdess);
                }

                if (z < gridMap._grid.GetLength(1) - 1 && tile.floor && gridMap._grid[x, z + 1].floor)
                {
                    Handles.color = Color.blue;

                    Vector3 from = (gridMap._grid[x, z + 1].transform.position - tile.transform.position) * 0.3f + tile.transform.position;
                    Vector3 to = (gridMap._grid[x, z + 1].transform.position - tile.transform.position) * 0.7f + tile.transform.position;

                    Handles.DrawLine(from, to, lineThickdess);
                }
            }
        }
    }
}

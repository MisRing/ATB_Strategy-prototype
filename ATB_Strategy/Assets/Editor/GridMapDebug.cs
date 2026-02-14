using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class GridMapDebug
{
    static float debugOffset = 0.4f;
    static float lineThickdess = 2f;


    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawGizmo(GridMap gridMap, GizmoType gizmoType)
    {
        if (!GridMapEditor.DrawDebug) return;
        if (gridMap._grid == null) return;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

        for (int x = 0; x < gridMap.SizeX; x++)
        {
            for (int z = 0; z < gridMap.SizeZ; z++)
            {
                GridTile tile = gridMap._grid[GridMapGenerator.GetIndex(gridMap, x, z)];

                if(!tile.IsGround)
                {
                    Handles.color = GridMapEditor.EmptyColor;
                }
                else if(!tile.IsEmpty)
                {
                    Handles.color = GridMapEditor.NotEmptyColor;
                }
                else
                {
                    Handles.color = GridMapEditor.DefaultColor;
                }

                Vector3 tilePos = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + gridMap.transform.position;

                Vector3 point1 = new Vector3(debugOffset, 0, debugOffset) + tilePos;
                Vector3 point2 = new Vector3(debugOffset, 0, -debugOffset) + tilePos;
                Vector3 point3 = new Vector3(-debugOffset, 0, -debugOffset) + tilePos;
                Vector3 point4 = new Vector3(-debugOffset, 0, debugOffset) + tilePos;

                Handles.DrawLine(point1, point2, lineThickdess);
                Handles.DrawLine(point2, point3, lineThickdess);
                Handles.DrawLine(point3, point4, lineThickdess);
                Handles.DrawLine(point4, point1, lineThickdess);

                if (x >= gridMap.SizeX - 1 || z >= gridMap.SizeZ - 1) continue;

                GridTile otherTile = gridMap._grid[GridMapGenerator.GetIndex(gridMap, x + 1, z)];
                if (x < gridMap.SizeX - 1 && tile.IsGround && otherTile.IsGround)
                {
                    Handles.color = Color.blue;

                    Vector3 tilePos1 = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + gridMap.transform.position;
                    Vector3 tilePos2 = new Vector3(otherTile.PositionX, otherTile.DeltaY, otherTile.PositionZ) + gridMap.transform.position;

                    Vector3 from = (tilePos2 - tilePos1) * 0.3f + tilePos1;
                    Vector3 to = (tilePos2 - tilePos1) * 0.7f + tilePos1;

                    Handles.DrawLine(from, to, lineThickdess);
                }

                otherTile = gridMap._grid[GridMapGenerator.GetIndex(gridMap, x, z + 1)];
                if (z < gridMap.SizeZ - 1 && tile.IsGround && otherTile.IsGround)
                {
                    Handles.color = Color.blue;

                    Vector3 tilePos1 = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + gridMap.transform.position;
                    Vector3 tilePos2 = new Vector3(otherTile.PositionX, otherTile.DeltaY, otherTile.PositionZ) + gridMap.transform.position;

                    Vector3 from = (tilePos2 - tilePos1) * 0.3f + tilePos1;
                    Vector3 to = (tilePos2 - tilePos1) * 0.7f + tilePos1;

                    Handles.DrawLine(from, to, lineThickdess);
                }
            }
        }
    }
}

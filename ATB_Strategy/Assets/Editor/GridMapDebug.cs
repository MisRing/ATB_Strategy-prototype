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
                GridTile tile = gridMap._grid[GridMapExtansion.GetIndex(gridMap, x, z)];

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

                if (x + 1 >= gridMap.SizeX || z + 1 >= gridMap.SizeZ) continue;

                GridTile otherTile = gridMap._grid[GridMapExtansion.GetIndex(gridMap, x + 1, z)];
                if (x < gridMap.SizeX - 1 && tile.IsGround && otherTile.IsGround)
                {
                    Handles.color = Color.blue;

                    Vector3 tilePos1 = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + gridMap.transform.position;
                    Vector3 tilePos2 = new Vector3(otherTile.PositionX, otherTile.DeltaY, otherTile.PositionZ) + gridMap.transform.position;

                    Vector3 from = (tilePos2 - tilePos1) * 0.3f + tilePos1;
                    Vector3 to = (tilePos2 - tilePos1) * 0.7f + tilePos1;

                    Handles.DrawLine(from, to, lineThickdess);
                }

                otherTile = gridMap._grid[GridMapExtansion.GetIndex(gridMap, x, z + 1)];
                if (z < gridMap.SizeZ - 1 && tile.IsGround && otherTile.IsGround)
                {
                    Handles.color = Color.blue;

                    Vector3 tilePos1 = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + gridMap.transform.position;
                    Vector3 tilePos2 = new Vector3(otherTile.PositionX, otherTile.DeltaY, otherTile.PositionZ) + gridMap.transform.position;

                    Vector3 from = (tilePos2 - tilePos1) * 0.3f + tilePos1;
                    Vector3 to = (tilePos2 - tilePos1) * 0.7f + tilePos1;

                    Handles.DrawLine(from, to, lineThickdess);
                }

                for (int i = 0; i < 4; i++)
                {
                    float height = 0;
                    if (tile.Covers[i] == TileCover.Low)
                    {
                        Handles.color = Color.red;
                        height = 0.5f;

                    }
                    else if (tile.Covers[i] == TileCover.Full)
                    {
                        Handles.color = Color.black;
                        height = 1f;
                    }
                    else continue;
                    
                    Vector3 cPoint1 = new Vector3(_directions[i][0], 0, _directions[i][1]) + tilePos;
                    Vector3 cPoint2 = new Vector3(_directions[i][0], height, _directions[i][1]) + tilePos;
                    Vector3 cPoint3 = new Vector3(_directions[i][2], height, _directions[i][3]) + tilePos;
                    Vector3 cPoint4 = new Vector3(_directions[i][2], 0, _directions[i][3]) + tilePos;

                    Handles.DrawLine(cPoint1, cPoint2, lineThickdess);
                    Handles.DrawLine(cPoint2, cPoint3, lineThickdess);
                    Handles.DrawLine(cPoint3, cPoint4, lineThickdess);
                    Handles.DrawLine(cPoint4, cPoint1, lineThickdess);
                }
            }
        }
    }

    private static readonly float[][] _directions =
    {
        new[] { debugOffset, debugOffset, -debugOffset, debugOffset },
        new[] { debugOffset, debugOffset, debugOffset, -debugOffset },
        new[] { debugOffset, -debugOffset, -debugOffset, -debugOffset },
        new[] { -debugOffset, -debugOffset, -debugOffset, debugOffset },
    };
}

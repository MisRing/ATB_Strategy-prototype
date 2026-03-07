using log4net.Util;
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

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

        for (int f = 0; f < gridMap.Floors; f++)
        {

            for (int x = 0; x < gridMap.SizeX; x++)
            {
                for (int z = 0; z < gridMap.SizeZ; z++)
                {
                    GridTile tile = gridMap.GetTile(x, z, f);

                    if (!tile.IsGround)
                    {
                        //Handles.color = GridMapEditor.EmptyColor;
                        continue;
                    }
                    else if (!tile.IsEmpty)
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

                    if (IsSceneViewCameraInRange(tilePos, 7f))
                    {
                        GUIStyle style = new GUIStyle();
                        style.normal.textColor = Color.black;
                        Handles.Label(tilePos + new Vector3(-debugOffset * 0.55f, 0f, -debugOffset * 0.55f), "floor_" + tile.Floor, style);
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
    }

    public static bool IsSceneViewCameraInRange(Vector3 position, float distance)
    {
        Vector3 cameraPos = Camera.current.WorldToScreenPoint(position);
        return ((cameraPos.x >= 0) &&
        (cameraPos.x <= Camera.current.pixelWidth) &&
        (cameraPos.y >= 0) &&
        (cameraPos.y <= Camera.current.pixelHeight) &&
        (cameraPos.z > 0) &&
        (cameraPos.z < distance));
    }

    private static readonly float[][] _directions =
    {
        new[] { debugOffset, debugOffset, -debugOffset, debugOffset },
        new[] { debugOffset, debugOffset, debugOffset, -debugOffset },
        new[] { debugOffset, -debugOffset, -debugOffset, -debugOffset },
        new[] { -debugOffset, -debugOffset, -debugOffset, debugOffset },
    };
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridMap))]
public class GridMapEditor : Editor
{
    private static int _newSizeX = 1;
    private static int _newSizeZ = 1;
    private GridMap _gridMap;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _gridMap = (GridMap)target;

        DrawGUI();

        //if (GUILayout.Button("Button Label"))
        //{
        //    // When the button is clicked, call the function in the target script
        //    //scriptReference.MyFunction();
        //}
    }

    private void DrawGUI()
    {
        RebuildGUIMenu();

        DebugGUI();
    }

    public static Color EmptyColor = Color.purple;
    public static Color DefaultColor = Color.green;
    public static Color NotEmptyColor = Color.red;
    public static bool DrawDebug = true;

    private static bool _debugMenuFoldout = true;

    private void DebugGUI()
    {
        _debugMenuFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_debugMenuFoldout, "Debug menu");
        if (_debugMenuFoldout)
        {
            DrawDebug = EditorGUILayout.Toggle("Draw debug", DrawDebug);
            EditorGUILayout.Space();
            DefaultColor = EditorGUILayout.ColorField("Default Color", DefaultColor);
            EmptyColor = EditorGUILayout.ColorField("Empty Color", EmptyColor);
            NotEmptyColor = EditorGUILayout.ColorField("Not Empty Color", NotEmptyColor);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private static bool _rebuildMenuFoldout = true;
    private void RebuildGUIMenu()
    {
        _rebuildMenuFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_rebuildMenuFoldout, "Rebuild menu");

        if (_rebuildMenuFoldout)
        {

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Grid Size", GUILayout.Width(120));
            EditorGUIUtility.labelWidth = 10;
            _newSizeX = EditorGUILayout.IntField("X", (int)_newSizeX);
            _newSizeZ = EditorGUILayout.IntField("Z", (int)_newSizeZ);

            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = oldLabelWidth;

            _newSizeX = Mathf.Clamp(_newSizeX, 1, 128);
            _newSizeZ = Mathf.Clamp(_newSizeZ, 1, 128);

            if (GUILayout.Button("Rebuild Grid"))
            {
                _gridMap.RecreateGrid(_newSizeX, _newSizeZ);
            }
            if (GUILayout.Button("Bake Grid"))
            {
                if (_gridMap._grid != null)
                {
                    _gridMap.BakeGrid();
                }
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}

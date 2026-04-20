using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IntStat))]
public class IntStatDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty valueProp = property.FindPropertyRelative("_value");

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PropertyField(position, valueProp, label);

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(FloatStat))]
public class FloatStatDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty valueProp = property.FindPropertyRelative("_value");

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PropertyField(position, valueProp, label);

        EditorGUI.EndProperty();
    }
}
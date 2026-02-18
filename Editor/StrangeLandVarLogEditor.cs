#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Core.SceneEntities;

[CustomEditor(typeof(StrangeLandVarLog))]
public class StrangeLandVarLogEditor : Editor
{
    SerializedProperty bindingsProp;

    void OnEnable() => bindingsProp = serializedObject.FindProperty("bindings");

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < bindingsProp.arraySize; i++)
        {
            var elem = bindingsProp.GetArrayElementAtIndex(i);
            var targetProp = elem.FindPropertyRelative("target");
            var memberProp = elem.FindPropertyRelative("memberName");
            var labelProp  = elem.FindPropertyRelative("label");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Binding {i}", EditorStyles.boldLabel);
            if (GUILayout.Button("X", GUILayout.Width(22)))
            {
                bindingsProp.DeleteArrayElementAtIndex(i);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(targetProp);

            var target = targetProp.objectReferenceValue as Component;
            var options = GetOptions(target);

            int idx = Mathf.Max(0, Array.IndexOf(options, memberProp.stringValue));
            idx = EditorGUILayout.Popup("Member", idx, options);
            memberProp.stringValue = options.Length > 0 ? options[idx] : "";

            EditorGUILayout.PropertyField(labelProp, new GUIContent("Label (optional)"));
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("+ Add Binding")) bindingsProp.arraySize++;

        serializedObject.ApplyModifiedProperties();
    }

    static string[] GetOptions(Component c)
    {
        if (c == null) return new[] { "" };

        const BindingFlags F =
            BindingFlags.Instance | BindingFlags.Public;

        var t = c.GetType();

        // Public fields: int/float/enum
        var fields = t.GetFields(F)
            .Where(f => IsAllowed(f.FieldType))
            .Select(f => f.Name);

        // Optional: public properties (readable, non-indexed): int/float/enum
        var props = t.GetProperties(F)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0 && IsAllowed(p.PropertyType))
            .Select(p => p.Name);

        var all = fields.Concat(props).Distinct().OrderBy(x => x).ToArray();
        return all.Length == 0 ? new[] { "" } : all;
    }

    static bool IsAllowed(Type t) =>
        t == typeof(int) || t == typeof(float) || t.IsEnum;
}
#endif

using UnityEditor;
using UnityEngine;

namespace gomoru.su.MMDBlendshapeGenerator
{
    internal static class GUIUtils
    {
        public static readonly GUIStyle ObjectFieldButtonStyle = typeof(EditorStyles).GetProperty("objectFieldButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetMethod.Invoke(null, null) as GUIStyle;

        public static void DrawSelectorBox(Rect position, SerializedProperty property, Mesh mesh)
        {
            var buttonRect = position;
            buttonRect.x += buttonRect.width - EditorGUIUtility.singleLineHeight;
            buttonRect.width = EditorGUIUtility.singleLineHeight;
            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Arrow);
            buttonRect = GUIUtils.ObjectFieldButtonStyle.margin.Remove(buttonRect);
            if (GUI.Button(buttonRect, GUIContent.none, GUIStyle.none))
            {
                BlendshapeSelector.Show(buttonRect, mesh, x => { property.stringValue = x; property.serializedObject.ApplyModifiedProperties(); });
            }
            EditorGUI.PropertyField(position, property, GUIContent.none);
            if (Event.current.type == EventType.Repaint)
            {
                GUIUtils.ObjectFieldButtonStyle.Draw(buttonRect, GUIContent.none, 0, GUI.enabled, GUI.enabled && buttonRect.Contains(Event.current.mousePosition));
            }
        }
    }
}

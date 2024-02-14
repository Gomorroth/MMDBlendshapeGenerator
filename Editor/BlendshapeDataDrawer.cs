#if UNITY_EDITOR

using System.Buffers;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace gomoru.su.MMDBlendshapeGenerator
{
    [CustomPropertyDrawer(typeof(BlendshapeData))]
    internal sealed class BlendshapeDataDrawer : PropertyDrawer
    {
        private GameObject target;
        private Mesh mesh;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect1 = position;
            var rect2 = position;
            rect1.width = EditorGUIUtility.labelWidth * 1.5f;
            rect2.width -= rect1.width + 4;
            rect2.x += rect1.width + 4;

            var name = property.FindPropertyRelative(nameof(BlendshapeData.Name));
            var weight = property.FindPropertyRelative(nameof(BlendshapeData.Weight));

            if (property.serializedObject.targetObject is MMDBlendshapeGenerator generator && target != generator.Body)
            {
                target = generator.Body;
                mesh = generator.Body.GetComponent<SkinnedMeshRenderer>()?.sharedMesh;
            }

            if (mesh == null)
                EditorGUI.PropertyField(rect1, name, GUIContent.none);
            else
                GUIUtils.DrawSelectorBox(rect1, name, mesh);

            EditorGUI.PropertyField(rect2, weight, GUIContent.none);
        }
    }
}

#endif
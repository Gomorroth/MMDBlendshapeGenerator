#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VRC.SDKBase;

using Self = gomoru.su.MMDBlendshapeGenerator.MMDBlendshapeGenerator;

namespace gomoru.su.MMDBlendshapeGenerator
{
    [CustomEditor(typeof(MMDBlendshapeGenerator))]
    internal sealed class MMDBlendshapeGeneratorEditor : Editor
    {
        private SerializedProperty body;
        private SerializedProperty sources;
        private Vector2 scrollPosition;

        private static AnimationModeDriver DefaultDriver => _driver ? _driver : _driver = CreateDriver();
        private static AnimationModeDriver _driver;

        private string _previewTarget;

        private static readonly GUIContent PreviewTooltip = new GUIContent("", "Preview");

        internal void OnEnable()
        {
            sources = serializedObject.FindProperty(nameof(Self.Sources));
            body = serializedObject.FindProperty(nameof(Self.Body));

            EditorApplication.update += Update;
        }

        internal void OnDestroy()
        {
            AnimationMode.StopAnimationMode();

            EditorApplication.update -= Update;
        }

        private void Update()
        {
            if (_previewTarget != null && AnimationMode.InAnimationMode())
            {
                var self = target as Self;

                var anim = new AnimationClip();
                foreach (var data in self.Sources.FirstOrDefault(x => x.Name == _previewTarget).Datas)
                {
                    anim.SetCurve("", typeof(SkinnedMeshRenderer), $"blendShape.{data.Name}", AnimationCurve.Constant(0, 0, data.Weight * 100));
                }
                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(self.Body, anim, 0);
                AnimationMode.EndSampling();
                DestroyImmediate(anim);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var self = target as Self;

            EditorGUI.BeginChangeCheck();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight * 20));
            EditorGUI.indentLevel++;
            for( int i = 0; i < sources.arraySize; i++ )
            {
                var sourceProperty = sources.GetArrayElementAtIndex(i);
                var datas = sourceProperty.FindPropertyRelative(nameof(BlendshapeSource.Datas));
                var name = sourceProperty.FindPropertyRelative(nameof(BlendshapeSource.Name)).stringValue;
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                bool flag = EditorGUILayout.ToggleLeft(GUIContent.none, _previewTarget == name, GUILayout.Width(30));
                if (EditorGUI.EndChangeCheck())
                {
                    if (!flag)
                    {
                        _previewTarget = null;
                        AnimationMode.StopAnimationMode(); 
                    }
                    else
                    {
                        _previewTarget = name;
                        AnimationMode.StartAnimationMode();
                    }
                }

                EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), PreviewTooltip);
                EditorGUILayout.PropertyField(datas, new GUIContent(name));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.EndScrollView();


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static AnimationModeDriver CreateDriver()
        {
            var driver = CreateInstance<AnimationModeDriver>();
            driver.name = nameof(MMDBlendshapeGenerator);
            return driver;
        }
    }
}

#endif
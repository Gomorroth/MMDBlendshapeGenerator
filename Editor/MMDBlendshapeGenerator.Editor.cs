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

        private string _previewTarget;
        private SkinnedMeshRenderer _temporarySMR;

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
            if (_temporarySMR != null)
                GameObject.DestroyImmediate(_temporarySMR.gameObject);
        }

        private void Update()
        {
            if (_previewTarget != null && AnimationMode.InAnimationMode())
            {
                var self = target as Self;
                var body = self.Body.GetComponent<SkinnedMeshRenderer>();
                var anim = new AnimationClip();
                foreach (var data in self.Sources.FirstOrDefault(x => x.Name == _previewTarget).Datas)
                {
                    var weight = Mathf.Abs(data.Weight);
                    var idx = body.sharedMesh.GetBlendShapeIndex(data.Name);
                    if (idx == -1)
                        continue;
                    var original = _temporarySMR.GetBlendShapeWeight(idx);
                    if (data.Weight >= 0)
                    {
                        anim.SetCurve("", typeof(SkinnedMeshRenderer), $"blendShape.{data.Name}", AnimationCurve.Constant(0, 0, original + weight * 100));
                    }
                    else
                    {
                        anim.SetCurve("", typeof(SkinnedMeshRenderer), $"blendShape.{data.Name}", AnimationCurve.Constant(0, 0, original * (1 - weight)));
                    }
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
                        if (_temporarySMR != null)
                            GameObject.DestroyImmediate(_temporarySMR.gameObject);
                    }
                    else
                    {
                        _previewTarget = name;

                        if (_temporarySMR != null)
                            GameObject.DestroyImmediate(_temporarySMR.gameObject);
                        var self = target as Self;
                        var temp = GameObject.Instantiate(self.Body);
                        temp.SetActive(false);
                        temp.hideFlags = HideFlags.HideAndDontSave;
                        temp.transform.parent = null;

                        foreach(var x in temp.GetComponents<Component>())
                        {
                            if (x is not (SkinnedMeshRenderer or Transform))
                                GameObject.DestroyImmediate(x);
                        }
                        foreach (Transform x in temp.transform)
                        {
                            GameObject.DestroyImmediate(x);
                        }

                        _temporarySMR = temp.GetComponent<SkinnedMeshRenderer>();

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

        [MenuItem("CONTEXT/MMDBlendshapeGenerator/Add blendshapes that already exist")]
        public static void AddExistBlendshapes(MenuCommand menu)
        {
            var generator = menu.context as MMDBlendshapeGenerator;
            var smr = generator.GetComponent<SkinnedMeshRenderer>();

            Undo.RecordObject(generator, "Add blendshapes that already exist");

            foreach (var blendshape in smr.EnumerateBlendshapes().Where(x => x.Weight != 0))
            {
                foreach (var source in generator.Sources)
                {
                    source.Datas.Add(new() { Name = blendshape.Name, Weight = blendshape.Weight });
                }
            }

            EditorUtility.SetDirty(generator);
        }
    }

    internal static class SkinnedMeshRendererExt
    {
        public static IEnumerable<(string Name, int Index, float Weight)> EnumerateBlendshapes(this SkinnedMeshRenderer smr)
        {
            var mesh = smr.sharedMesh;
            if (!mesh)
                yield break;
            var count = mesh.blendShapeCount;

            for (int i = 0; i < count; i++)
            {
                var name = mesh.GetBlendShapeName(i);
                var weight = smr.GetBlendShapeWeight(i);
                yield return (name, i, weight);
            }
        }
    }
}

#endif
#if UNITY_EDITOR

using System;
using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace gomoru.su.MMDBlendshapeGenerator
{
    internal sealed class BlendshapeSelector : EditorWindow
    {
        public Mesh TargetMesh { get; set; }

        private string[] _blendshapes;
        private Vector2 _scrollPosition;
        private string _searchText;

        public Action<string> Callback;

        public (DateTime Time, string Selected) _lastSelect;

        private bool _isOpen = false;

        public static void Show(Rect position, Mesh targetMesh, Action<string> callback)
        {
            if (targetMesh == null)
                return;
            var window = CreateInstance<BlendshapeSelector>();
            window.titleContent = new GUIContent($"Blendshape Selector: {targetMesh.name}");
            window.TargetMesh = targetMesh;
            window.Callback = callback;
            window.position = new Rect(GUIUtility.GUIToScreenPoint(position.center), window.position.size);
            window.ShowAuxWindow();
        }

        internal void OnGUI()
        {
            if (_blendshapes == null)
                _blendshapes = Enumerable.Range(0, TargetMesh.blendShapeCount).Select(i => TargetMesh.GetBlendShapeName(i)).ToArray();

            _searchText = EditorGUILayout.TextField(_searchText);


            if ((_isOpen = EditorGUILayout.Foldout(_isOpen, TargetMesh.name)) == false)
                return;


            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorGUI.indentLevel++;


            foreach (ref readonly var x in _blendshapes.AsSpan())
            {
                if (!string.IsNullOrEmpty(_searchText))
                {
                    if (x.IndexOf(_searchText) == -1)
                        continue;
                }
                var rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
                if (GUI.Button(rect, x, _lastSelect.Selected == x ? EditorStyles.linkLabel : EditorStyles.label))
                {
                    if (_lastSelect.Selected == x && DateTime.UtcNow - _lastSelect.Time < TimeSpan.FromMilliseconds(500))
                    {
                        Callback(x);
                        Close();
                    }
                    else
                    {
                        _lastSelect = (DateTime.UtcNow, x);
                    }
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.EndScrollView();
        }

        private struct Blendshape
        {
            public string Name;
            public int Index;
        }
    }
}

#endif
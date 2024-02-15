#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace gomoru.su.MMDBlendshapeGenerator
{
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
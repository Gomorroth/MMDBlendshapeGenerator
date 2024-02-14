


using System;
using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using gomoru.su.MMDBlendshapeGenerator;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

[assembly: ExportsPlugin(typeof(MMDBlendshapeGeneratorCore))]

namespace gomoru.su.MMDBlendshapeGenerator
{
    public sealed class MMDBlendshapeGeneratorCore : Plugin<MMDBlendshapeGeneratorCore>
    {
        [MenuItem("Test/ASduhuyiqhweq")]
        public static void Mes12h()
        {
            var mesh = GameObject.Find("Body").GetComponent<SkinnedMeshRenderer>().sharedMesh;
            using var data = mesh.GetBlendShapeBuffer();
            var range = mesh.GetBlendShapeBufferRange(0);
            Debug.Log(data.stride);
            Debug.Log(Unsafe.SizeOf<Vector3>());
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 20)]
        public struct Blendshape
        {
            public uint Index;
            public Vector3 Position;
            public Vector3 Normal;
            public Vector3 Tangent;
        }

        protected override void Configure()
        {
            InPhase(BuildPhase.Generating).Run("Generate MMD Blendshape", context =>
            {
                if (context.AvatarRootObject.GetComponentInChildren<MMDBlendshapeGenerator>() is not { Body: { } body, Sources: { Length: > 0 } sources } generator)
                    return;

                var smr = body.GetComponentInChildren<SkinnedMeshRenderer>();
                var origmesh = smr.sharedMesh;
                var mesh = Object.Instantiate(origmesh);
                AssetDatabase.AddObjectToAsset(mesh, context.AssetContainer);
                mesh.ClearBlendShapes();


                int vertexCount = mesh.vertexCount;

                var verticies = new Vector3[vertexCount];
                var normals = new Vector3[vertexCount];
                var tangents = new Vector3[vertexCount];
                var verticies2 = new Vector3[vertexCount];
                var normals2 = new Vector3[vertexCount];
                var tangents2 = new Vector3[vertexCount];

                for(int i = 0; i < origmesh.blendShapeCount; i++)
                {
                    var name = origmesh.GetBlendShapeName(i);
                    var weight = origmesh.GetBlendShapeFrameWeight(i, 0);

                    if (sources.FirstOrDefault(x => x.Name == name) is { Datas: { Count: > 0 } } source)
                    {
                        foreach (var data in source.Datas)
                        {
                            var targetIdx = origmesh.GetBlendShapeIndex(data.Name);
                            if (targetIdx == -1)
                                continue;

                            origmesh.GetBlendShapeFrameVertices(targetIdx, 0, verticies2, normals2, tangents2);

                            for (int i2 = 0; i2 < vertexCount; i2++)
                            {
                                verticies[i2] = Vector3.Lerp(verticies[i2], verticies[i2] + verticies2[i2], data.Weight);
                                normals[i2] = Vector3.Lerp(normals[i2], normals[i2] + normals2[i2], data.Weight);
                                tangents[i2] = Vector3.Lerp(tangents[i2], tangents[i2] + tangents2[i2], data.Weight);
                            }
                        }
                    }
                    else
                    {
                        origmesh.GetBlendShapeFrameVertices(i, 0, verticies, normals, tangents);
                    }

                    mesh.AddBlendShapeFrame(name, weight, verticies, normals, tangents);

                    verticies.Clear();
                    normals.Clear();
                    tangents.Clear();
                }

                smr.sharedMesh = mesh;

                GameObject.DestroyImmediate(generator);
            });
        }
    }
}
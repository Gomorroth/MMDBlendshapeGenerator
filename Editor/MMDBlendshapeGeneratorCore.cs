using System.Linq;
using gomoru.su.MMDBlendshapeGenerator;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: ExportsPlugin(typeof(MMDBlendshapeGeneratorCore))]

namespace gomoru.su.MMDBlendshapeGenerator
{
    public sealed class MMDBlendshapeGeneratorCore : Plugin<MMDBlendshapeGeneratorCore>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Generating).Run("Generate MMD Blendshape", context =>
            {
                if (context.AvatarRootObject.GetComponentInChildren<MMDBlendshapeGenerator>() is not { Body: { } body, Sources: { Length: > 0 } sources } generator || !generator.enabled)
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
                    var shapeWeight = origmesh.GetBlendShapeFrameWeight(i, 0);

                    if (sources.FirstOrDefault(x => x.Name == name) is { Datas: { Count: > 0 } datas } source && !datas.All(x => x.Weight == 0))
                    {
                        foreach (var data in source.Datas)
                        {
                            if (data.Weight == 0)
                                continue;
                                
                            var targetIdx = origmesh.GetBlendShapeIndex(data.Name);
                            if (targetIdx == -1)
                                continue;

                            origmesh.GetBlendShapeFrameVertices(targetIdx, 0, verticies2, normals2, tangents2);

                            var origweight = smr.GetBlendShapeWeight(targetIdx) / 100;
                            var weight = Mathf.Abs(data.Weight);
                            float isCancel = data.Weight < 0 ? -origweight : 1;

                            for (int i2 = 0; i2 < vertexCount; i2++)
                            {
                                verticies[i2] = Vector3.Lerp(verticies[i2], verticies[i2] + verticies2[i2] * isCancel, weight);
                                normals[i2] = Vector3.Lerp(normals[i2], normals[i2] + normals2[i2] * isCancel, weight);
                                tangents[i2] = Vector3.Lerp(tangents[i2], tangents[i2] + tangents2[i2] * isCancel, weight);
                            }
                        }
                    }
                    else
                    {
                        origmesh.GetBlendShapeFrameVertices(i, 0, verticies, normals, tangents);
                    }

                    mesh.AddBlendShapeFrame(name, shapeWeight, verticies, normals, tangents);

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
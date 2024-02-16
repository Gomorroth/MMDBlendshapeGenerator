using System.Linq;
using UnityEngine;
using VRC.SDKBase;

namespace gomoru.su.MMDBlendshapeGenerator
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public sealed class MMDBlendshapeGenerator : MonoBehaviour, IEditorOnly
    {
        public GameObject Body;

        [SerializeField]
        private bool Initialized = false;
        
        public BlendshapeSource[] Sources = new[]
        {
            S("あ"),
            S("い"),
            S("う"),
            S("え"),
            S("お"),
            S("にやり"),
            S("∧"),
            S("ワ"),
            S("ω"),
            S("▲"),
            S("口角上げ"),
            S("口角下げ"),
            S("口横広げ"),
            S("まばたき"),
            S("笑い"),
            S("はぅ"),
            S("瞳小"),
            S("ｳｨﾝｸ２右"),
            S("ウィンク２"),
            S("ウィンク"),
            S("ウィンク右"),
            S("なごみ"),
            S("じと目"),
            S("びっくり"),
            S("ｷﾘｯ"),
            S("はぁと"),
            S("星目"),
            S("にこり"),
            S("上"),
            S("下"),
            S("真面目"),
            S("困る"),
            S("怒り"),
            S("前"),
            S("照れ"),
            S("にやり２"),
            S("ん"),
            S("あ2"),
            S("恐ろしい子！"),
            S("歯無し下"),
            S("涙"),
        };

        private static BlendshapeSource S(string name) => new BlendshapeSource(name);

        public void Initialize()
        {
            if (Body == null)
                Body = gameObject;

            if (Initialized)
                return;

            Initialized = true;

            var smr = GetComponent<SkinnedMeshRenderer>();
            var mesh = smr.sharedMesh;
            if (mesh == null)
                return;

            int count = mesh.blendShapeCount;
            for(int i = 0; i < count; i++)
            {
                var name = mesh.GetBlendShapeName(i);
                var weight = smr.GetBlendShapeWeight(i);
                if (weight == 0)
                    continue;

                foreach(var source in Sources)
                {
                    if (!source.Datas.Any(x => x.Name == name))
                        source.Datas.Add(new() { Name = name, Weight = 0 });
                }
            }

            foreach (var source in Sources)
            {
                int idx = mesh.GetBlendShapeIndex(source.Name);
                if (idx == -1)
                    continue;

                if (!source.Datas.Any(x => x.Name == name)) 
                    source.Datas.Add(new() { Name = source.Name, Weight = 1 });
            }
        }

        public void Start() => Initialize();
        public void Reset() => Initialize();
    }
}
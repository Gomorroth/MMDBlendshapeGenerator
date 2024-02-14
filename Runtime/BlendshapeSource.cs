
using System;
using System.Collections.Generic;

namespace gomoru.su.MMDBlendshapeGenerator
{
    [Serializable]
    public sealed class BlendshapeSource
    {
        public string Name;
        public List<BlendshapeData> Datas;

        public BlendshapeSource(string name)
        {
            Name = name;
            Datas = new List<BlendshapeData>();
        }
    }
}
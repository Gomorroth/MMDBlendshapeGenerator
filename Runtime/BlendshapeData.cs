﻿
using System;
using UnityEngine;

namespace gomoru.su.MMDBlendshapeGenerator
{
    [Serializable]
    public struct BlendshapeData
    {
        public string Name;
        [Range(-1, 1)]
        public float Weight;
    }
}
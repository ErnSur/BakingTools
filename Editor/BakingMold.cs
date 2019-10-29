using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace QuickEye.BakingTools
{
    [Serializable]
    public class BakingMold
    {
        public string name;
        public StaticFlagPropertyOverride staticFlags;
        public LightmapScalePropertyOverride lightmapScale;
        public StitchSeamsPropertyOverride stitchSeams;
        public ShadowCastingModePropertyOverride castShadows;
        public NavMeshAreaPropertyOverride navMeshArea;
    }
}

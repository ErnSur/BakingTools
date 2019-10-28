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
        public ToggleableEditorFlags staticFlags;
        public ToggleableInt lightmapScale;
        public ToggleableBool stitchSeams;
        public ToggleableShadowCastingMode castShadows;
    }
}

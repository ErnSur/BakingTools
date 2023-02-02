using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace QuickEye.BakingTools
{
    [Serializable]
    public class BakingPreset
    {
        public string name;
        public StaticFlagPropertyOverride staticFlags;
        public LightmapScalePropertyOverride lightmapScale;
        public StitchSeamsPropertyOverride stitchSeams;
        public ShadowCastingModePropertyOverride castShadows;
        public NavMeshAreaPropertyOverride navMeshArea;
        
        public void ApplyPreset(GameObject go, bool includeChildren)
        {
            Undo.RecordObject(go, "Apply baking preset");

            staticFlags.TryApply(go);
            if (go.TryGetComponent<MeshRenderer>(out var renderer))
            {
                castShadows.TryApply(renderer);
                lightmapScale.TryApply(renderer);
                stitchSeams.TryApply(renderer);
                navMeshArea.TryApply(renderer);
            }

            if (!includeChildren)
                return;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i).gameObject;
                ApplyPreset(child, true);
            }
        }
    }
}

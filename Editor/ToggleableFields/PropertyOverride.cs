using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace QuickEye.BakingTools
{
    public abstract class PropertyOverride<TValue, TInput> : ToglabbleParameter<TValue>
    {
        public bool TryApply(TInput input)
        {
            if (!isOn)
            {
                return false;
            }
            //Debug.Log($"Applying {typeof(TValue).Name} value:{value}");

            Apply(input);
            return true;
        }

        public abstract void Apply(TInput input);
    }

    [Serializable]
    public class StaticFlagPropertyOverride : PropertyOverride<StaticEditorFlags, GameObject>
    {
        public override void Apply(GameObject obj)
        {
            Undo.RecordObject(obj, "Set static flags");
            GameObjectUtility.SetStaticEditorFlags(obj, value);
        }
    }

    [Serializable]
    public class LightmapScalePropertyOverride : PropertyOverride<int, MeshRenderer>
    {
        public override void Apply(MeshRenderer renderer)
        {
            using (var so = new SerializedObject(renderer))
            {
                so.FindProperty("m_ScaleInLightmap").floatValue = value;
                so.ApplyModifiedProperties();
            }
        }
    }

    [Serializable]
    public class StitchSeamsPropertyOverride : PropertyOverride<bool, MeshRenderer>
    {
        public override void Apply(MeshRenderer renderer)
        {
            using (var so = new SerializedObject(renderer))
            {
                so.FindProperty("m_StitchLightmapSeams").boolValue = value;
                so.ApplyModifiedProperties();
            }
        }
    }

    [Serializable]
    public class ShadowCastingModePropertyOverride : PropertyOverride<ShadowCastingMode, MeshRenderer>
    {
        public override void Apply(MeshRenderer renderer)
        {
            using (var so = new SerializedObject(renderer))
            {
                so.FindProperty("m_CastShadows").enumValueIndex = (int)value;
                so.ApplyModifiedProperties();
            }
        }
    }

    [Serializable]
    public class NavMeshAreaPropertyOverride : PropertyOverride<NavMeshAreaPropertyOverride.NavMeshArea, MeshRenderer>
    {
        public override void Apply(MeshRenderer renderer)
        {
            Undo.RecordObject(renderer.gameObject, "Update NavMeshArea");
            GameObjectUtility.SetNavMeshArea(renderer.gameObject, (int)value);
        }

        public enum NavMeshArea
        {
            Walkable = 0,
            NotWalkable = 1,
            Jump = 2
        }
    }
}

using System;
using UnityEditor;
using UnityEngine.Rendering;

namespace QuickEye.BakingTools
{
    public class ToglabbleParameter<T>
    {
        public bool isOn;
        public T value;

        public static implicit operator T(ToglabbleParameter<T> p) => p.value;
    }

    [Serializable]
    public class ToggleableInt : ToglabbleParameter<int> { }
    [Serializable]
    public class ToggleableEditorFlags : ToglabbleParameter<StaticEditorFlags> { }
    [Serializable]
    public class ToggleableBool : ToglabbleParameter<bool> { }
    [Serializable]
    public class ToggleableShadowCastingMode : ToglabbleParameter<ShadowCastingMode> { }
}

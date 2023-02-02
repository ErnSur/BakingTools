using UnityEngine;

namespace QuickEye.BakingTools
{
    //Replace with PackageSettingsRepository.GetSettingsPath?
    [Icon("unityeditor/presets/preset icon")]
    [CreateAssetMenu(menuName = "QuickEye/Baking Preset Library")]
    public class BakingPresetLibrary : ScriptableObject
    {
        public BakingPreset[] presets;
    }
}

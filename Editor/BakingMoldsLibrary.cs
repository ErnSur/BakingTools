using UnityEngine;

namespace QuickEye.BakingTools
{
    //Replace with PackageSettingsRepository.GetSettingsPath?
    [CreateAssetMenu(menuName = "QuickEye/Baking Molds Library")]
    public class BakingMoldsLibrary : ScriptableObject
    {
        public BakingMold[] molds;
    }
}

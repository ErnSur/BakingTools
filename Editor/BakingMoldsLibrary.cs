using UnityEngine;

namespace QuickEye.BakingTools
{
    [CreateAssetMenu(menuName = "QuickEye/Baking Molds Library")]
    public class BakingMoldsLibrary : ScriptableObject
    {
        public BakingMold[] molds;
    }
}

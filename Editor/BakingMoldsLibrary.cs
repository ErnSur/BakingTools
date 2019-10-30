using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace QuickEye.BakingTools
{
    //Replace with PackageSettingsRepository.GetSettingsPath?
    [CreateAssetMenu(menuName = "QuickEye/Baking Molds Library")]
    public class BakingMoldsLibrary : ScriptableObject
    {
        public BakingMold[] molds;

        public void SaveToSettings()
        {
            ChefToolsSettings.settings.Set("molds", molds, SettingsScope.User);
            
            ChefToolsSettings.settings.Save();
        }
    }

    public static class ChefToolsSettings
    {
        public const string packageName = "BakingTools";
        public const string bakingMoldsSettingsName = "baking-molds";

        internal static Settings settings;

        static ChefToolsSettings()
        {
            settings = new Settings(packageName);
        }
    }
}

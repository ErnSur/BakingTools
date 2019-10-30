using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    //We can use UI Elements Default Inspector in 2019.3+
    [CustomEditor(typeof(BakingMoldsLibrary))]
    public class BakingMoldsLibraryEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var molds = new PropertyField(serializedObject.FindProperty("molds"));
            root.Add(molds);

            var openToolsButton = new Button(OpenTools);
            openToolsButton.text = "Open Tools";
            root.Add(openToolsButton);

            var saveButton = new Button((target as BakingMoldsLibrary).SaveToSettings);
            saveButton.text = "Save to settings";
            root.Add(saveButton);

            var printButton = new Button(Print);
            printButton.text = "Prints";
            root.Add(printButton);

            return root;
        }

        private void OpenTools()
        {
            ChefTools.OpenWindow();
        }

        private void Print()
        {
            var molds = ChefToolsSettings.settings.Get<BakingMold[]>("molds", SettingsScope.User);
            Debug.Log(molds.Length);
        }
    }
}

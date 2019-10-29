using UnityEditor;
using UnityEditor.UIElements;
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
            
            return root;
        }

        private void OpenTools()
        {
            ChefTools.OpenWindow(target as BakingMoldsLibrary);
        }
    }
}

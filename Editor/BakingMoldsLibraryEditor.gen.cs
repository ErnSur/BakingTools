// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.2
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    partial class BakingMoldsLibraryEditor
    {
        public const string UxmlPath = "Packages/com.quickeye.bakingtools/Editor/BakingMoldsLibraryEditor.uxml";
        private UnityEditor.UIElements.PropertyField _detailsField;
        private ListView _moldsListView;
    
        protected void AssignQueryResults(VisualElement root)
        {
            _detailsField = root.Q<UnityEditor.UIElements.PropertyField>("details-field");
            _moldsListView = root.Q<ListView>("molds-list-view");
        }
    }
}

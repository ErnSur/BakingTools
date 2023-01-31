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
        private VisualElement _targetDetailsContainer;
        private ListView _moldsListView;
    
        protected void AssignQueryResults(VisualElement root)
        {
            _targetDetailsContainer = root.Q<VisualElement>("target-details-container");
            _moldsListView = root.Q<ListView>("molds-list-view");
        }
    }
}

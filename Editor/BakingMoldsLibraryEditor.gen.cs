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
        private VisualElement _transitionContainer;
        private UnityEditor.UIElements.ToolbarButton _backButton;
        private ListView _moldsListView;
        private VisualElement _moldDetails;
    
        protected void AssignQueryResults(VisualElement root)
        {
            _transitionContainer = root.Q<VisualElement>("transition-container");
            _backButton = root.Q<UnityEditor.UIElements.ToolbarButton>("back-button");
            _moldsListView = root.Q<ListView>("molds-list-view");
            _moldDetails = root.Q<VisualElement>("mold-details");
        }
    }
}

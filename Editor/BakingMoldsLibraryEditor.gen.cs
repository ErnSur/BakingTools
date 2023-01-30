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
        private UnityEditor.UIElements.ToolbarBreadcrumbs _breadcrumbs;
        private VisualElement _transitionContainer;
        private ListView _moldsListView;
        private VisualElement _moldDetails;
        private VisualElement _arrowButton;
    
        protected void AssignQueryResults(VisualElement root)
        {
            _breadcrumbs = root.Q<UnityEditor.UIElements.ToolbarBreadcrumbs>("breadcrumbs");
            _transitionContainer = root.Q<VisualElement>("transition-container");
            _moldsListView = root.Q<ListView>("molds-list-view");
            _moldDetails = root.Q<VisualElement>("mold-details");
            _arrowButton = root.Q<VisualElement>("arrow-button");
        }
    }
}

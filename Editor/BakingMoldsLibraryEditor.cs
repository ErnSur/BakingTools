using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    [CustomEditor(typeof(BakingMoldsLibrary))]
    public partial class BakingMoldsLibraryEditor : Editor
    {
        [SerializeField]
        VisualTreeAsset uiTemplate;

        [SerializeField]
        int serializedSelectionIndex;

        SerializedProperty _moldListProperty;
        SerializedProperty _lastSelectedProperty;

        SerializedProperty SelectedProperty =>
            HasSelection ? _moldListProperty.GetArrayElementAtIndex(_moldsListView.selectedIndex) : null;

        bool HasSelection => _moldsListView.selectedIndex != -1;
        public override bool UseDefaultMargins() => false;

        void OnEnable()
        {
            _moldListProperty = serializedObject.FindProperty($"molds");
        }

        void OnDestroy()
        {
            serializedSelectionIndex = _moldsListView.selectedIndex;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = uiTemplate.CloneTree();
            AssignQueryResults(root);
            SetupListView();

            return root;
        }

        void SetupDetailsView()
        {
            var isExpanded = _lastSelectedProperty?.isExpanded ?? false;
            _lastSelectedProperty = SelectedProperty;
            _lastSelectedProperty.isExpanded = isExpanded;
            _detailsField.BindProperty(SelectedProperty);
        }

        void SetupListView()
        {
            _moldsListView.makeItem = CreateListViewElement;
            _moldsListView.bindingPath = nameof(BakingMoldsLibrary.molds);
            _moldsListView.selectionChanged += _ =>
            {
                //serializedSelectionIndex = _moldsListView.selectedIndex;
                SetupDetailsView();
            };
            _moldsListView.RegisterCallback<AttachToPanelEvent>(e =>
            {
                _moldsListView.selectedIndex = serializedSelectionIndex;
            });
            _moldsListView.itemsChosen += items => { ApplyPreset(); };
            var footer=_moldsListView.Q(classes: BaseListView.footerUssClassName);
            footer.Add(new Button()
            {
                text = "Apply",
                style = {width = 50,fontSize = 13, unityFontStyleAndWeight = FontStyle.Normal}
            });
        }

        void ApplyPreset()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                sceneView.ShowNotification(new GUIContent($"Apply preset"), 0.2f);
                //sceneView.Focus();
            }
        }

        VisualElement CreateListViewElement()
        {
            var root = new BindableElement();
            root.style.height = _moldsListView.fixedItemHeight;
            root.AddToClassList("mold-list__item");
            var label = new Label();
            label.name = "mold-list__item__name";
            label.bindingPath = nameof(BakingMold.name);
            root.Add(label);

            var applyButton = new Button();
            applyButton.text = "Apply";
            /*applyButton.AddManipulator(new Clickable(() =>
            {
                ApplyPreset();
                //SetupDetailsView();
                //_transitionContainer.AddToClassList("details-active");
            }));*/
            applyButton.name = "mold-list__item__apply-button";
            root.Add(applyButton);

            return root;
        }
    }
}
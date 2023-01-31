using System;
using System.Linq;
using Unity.Properties;
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

        [CreateProperty]
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

        void SetupListView()
        {
            _moldsListView.makeItem = ()=>  new PropertyField(){name = "preset-property"};
            _moldsListView.bindingPath = nameof(BakingMoldsLibrary.molds);
            _moldsListView.selectionChanged += _ =>
            {
                //serializedSelectionIndex = _moldsListView.selectedIndex;
            };
            _moldsListView.RegisterCallback<AttachToPanelEvent>(e =>
            {
                _moldsListView.selectedIndex = serializedSelectionIndex;
            });
            _moldsListView.itemsChosen += items => { ApplyPreset(); };
            _moldsListView.bindItem += (element, i) =>
            {
                ((PropertyField)element).BindProperty(_moldListProperty.GetArrayElementAtIndex(i));
                var foldout = element.Q<Foldout>();
                foldout.Q(className: Foldout.inputUssClassName).pickingMode = PickingMode.Ignore;
                foldout.Q(className: Foldout.toggleUssClassName).pickingMode = PickingMode.Ignore;
                //foldout.Q(className: Foldout.textUssClassName).pickingMode = PickingMode.Ignore;
                foldout.Q(className: Foldout.checkmarkUssClassName).pickingMode = PickingMode.Position;
                var applyButton = new ToolbarButton();
                applyButton.text = "Apply";
                /*applyButton.AddManipulator(new Clickable(() =>
                {
                    ApplyPreset();
                    //SetupDetailsView();
                    //_transitionContainer.AddToClassList("details-active");
                }));*/
                applyButton.name = "mold-list__item__apply-button";
                foldout.Q(className:Foldout.inputUssClassName).Add(applyButton);
                //root.Add(applyButton);
            };
            

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
            var root = new PropertyField();

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
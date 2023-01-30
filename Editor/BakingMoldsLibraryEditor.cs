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
            SetupBreadcrumbs();
            SetupListView();
            return root;
        }

        void SetupDetailsView()
        {
            _moldDetails.Clear();
            var propertyField = new PropertyField(SelectedProperty);
            propertyField.Bind(serializedObject);
            _moldDetails.Add(propertyField);
        }

        void SetupBreadcrumbs()
        {
            // _breadcrumbs.PushItem("Molds", () =>
            // {
            //     _transitionContainer.RemoveFromClassList("details-active");
            //     if (_breadcrumbs.childCount > 1)
            //         _breadcrumbs.PopItem();
            // });
            _backButton.clicked += () =>
            {
                _transitionContainer.RemoveFromClassList("details-active");
            };
        }

        void SetupListView()
        {
            _moldsListView.makeItem = CreateListViewElement;
            _moldsListView.bindingPath = nameof(BakingMoldsLibrary.molds);
            _moldsListView.RegisterCallback<AttachToPanelEvent>(e =>
            {
                _moldsListView.selectedIndex = serializedSelectionIndex;
            });
            _moldsListView.itemsChosen += items =>
            {
                var sceneView = SceneView.lastActiveSceneView;
                if (sceneView != null)
                    sceneView.ShowNotification(new GUIContent($"Apply {SelectedProperty.displayName}"));
            };
        }

        VisualElement CreateListViewElement()
        {
            var root = new BindableElement();
            root.style.height = _moldsListView.fixedItemHeight;
            root.AddToClassList("mold-list__item");
            var label = new Label();
            label.bindingPath = nameof(BakingMold.name);
            root.Add(label);

            var button = new VisualElement();
            button.RegisterCallback<ClickEvent>(e =>
            {
                var elementName = SelectedProperty.displayName;
               // _breadcrumbs.PushItem(elementName);
                SetupDetailsView();
                _transitionContainer.AddToClassList("details-active");
            });
            button.name = "arrow-button";
            root.Add(button);

            return root;
        }

        public override bool HasPreviewGUI() => true;

        public override GUIContent GetPreviewTitle() => HasSelection
            ? new GUIContent(SelectedProperty.displayName)
            : new GUIContent("nothing");

        public override void OnPreviewSettings()
        {
            if (HasSelection && GUILayout.Button("Apply", EditorStyles.toolbarButton))
            {
            }

            if (GUILayout.Button("+", EditorStyles.toolbarButton))
            {
                _moldListProperty.InsertArrayElementAtIndex(HasSelection ? _moldsListView.selectedIndex : 0);
                serializedObject.ApplyModifiedProperties();
            }

            if (HasSelection && GUILayout.Button("-", EditorStyles.toolbarButton))
            {
                _moldListProperty.DeleteArrayElementAtIndex(_moldsListView.selectedIndex);
                serializedObject.ApplyModifiedProperties();
                _moldsListView.selectedIndex = Math.Max(_moldsListView.selectedIndex - 1, 0);
            }

            base.OnPreviewSettings();
        }

        public override void DrawPreview(Rect previewArea)
        {
            if (_moldsListView.selectedIndex == -1)
                return;
            previewArea.xMin += 5;
            var prop = _moldListProperty.GetArrayElementAtIndex(_moldsListView.selectedIndex);
            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                DrawPropertyChildren(previewArea, prop);
                if (changeCheck.changed)
                    serializedObject.ApplyModifiedProperties();
            }
        }

        public static void DrawPropertyChildren(Rect position, SerializedProperty prop)
        {
            var endProperty = prop.GetEndProperty();
            var childrenDepth = prop.depth + 1;
            while (prop.NextVisible(true) && !SerializedProperty.EqualContents(prop, endProperty))
            {
                if (prop.depth != childrenDepth)
                    continue;
                position.height = EditorGUI.GetPropertyHeight(prop);
                EditorGUI.PropertyField(position, prop, true);
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    [CustomEditor(typeof(BakingPresetLibrary))]
    public partial class BakingPresetLibraryEditor : Editor
    {
        [SerializeField]
        VisualTreeAsset uiTemplate;

        [SerializeField]
        int serializedSelectionIndex;

        SerializedProperty _presetsProperty;

        void OnEnable()
        {
            _presetsProperty = serializedObject.FindProperty(nameof(BakingPresetLibrary.presets));
        }

        void OnDestroy()
        {
            //TODO: remove this, causes an error when object is selected in object picker
            if (_presetListView != null)
                serializedSelectionIndex = _presetListView.selectedIndex;
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
            _presetListView.makeItem = () =>
            {
                var item = new PresetListItem();
                item.ApplyButtonClicked += ApplyPreset;
                return item;
            };
            _presetListView.bindItem += (element, i) =>
            {
                try
                {
                    ((PresetListItem)element).Bind(_presetsProperty.GetArrayElementAtIndex(i));
                }
                catch (Exception e)
                {
                    Debug.Log($"i:{i}, PropSize: {_presetsProperty.arraySize}");
                    throw;
                }
            };
            _presetListView.unbindItem += (element, i) => { ((PresetListItem)element).Unbind(); };
            _presetListView.itemsChosen += items => { ApplyPreset(); };
            _presetListView.bindingPath = nameof(BakingPresetLibrary.presets);
            _presetListView.RegisterCallback<AttachToPanelEvent>(e =>
            {
                _presetListView.selectedIndex = serializedSelectionIndex;
            });
            //_presetListView.ad
        }

        void ApplyPreset()
        {
                
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
                return;
            if (!IsGameObjectSelected())
            {
                sceneView.ShowNotification(new GUIContent("Select game object first"));
                sceneView.Focus();
                return;
            }

            var preset = ((BakingPresetLibrary)target).presets[_presetListView.selectedIndex];
            foreach (var gameObject in Selection.gameObjects)
            {
                preset.ApplyPreset(gameObject, false);
                EditorGUIUtility.PingObject(gameObject);
            }

            var message = Selection.gameObjects.Length > 1
                ? $"Applied {preset.name} to {Selection.gameObjects.Length} objects"
                : $"Applied {preset.name} to {Selection.activeGameObject.name}";
            sceneView.ShowNotification(new GUIContent(message), 0.4f);
            //sceneView.Focus();
        }

        bool IsGameObjectSelected() => Selection.gameObjects.Length > 0;

        public override bool UseDefaultMargins() => false;

        class PresetListItem : VisualElement
        {
            public event Action ApplyButtonClicked;
            readonly PropertyField _propertyField;
            bool _initialized;

            public PresetListItem()
            {
                Add(_propertyField = new PropertyField() { name = "preset-property" });
            }

            public void Bind(SerializedProperty property)
            {
                _propertyField.BindProperty(property);

                if (!_initialized)
                    Initialize();
            }

            void Initialize()
            {
                UpdateFoldoutInputArea();
                AddApplyButton();
                _initialized = true;
            }

            public void Unbind()
            {
                _propertyField.Unbind();
            }

            void UpdateFoldoutInputArea()
            {
                var foldout = _propertyField.Q<Foldout>();
                foldout.Q(className: Foldout.inputUssClassName).pickingMode = PickingMode.Ignore;
                foldout.Q(className: Foldout.toggleUssClassName).pickingMode = PickingMode.Ignore;
                foldout.Q(className: Foldout.checkmarkUssClassName).pickingMode = PickingMode.Position;
            }

            void AddApplyButton()
            {
                var applyButton = new ToolbarButton(() => ApplyButtonClicked?.Invoke());
                applyButton.text = "Apply";
                applyButton.name = "preset-list__item__apply-button";
                _propertyField.Q<Foldout>().Q(className: Foldout.inputUssClassName).Add(applyButton);
            }
        }
    }
}
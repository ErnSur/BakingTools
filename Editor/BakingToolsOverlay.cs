using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    [Icon("Preset.Context")]
    [Overlay(typeof(SceneView), "Baking Presets", defaultDisplay = true)]
    class BakingToolsOverlay : Overlay
    {
        readonly VisualElement view = new VisualElement()
        {
            style = { width = 250 }
        };

        static BakingPresetLibrary Library
        {
            get => Settings.instance.LastLibraryOpened;
            set => Settings.instance.LastLibraryOpened = value;
        }

        public override VisualElement CreatePanelContent()
        {
            RebuildView();
            return view;
        }

        VisualElement CreateLibrarySelectionView()
        {
            var root = new VisualElement();

            root.style.alignContent = Align.Center;
            root.style.justifyContent = Justify.Center;

            var label = new Label("Select Baking Preset Library or create a new one.");
            var selectLibraryButton = new Button(SelectLibrary) { text = "Select Library" };
            var createButton = new Button(CreateLibrary) { text = "Create Library" };
            root.Add(label);
            root.Add(selectLibraryButton);
            root.Add(createButton);
            return root;
        }

        VisualElement CreatePresetLibraryView()
        {
            var inspectorElement = new InspectorElement(new SerializedObject(Library));
            inspectorElement.Q<Toolbar>().Add(new Button(){text = "Change"});
            return inspectorElement;
        }

        void CreateLibrary()
        {
            var tempObject = ScriptableObject.CreateInstance<BakingPresetLibrary>();
            var createAssetAction = ScriptableObject.CreateInstance<UserCreatedLibraryAction>();
            createAssetAction.overlay = this;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                createAssetAction,
                "Assets/New Baking Preset Library.asset",
                EditorGUIUtility.GetIconForObject(MonoScript.FromScriptableObject(tempObject)),
                null);
            Object.DestroyImmediate(tempObject);
        }

        void SelectLibrary()
        {
            SearchService.ShowObjectPicker((selectedObject, canceled) =>
                {
                    if (canceled)
                        return;
                    Library = (BakingPresetLibrary)selectedObject;
                    RebuildView();
                },
                null, null, null, typeof(BakingPresetLibrary));
        }

        void RebuildView()
        {
            view.Clear();
            if (Library == null)
                view.Add(CreateLibrarySelectionView());
            else
                view.Add(CreatePresetLibraryView());
        }

        [FilePath("Library/BakingPresetOverlaySettings", FilePathAttribute.Location.ProjectFolder)]
        class Settings : ScriptableSingleton<Settings>
        {
            [SerializeField]
            BakingPresetLibrary lastLibraryOpened2;

            public BakingPresetLibrary LastLibraryOpened
            {
                get => lastLibraryOpened2;
                set
                {
                    lastLibraryOpened2 = value;
                    Save(true);
                }
            }
        }

        class UserCreatedLibraryAction : EndNameEditAction
        {
            public BakingToolsOverlay overlay;

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var newLibrary = CreateInstance<BakingPresetLibrary>();
                AssetDatabase.CreateAsset(newLibrary, pathName);
                Settings.instance.LastLibraryOpened = newLibrary;
                overlay.RebuildView();
            }
        }
    }
}
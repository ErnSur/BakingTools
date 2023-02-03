using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Search;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    [Icon("Preset.Context")]
    [Overlay(typeof(SceneView), "Baking Presets", defaultDisplay = true)]
    class BakingToolsOverlay : Overlay
    {
        VisualElement view;
        BakingPresetLibrary _library;

        public override void OnCreated()
        {
            // library = AssetDatabase.FindAssets($"t:{nameof(BakingPresetLibrary)}")
            //     .Select(AssetDatabase.GUIDToAssetPath)
            //     .Select(AssetDatabase.LoadAssetAtPath<BakingPresetLibrary>)
            //     .FirstOrDefault();
        }

        public override VisualElement CreatePanelContent()
        {
            if (_library == null)
                return GetSelectLibraryView();
            
            if (view != null)
                return view;
            view = new InspectorElement(new SerializedObject(_library));
            view.style.width = 250;
            // var root = new VisualElement();
            // root.style.flexDirection = FlexDirection.Row;
            // root.Add(new SampleListView());
            // view = root;

            return view;
        }

        VisualElement GetSelectLibraryView()
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

        void CreateLibrary()
        {
            
        }

        void SelectLibrary()
        {
            SearchService.ShowObjectPicker((selectedObject, canceled) =>
                {
                    if (canceled)
                        return;
                    _library = (BakingPresetLibrary)selectedObject;
                },
                null, null, null, typeof(BakingPresetLibrary));
        }
    }
}
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    [Icon("Preset.Context")]
    [Overlay(typeof(SceneView), "Baking Presets")]
    class BakingToolsOverlay : Overlay
    {
        BakingMoldsLibrary library;
        VisualElement view;

        public override void OnCreated()
        {
            library = AssetDatabase.FindAssets($"t:{nameof(BakingMoldsLibrary)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<BakingMoldsLibrary>)
                .FirstOrDefault();
        }

        public override VisualElement CreatePanelContent()
        {
            if (view != null)
                return view;
            view = new InspectorElement(new SerializedObject(library));
            view.style.width = 250;
            // var root = new VisualElement();
            // root.style.flexDirection = FlexDirection.Row;
            // root.Add(new SampleListView());
            // view = root;

            return view;
        }
    }
}
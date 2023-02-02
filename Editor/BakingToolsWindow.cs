using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace QuickEye.BakingTools
{
    class BakingToolsWindow : EditorWindow
    {
        BakingPresetLibrary library;
        //Editor libEditor;
        //VisualElement view;

        [MenuItem("Window/Baking Tools")]
        public static void Open()
        {
            GetWindow<BakingToolsWindow>();
        }

        void OnEnable()
        {
            library = AssetDatabase.FindAssets($"t:{nameof(BakingPresetLibrary)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<BakingPresetLibrary>)
                .FirstOrDefault();
            // libEditor = Editor.CreateEditor(library);
        }

        void CreateGUI()
        {
            //view = libEditor.CreateInspectorGUI();
            var inspector = new InspectorElement(new SerializedObject(Selection.activeTransform));
            rootVisualElement.Add(inspector);
            //  rootVisualElement.Add(view);
        }
    }
}
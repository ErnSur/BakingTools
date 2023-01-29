using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace QuickEye.BakingTools
{
    class BakingToolsWindow : EditorWindow
    {
        BakingMoldsLibrary library;
        //Editor libEditor;
        //VisualElement view;

        [MenuItem("Window/Baking Tools")]
        public static void Open()
        {
            GetWindow<BakingToolsWindow>();
        }

        void OnEnable()
        {
            library = AssetDatabase.FindAssets($"t:{nameof(BakingMoldsLibrary)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<BakingMoldsLibrary>)
                .FirstOrDefault();
            // libEditor = Editor.CreateEditor(library);
        }

        void CreateGUI()
        {
            //view = libEditor.CreateInspectorGUI();
            var inspector = new InspectorElement(new SerializedObject(library));
            inspector.style.backgroundColor = Color.green;
            
            rootVisualElement.Add(inspector);
            //  rootVisualElement.Add(view);
        }
    }
}
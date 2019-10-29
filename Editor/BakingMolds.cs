using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    public class ChefTools : EditorWindow
    {
        public BakingMoldsLibrary library;

        //DisplaySelected objects as baking properties list
        //include list of materials
        [MenuItem ("CONTEXT/MeshRenderer/ChefTools")]
        public static void OpenWindow()
        {
            GetWindow<ChefTools>();
        }
        private void OnGUI()
        {
            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            {
                foreach (var mold in library.molds)
                {
                    //Maybe use somenthing like Selectable Button Controll
                    //First Click Would Select Mold and show its stats
                    //Click on Selected Button Would Make it apply Mold
                    if (GUILayout.Button(mold.name)) // Add Shortcut possibility to each mold
                    {
                        ShowNotification(new GUIContent($"{mold.name} Applied"));
                        var includeChildren = GetShouldIncludeChildren(Selection.gameObjects);

                        foreach (var go in Selection.gameObjects)
                        {
                            switch (includeChildren)
                            {
                                default:
                                case ShouldIncludeChildren.Cancel:
                                    return;

                                case ShouldIncludeChildren.HasNoChildren:
                                case ShouldIncludeChildren.DontIncludeChildren:
                                    ApplyMold(go,mold,false);
                                    break;

                                case ShouldIncludeChildren.IncludeChildren:
                                    ApplyMold(go, mold,true);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void ApplyMold(GameObject go, BakingMold mold, bool includeChildren)
        {
            Undo.RecordObject(go, "Apply baking mold");

            mold.staticFlags.TryApply(go);
            if(go.TryGetComponent<MeshRenderer>(out var renderer))
            {
                mold.castShadows.TryApply(renderer);
                mold.lightmapScale.TryApply(renderer);
                mold.stitchSeams.TryApply(renderer);
                mold.navMeshArea.TryApply(renderer);
            }

            if (includeChildren)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    var child = go.transform.GetChild(i).gameObject;
                    ApplyMold(child, mold,true);
                }
            }
        }

        private ShouldIncludeChildren GetShouldIncludeChildren(IEnumerable<GameObject> gameObjects)
        {
            if (!HasChildren())
            {
                return ShouldIncludeChildren.HasNoChildren;
            }

            return (ShouldIncludeChildren)EditorUtility.DisplayDialogComplex(
                    "Change Static Flags",
                    "Do you want to set the static flags for all the child objects as well?",
                    "Yes, change children",
                    "No, this object only",
                    "Cancel");

            bool HasChildren()
            {
                return gameObjects.Any(go => go.transform.childCount > 0);
            }
        }

        public enum ShouldIncludeChildren
        {
            HasNoChildren = -1,
            IncludeChildren = 0,
            DontIncludeChildren = 1,
            Cancel = 2
        }
    }

    [CustomEditor(typeof(BakingMoldsLibrary))]
    public class BakingMoldsLibraryEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var molds = new PropertyField(serializedObject.FindProperty("molds"));
            root.Add(molds);

            var openToolsButton = new Button(OpenTools);
            openToolsButton.text = "Open Tools";
            root.Add(openToolsButton);

            
            return root;
        }

        private void OpenTools()
        {
            var win = EditorWindow.GetWindow<ChefTools>();
            win.library = target as BakingMoldsLibrary;
        }
    }
}

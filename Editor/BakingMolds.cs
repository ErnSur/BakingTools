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
        private void OnGUI()
        {
            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            {
                foreach (var mold in library.molds)
                {
                    if (GUILayout.Button(mold.name))
                    {
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

            SetStaticFlags(go, mold.staticFlags);

            if(go.TryGetComponent<MeshRenderer>(out var renderer))
            {
                renderer.shadowCastingMode = mold.castShadows;
                SetLightmapScale(renderer, mold.lightmapScale);
                SetStitchSeams(renderer, mold.stitchSeams);
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

        private void SetLightmapScale(MeshRenderer renderer, float scale)
        {
            var so = new SerializedObject(renderer);
            so.FindProperty("m_ScaleInLightmap").floatValue = scale;
            so.ApplyModifiedProperties();
        }

        private void SetStitchSeams(MeshRenderer renderer, bool stitchSeams)
        {
            var so = new SerializedObject(renderer);
            so.FindProperty("m_StitchLightmapSeams").boolValue = stitchSeams;
            so.ApplyModifiedProperties();
        }

        private static void SetStaticFlags(GameObject obj, StaticEditorFlags flags)
        {
            Undo.RecordObject(obj, "Set Interior Static");
            GameObjectUtility.SetStaticEditorFlags(obj, flags);
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

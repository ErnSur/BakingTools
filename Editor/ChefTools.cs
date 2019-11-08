using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    public class ChefTools : EditorWindow, IHasCustomMenu
    {
        private const string _navMeshSettingsMenuPath = "Window/AI/Navigation";
        private const string _lightingSettingsMenuPath = "Window/Rendering/Lighting Settings";
        private const string _occlusionCullingSettingsMenuPath = "Window/Rendering/Occlusion Culling";

        [SerializeField]
        private BakingMoldsLibrary _library;

        //DisplaySelected objects as baking properties list
        //include list of materials
        public static void OpenWindow(BakingMoldsLibrary lib)
        {
            var wnd = GetWindow<ChefTools>();
            wnd._library = lib;
            wnd.titleContent = new GUIContent("Chef Tools");
            wnd.Init();
        }

        private void OnEnable()
        {
            if (_library != null)
            {
                Init();
            }
        }

        private void Init()
        {
            var scrollView = new ScrollView(ScrollViewMode.Vertical);
            rootVisualElement.Add(scrollView);

            var so = new SerializedObject(_library);
            var list = new MoldsReorderableList(so, so.FindProperty("molds"));
            list.applyButtonClickedEvent += i =>  OnApplyButtonClicked(_library.molds[i]);
            scrollView.Add(list);

            var moldInspector = new VisualElement();
            var moldProp = new PropertyField();
            moldProp.Bind(so);
            list.OnSelectCallback += UpdateMoldInspector;

            void UpdateMoldInspector(ReorderableList l)
            {
                moldProp.bindingPath = $"molds.Array.data[{l.index}]";
                moldProp.Bind(so);
                moldProp.Q<Foldout>().value = true;
            }
            moldInspector.Add(moldProp);

            scrollView.Add(moldInspector);
        }

        // Add Shortcut possibility to each mold
        private void OnApplyButtonClicked(BakingMold mold)
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
                        ApplyMold(go, mold, false);
                        break;

                    case ShouldIncludeChildren.IncludeChildren:
                        ApplyMold(go, mold, true);
                        break;
                }
            }
        }

        private void ApplyMold(GameObject go, BakingMold mold, bool includeChildren)
        {
            Undo.RecordObject(go, "Apply baking mold");

            mold.staticFlags.TryApply(go);
            if (go.TryGetComponent<MeshRenderer>(out var renderer))
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
                    ApplyMold(child, mold, true);
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

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Lighting Settings"), false, OpenLightingSettings);
            menu.AddItem(new GUIContent("Occlusion Settings"), false, OpenOcclusionSettings);
            menu.AddItem(new GUIContent("Navigation Settings"), false, OpenNavigationSettings);
            void OpenNavigationSettings()
            {
                EditorApplication.ExecuteMenuItem(_navMeshSettingsMenuPath);
            }
            void OpenLightingSettings()
            {
                EditorApplication.ExecuteMenuItem(_lightingSettingsMenuPath);
            }
            void OpenOcclusionSettings()
            {
                EditorApplication.ExecuteMenuItem(_occlusionCullingSettingsMenuPath);
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
}

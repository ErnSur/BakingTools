using System;
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
        [MenuItem("CONTEXT/MeshRenderer/ChefTools")]
        public static void OpenWindow(BakingMoldsLibrary lib)
        {
            var wnd = GetWindow<ChefTools>();
            wnd.library = lib;
            wnd.titleContent = new GUIContent("Chef Tools");
            wnd.Init();
        }

        private void OnEnable()
        {
            if (library != null)
            {
                Init();
            }
        }

        private void Init()
        {
            var so = new SerializedObject(library);
            //var molds = new PropertyField(so.FindProperty("molds"));
            //rootVisualElement.Add(molds);

            //using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            var listView = new ListView();
            listView.itemsSource = library.molds;
            listView.itemHeight = 23;
            listView.style.minHeight = 200;
            listView.style.flexGrow = 1;
            listView.makeItem = () =>
            {
                var container = new VisualElement();
                container.style.flexDirection = FlexDirection.Row;
                container.style.alignItems = Align.Center;
                container.style.justifyContent = Justify.SpaceBetween;

                var label = new Label();
                container.Add(label);

                var b = new Button();
                b.style.height = 16;
                b.style.width = 100;
                b.text = "Apply";
                container.Add(b);
                return container;
            };
            listView.bindItem = (e, index) =>
            {
                Debug.Log(index);
                e.Q<Label>().text = library.molds[index].name;
                e.Q<Button>().clickable = new Clickable(() => OnApplyButtonClicked(library.molds[index]));
            };
            //listView.bindingPath = "molds";
            listView.selectionType = SelectionType.Single;



            var moldInspector = new VisualElement();
            var moldProp = new PropertyField();
            listView.onSelectionChanged += UpdateMoldInspecotr;

            void UpdateMoldInspecotr(List<object> obj)
            {
                moldProp.bindingPath = $"molds.Array.data[{listView.selectedIndex}]";
                moldProp.Bind(so);
            }
            moldInspector.Add(moldProp);


            rootVisualElement.Add(listView);
            rootVisualElement.Add(moldInspector);
            rootVisualElement.Bind(so);
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

        public enum ShouldIncludeChildren
        {
            HasNoChildren = -1,
            IncludeChildren = 0,
            DontIncludeChildren = 1,
            Cancel = 2
        }
    }
}

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
    public class ChefTools : EditorWindow, ISerializationCallbackReceiver
    {
        [SerializeField]
        private BakingMold[] molds;

        static ChefTools()
        {

        }

        //public BakingMoldsLibrary library;

        //DisplaySelected objects as baking properties list
        //include list of materials
        [MenuItem("CONTEXT/MeshRenderer/ChefTools")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<ChefTools>();
            //wnd.library = lib;
            wnd.titleContent = new GUIContent("Chef Tools");
        }

        private void OnEnable()
        {
            Debug.Log("Enable");

            molds = ChefToolsSettings.settings.Get<BakingMold[]>("molds", SettingsScope.Project);

            Init();
        }

        private void Init()
        {
            var so = new SerializedObject(this);
            var moldsProperty = so.FindProperty("molds");
            var moldsPropertyField = new PropertyField(moldsProperty);

            var listView = CreateMoldListView();
            var addButton = CreateListAddButton(moldsProperty,listView);
            var moldInspector = CreateMoldInspector(so, listView);

            var refresh = new Button(() => { listView.Refresh(); })
            { text = "Refresh"};
            rootVisualElement.Add(refresh);

            rootVisualElement.Add(listView);
            rootVisualElement.Add(addButton);
            rootVisualElement.Add(moldInspector);
            rootVisualElement.Bind(so);
        }

        private static VisualElement CreateMoldInspector(SerializedObject so, ListView listView)
        {
            var moldInspector = new VisualElement();
            var moldProp = new PropertyField();
            listView.onSelectionChanged += UpdateMoldInspecotr;

            void UpdateMoldInspecotr(List<object> obj)
            {
                moldProp.bindingPath = $"molds.Array.data[{listView.selectedIndex}]";
                moldProp.Bind(so);
                moldProp.Q<Foldout>().value = true;
            }
            moldInspector.Add(moldProp);
            return moldInspector;
        }

        private VisualElement CreateListAddButton(SerializedProperty moldsProperty,ListView listView)
        {
            var add = new Button(AddMold) { text = "Add" };
            void AddMold()
            {
                Debug.Log(moldsProperty.arraySize);
                var l = molds.ToList();
                l.Add(null);
                molds = l.ToArray();
                listView.Refresh();
            }
            return add;
        }

        private ListView CreateMoldListView()
        {
            //using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            var listView = new ListView();
            listView.itemsSource = molds;
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
                e.Q<Label>().text = molds[index].name;
                e.Q<Button>().clickable = new Clickable(() => OnApplyButtonClicked(molds[index]));
            };
            listView.selectionType = SelectionType.Single;
            return listView;
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

        public void OnBeforeSerialize()
        {
            Debug.Log("Serialize");
            ChefToolsSettings.settings.Set("molds", molds, SettingsScope.Project);
            ChefToolsSettings.settings.Save();
        }

        public void OnAfterDeserialize()
        {
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

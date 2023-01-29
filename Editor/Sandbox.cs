using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    public class BakingToolInspector : InspectorElement
    {
        public BakingToolInspector() : base(AssetDatabase.FindAssets($"t:{nameof(BakingMoldsLibrary)}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<BakingMoldsLibrary>)
            .FirstOrDefault())
        {
            
        }

        class Factory : UxmlFactory<BakingToolInspector, UxmlTraits>
        {
            
        }
    }
    
    public class SampleListView : ListView
    {
        public SampleListView()
        {
            itemsSource = Enumerable.Range(0, 5).Select(i => i.ToString()).ToArray();
            showAddRemoveFooter = true;
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/FlexTest.uss"));
            style.borderBottomWidth =
            style.borderLeftWidth =
            style.borderRightWidth =
            style.borderTopWidth = 2;
            style.borderBottomColor =
                style.borderLeftColor =
                    style.borderRightColor =
                        style.borderTopColor = Color.blue;
            Rebuild();
            
        }

        class Factory : UxmlFactory<SampleListView, UxmlTraits>
        {
            
        }
    }
    
    
    public class AdvancedListView : ListView
    {
        public AdvancedListView()
        {
            itemsSource = Enumerable.Range(0, 30).Select(i => i.ToString()).ToArray();
            //Rebuild();
            
            showAddRemoveFooter = true;
            showBorder = true;
            fixedItemHeight = 25;
            showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            showBoundCollectionSize = false;
            reorderMode = ListViewReorderMode.Animated;
            name = "mold-list";
            makeItem = CreateListViewElement;
            //bindingPath = nameof(BakingMoldsLibrary.molds);
            this.BindProperty(new SerializedObject(GetData()).FindProperty("molds"));
        }

        private BakingMoldsLibrary GetData()
        {
            return AssetDatabase.FindAssets($"t:{nameof(BakingMoldsLibrary)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<BakingMoldsLibrary>)
                .FirstOrDefault();
        }
        
        VisualElement CreateListViewElement()
        {
            var root = new BindableElement();
            root.style.height = fixedItemHeight;
            root.AddToClassList("mold-list__item");
            var label = new Label();
            label.bindingPath = nameof(BakingMold.name);
            root.Add(label);


            var button = new ToolbarButton();
            button.text = "Apply";
            root.Add(button);

            return root;
        }

        class Factory : UxmlFactory<AdvancedListView, UxmlTraits>
        {
            
        }
    }
}
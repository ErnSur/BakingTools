using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    public class MoldsReorderableList : IMGUIContainer
    {
        public event Action<int> applyButtonClickedEvent;

        private readonly ReorderableList _imguiList;

        public int Index => _imguiList.index;

        public ReorderableList.SelectCallbackDelegate OnSelectCallback
        {
            get => _imguiList.onSelectCallback;
            set => _imguiList.onSelectCallback = value;
        }

        public MoldsReorderableList(SerializedObject so, SerializedProperty property)
        {
            _imguiList = new ReorderableList(so, property)
            {
                drawElementCallback = DrawElement,
                drawHeaderCallback = DrawHeader,
                onChangedCallback = _ => so.ApplyModifiedProperties(),
                headerHeight = 20
            };

            onGUIHandler = _imguiList.DoLayoutList;
        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, _imguiList.serializedProperty.displayName);
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var labelRect = rect;
            var isSelected = _imguiList.index == index;

            if (isSelected)
            {
                var buttonRect = new Rect(rect)
                {
                    xMin = rect.xMax - 100,
                };
                labelRect.xMax = buttonRect.xMin;

                DrawApplyButton(buttonRect, index);
            }

            GUI.Label(labelRect, _imguiList.serializedProperty.GetArrayElementAtIndex(index).displayName);
        }

        private void DrawApplyButton(Rect rect, int index)
        {
            if (GUI.Button(rect, "Apply"))
            {
                applyButtonClickedEvent?.Invoke(index);
            }
        }
    }
}

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    [CustomPropertyDrawer(typeof(ToglabbleParameter<>),true)]
    public class ToglableParameterDrawer : PropertyDrawer
    {
        private Toggle _isOnField;
        private VisualElement _valueField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.flexGrow = 1;

            _isOnField = new Toggle();
            _isOnField.name =
            _isOnField.bindingPath = "isOn";
            _isOnField.RegisterValueChangedCallback(OnIsOnValueChange);
            root.Add(_isOnField);

            var valueProp = property.FindPropertyRelative("value");

            if(IsPropertyTypeFlag(valueProp, out var defaultValue))
            {
                _valueField = new EnumFlagsField(defaultValue, false)
                {
                    label = property.displayName
                };
            }
            else
            {
                _valueField = new PropertyField
                {
                    label = property.displayName
                };
            }

            _valueField.name =
            (_valueField as IBindable).bindingPath = "value";
            _valueField.style.flexGrow = 1;

            root.Add(_valueField);

            root.RegisterCallback<AttachToPanelEvent>(InitFields);

            return root;
        }

        private void InitFields(AttachToPanelEvent evt)
        {
            _valueField.SetEnabled(_isOnField.value);
        }

        private void OnIsOnValueChange(ChangeEvent<bool> evt)
        {
            if(evt.target == _isOnField)
            {
                _valueField.SetEnabled(evt.newValue);
            }
        }

        private bool IsPropertyTypeFlag(SerializedProperty prop, out Enum value)
        {
            if (prop.propertyType == SerializedPropertyType.Enum)
            {
                var baseType = fieldInfo.FieldType.BaseType;
                if (baseType.IsGenericType)
                {
                    var genericArgument = baseType.GetGenericArguments()[0];
                    
                    value = Enum.ToObject(genericArgument, prop.enumValueIndex) as Enum;

                    return genericArgument.IsDefined(typeof(FlagsAttribute), true);
                }
            }
            value = null;

            return false;
        }
    }
}

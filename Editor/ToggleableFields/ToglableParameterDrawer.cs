using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.BakingTools
{
    [CustomPropertyDrawer(typeof(ToglabbleParameter<>), true)]
    public class ToglableParameterDrawer : PropertyDrawer
    {
        #region TemporaryBugWorkaround
        //Unity seems to reuse PropertyDrawers when drawing ones in a list
        //due to this everything needs to be inside the scope of CreatePropertyGUI function
        //if this is a bug and will be fixed we can move it outside this scope
        //private Toggle _isOnField;
        //private VisualElement _valueField;
        #endregion
        

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.flexGrow = 1;

            var isOnField = new Toggle();
            isOnField.name =
            isOnField.bindingPath = "isOn";
            root.Add(isOnField);

            VisualElement valueField;

            var valueProp = property.FindPropertyRelative("value");
            if (IsPropertyTypeFlag(valueProp, out var defaultValue))
            {
                valueField = new EnumFlagsField(defaultValue, false)
                {
                    label = property.displayName
                };
            }
            else
            {
                valueField = new PropertyField
                {
                    label = property.displayName
                };
            }

            isOnField.RegisterValueChangedCallback(OnIsOnValueChange);

            valueField.name =
            (valueField as IBindable).bindingPath = "value";
            valueField.style.flexGrow = 1;

            root.Add(valueField);

            root.RegisterCallback<AttachToPanelEvent>(InitFields);

            return root;

            #region TemporaryBugWorkaround
            //Unity seems to reuse PropertyDrawers when drawing ones in a list
            //due to this everything needs to be inside the scope of this function
            //if this is a bug and will be fixed we can move it outside this scope
            void InitFields(AttachToPanelEvent evt)
            {
                valueField.SetEnabled(isOnField.value);
            }

            void OnIsOnValueChange(ChangeEvent<bool> evt)
            {
                if (evt.target == isOnField)
                {
                    valueField.SetEnabled(evt.newValue);
                }
            }

            bool IsPropertyTypeFlag(SerializedProperty prop, out Enum value)
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
            #endregion
        }
    }
}

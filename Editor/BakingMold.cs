using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace QuickEye.BakingTools
{
    [System.Serializable]
    public class BakingMold
    {
        public string name;
        public StaticEditorFlags staticFlags;
        public int lightmapScale;
        public bool stitchSeams;
        public ShadowCastingMode castShadows;
        public ToglabbleInt lightmapScale2;

    }

    [System.Serializable]
    public class ToglabbleInt : ToglabbleParameter<int> { }

    public class ToglabbleParameter<T>
    {
        public bool isOn;
        public T value;

        public static explicit operator T(ToglabbleParameter<T> p) => p.value;
    }

    //[CustomPropertyDrawer(typeof(ToglabbleParameter<>),true)]
    public class ToglableParameterDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var toggleRect = new Rect
            {
                xMin = position.xMin,
                yMin = position.yMin,
                yMax = position.yMax,
                xMax = position.xMin = 30
            };

            using(var s  = new EditorGUI.ChangeCheckScope())
            {
                EditorGUI.PropertyField(toggleRect, property.FindPropertyRelative("isOn"), GUIContent.none);
                
                EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), true);
                if (s.changed)
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            //base.OnGUI(position, property, label);
        }
    }
}

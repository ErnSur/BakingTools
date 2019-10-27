using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuickEye.BakingTools
{
    public class ChefTools : EditorWindow
    {
        public BakingMoldsLibrary library;

        //DisplaySelected objects as baking properties list
        //include flags,scale,list of materials,seams
        private void OnGUI()
        {
            var meshRenderers = Selection.GetFiltered<MeshRenderer>(SelectionMode.OnlyUserModifiable);
            using(new EditorGUI.DisabledScope(meshRenderers.Length == 0))
            {
                foreach (var mold in library.molds)
                {
                    if (GUILayout.Button(mold.name))
                    {
                        foreach (var renderer in meshRenderers)
                        {
                            Undo.RecordObject(renderer,"change scale");
                            SetLightmapScale(renderer, mold.lightmapScale);
                            SetStitchSeams(renderer, mold.stitchSeams);
                        }
                    }
                }
            }
        }

        private void DrawMoldSection(BakingMold mold)
        {
            if (GUILayout.Button(mold.name))
            {

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
    }

    [System.Serializable]
    public class BakingMold
    {
        public string name;
        public StaticEditorFlags staticFlags;
        public int lightmapScale;
        public bool stitchSeams;
    }

    [CustomEditor(typeof(BakingMoldsLibrary))]
    public class BakingMoldsLibraryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open Chef Tools"))
            {
                var win = EditorWindow.GetWindow<ChefTools>();
                win.library = target as BakingMoldsLibrary;
            }
        }
    }
}

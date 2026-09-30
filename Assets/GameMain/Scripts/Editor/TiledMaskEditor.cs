using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Editor
{
    [CustomEditor(typeof(TiledMask), true)]
    public class TiledMaskEditor : ImageEditor
    {
        SerializedProperty m_rowcount;
        SerializedProperty m_colcount;
        SerializedProperty m_progress;
        SerializedProperty m_flipY;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_rowcount = serializedObject.FindProperty("rowCount");
            m_colcount = serializedObject.FindProperty("colCount");
            m_progress = serializedObject.FindProperty("progress");
            m_flipY = serializedObject.FindProperty("flipShowY");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var m_Type = serializedObject.FindProperty("m_Type");
            m_Type.enumValueIndex = (int)Image.Type.Tiled;

            EditorGUILayout.PropertyField(m_rowcount, new GUIContent("行数"));
            EditorGUILayout.PropertyField(m_colcount, new GUIContent("列数"));
            EditorGUILayout.Slider(m_progress, 0, 1, new GUIContent("进度"));
            EditorGUILayout.PropertyField(m_flipY, new GUIContent("翻转显示Y"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
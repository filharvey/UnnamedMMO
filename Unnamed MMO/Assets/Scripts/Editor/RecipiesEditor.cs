using Acemobe.MMO.Data.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Acemobe.MMO
{
    [CustomPropertyDrawer(typeof(RecipieMaterial))]
    class RecipieMaterialEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int oldIndentLevel = EditorGUI.indentLevel;
            SerializedProperty count = property.FindPropertyRelative("count");
            SerializedProperty mat = property.FindPropertyRelative("material");

            EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Item"));

            float width = contentPosition.width;
            EditorGUI.indentLevel = 0;
            contentPosition.width = width * 0.6f;

            EditorGUIUtility.labelWidth = 0f;
            EditorGUI.ObjectField(contentPosition, mat, new GUIContent(""));

            contentPosition.x += contentPosition.width;
            contentPosition.width = width * 0.4f;

            EditorGUIUtility.labelWidth = 14f;
            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.IntField(contentPosition, new GUIContent("#"), count.intValue);

            if (EditorGUI.EndChangeCheck())
            {
                count.intValue = newValue;
            }

            EditorGUI.EndProperty();
            EditorGUI.indentLevel = oldIndentLevel;
        }
    }

    [CustomEditor(typeof(Recipies))]
    class RecipiesEditor : Editor
    {
        new SerializedProperty name;
        SerializedProperty description;
        SerializedProperty item;
        SerializedProperty materials;
        SerializedProperty required;

        void OnEnable()
        {
            string path = AssetDatabase.GetAssetPath(target);

            name = serializedObject.FindProperty("name");
            description = serializedObject.FindProperty("description");
            item = serializedObject.FindProperty("item");
            required = serializedObject.FindProperty("requiredItem");
            materials = serializedObject.FindProperty("materials");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(name);
            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(item);
            EditorGUILayout.PropertyField(required);
            RecipieEditorList.Show(materials);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
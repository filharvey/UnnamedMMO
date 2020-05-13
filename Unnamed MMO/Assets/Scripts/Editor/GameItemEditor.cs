using Acemobe.MMO.Data.ScriptableObjects;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

namespace Acemobe.MMO
{
    [CustomEditor(typeof(GameItem))]
    class GameItemEditor : Editor
    {
        new SerializedProperty name;
        SerializedProperty icon;
        SerializedProperty prefab;
        SerializedProperty itemType;
        SerializedProperty type;
        SerializedProperty actionType;
        SerializedProperty maxStack;
        SerializedProperty isHarvestable;
        SerializedProperty isPickable;
        SerializedProperty isBuildingBase;
        SerializedProperty requiresBase;
        SerializedProperty dropItems;
        SerializedProperty growthTime;

        void OnEnable()
        {
            name = serializedObject.FindProperty("name");
            icon = serializedObject.FindProperty("icon");
            prefab = serializedObject.FindProperty("prefab");
            type = serializedObject.FindProperty("type");
            itemType = serializedObject.FindProperty("itemType");
            actionType = serializedObject.FindProperty("actionType");
            maxStack = serializedObject.FindProperty("maxStack");
            isHarvestable = serializedObject.FindProperty("isHarvestable");
            isPickable = serializedObject.FindProperty("isPickable");
            isBuildingBase = serializedObject.FindProperty("isBuildingBase");
            
            requiresBase = serializedObject.FindProperty("requiresBase");

            growthTime = serializedObject.FindProperty("growthTime");
            dropItems = serializedObject.FindProperty("dropItems");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(name);
            EditorGUILayout.PropertyField(icon);
            EditorGUILayout.PropertyField(prefab);

            EditorGUILayout.PropertyField(type);
            EditorGUILayout.PropertyField(itemType);
            EditorGUILayout.PropertyField(actionType);
            EditorGUILayout.PropertyField(maxStack);
            EditorGUILayout.PropertyField(isHarvestable);
            EditorGUILayout.PropertyField(isPickable);
            EditorGUILayout.PropertyField(isBuildingBase);
            EditorGUILayout.PropertyField(requiresBase);
            EditorGUILayout.PropertyField(growthTime);

            GameItemEditorList.Show(dropItems);

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(DropItens))]
    class DropItensEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int oldIndentLevel = EditorGUI.indentLevel;
            SerializedProperty mat = property.FindPropertyRelative("material");
            SerializedProperty min = property.FindPropertyRelative("min");
            SerializedProperty max = property.FindPropertyRelative("max");
            SerializedProperty chance = property.FindPropertyRelative("chance");

            EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Item"));

            float width = contentPosition.width;
            float x = contentPosition.x;
            EditorGUI.indentLevel = 0;
            contentPosition.width = width * 0.7f;
            contentPosition.height = 20f;

            EditorGUIUtility.labelWidth = 0f;
            EditorGUI.ObjectField(contentPosition, mat, new GUIContent(""));

            contentPosition.x += contentPosition.width;
            contentPosition.width = width * 0.3f;

            EditorGUIUtility.labelWidth = 25f;
            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.IntField(contentPosition, new GUIContent("Min"), min.intValue);

            if (EditorGUI.EndChangeCheck())
            {
                min.intValue = newValue;
            }

            contentPosition.x = x;
            contentPosition.y += 22;
            contentPosition.width = width * 0.7f;

            EditorGUIUtility.labelWidth = 50f;
            EditorGUI.BeginChangeCheck();
            newValue = EditorGUI.IntField(contentPosition, new GUIContent("Chance"), chance.intValue);

            if (EditorGUI.EndChangeCheck())
            {
                chance.intValue = newValue;
            }

            contentPosition.x += contentPosition.width;
            contentPosition.width = width * 0.3f;

            EditorGUIUtility.labelWidth = 25f;
            EditorGUI.BeginChangeCheck();
            newValue = EditorGUI.IntField(contentPosition, new GUIContent("Max"), max.intValue);

            if (EditorGUI.EndChangeCheck())
            {
                max.intValue = newValue;
            }

            EditorGUI.EndProperty();
            EditorGUI.indentLevel = oldIndentLevel;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 44.0f;
        }
    }
}

#endif

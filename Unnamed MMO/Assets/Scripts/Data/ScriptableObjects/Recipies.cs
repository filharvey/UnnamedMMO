using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Acemobe.MMO.Data.ScriptableObjects
{
    [Serializable]
    public class RecipieMaterial
    {
        public GameItem material;
        public int count;
    }

    [CreateAssetMenu]
    public class Recipies : ScriptableObject
    {
        public new string name;
        public string description;
        public Sprite icon;

        public GameItem item;

        public List<RecipieMaterial> materials;
    }

    public static class EditorList
    {
        private static GUIContent
            deleteButtonContent = new GUIContent("-", "Delete"),
            addButtonContent = new GUIContent("+", "Add");
        private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

        public static void Show(SerializedProperty list, bool showListSize = true, bool showListLabel = true)
        {
            Recipies recipie = (Recipies)list.serializedObject.targetObject;

            if (showListLabel)
            {
                EditorGUILayout.PropertyField(list, false);
            }

            EditorGUI.indentLevel += 1;
            if (list.isExpanded)
            {
                if (showListSize)
                {
                    EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                }

                for (int i = 0; i < list.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), true);

                    if (GUILayout.Button(deleteButtonContent, GUILayout.Width(20f), GUILayout.Width(20f)))
                    {
                        RecipieMaterial material = recipie.materials[i];
                        recipie.materials.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button(addButtonContent, EditorStyles.miniButton))
                {
                    RecipieMaterial material = new RecipieMaterial();
                    recipie.materials.Add(material);
                }
            }
            EditorGUI.indentLevel -= 1;
        }
    }

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

        void OnEnable()
        {
            string path = AssetDatabase.GetAssetPath(target);

            name = serializedObject.FindProperty("name");
            description = serializedObject.FindProperty("description");
            item = serializedObject.FindProperty("item");
            materials = serializedObject.FindProperty("materials");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(name);
            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(item);
            EditorList.Show(materials);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

public class MonsterDefn : ScriptableObject
{
    public int MaxHP;
}

[CreateAssetMenu]
public class MonsterDefnList : ScriptableObject
{
    public List<MonsterDefn> MonsterDefns;
}

public class MonsterDefnEditor : EditorWindow
{
    MonsterDefnList monsterDefnList;

    [MenuItem("Window/Monster Defn Editor %#m")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(MonsterDefnEditor));
    }

    void OnEnable()
    {
        monsterDefnList = AssetDatabase.LoadAssetAtPath("Assets/MonsterDefnList.asset",
                            typeof(MonsterDefnList)) as MonsterDefnList;
    }

    void OnGUI()
    {
        if (GUILayout.Button("Create New MonsterDefn List"))
        {
            monsterDefnList = ScriptableObject.CreateInstance<MonsterDefnList>();
            monsterDefnList.MonsterDefns = new List<MonsterDefn>();
            AssetDatabase.CreateAsset(monsterDefnList, "Assets/MonsterDefnList.asset");
            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Add MonsterDefn"))
        {
            MonsterDefn newMonsterDefn = ScriptableObject.CreateInstance<MonsterDefn>();
            monsterDefnList.MonsterDefns.Add(newMonsterDefn);
            AssetDatabase.AddObjectToAsset(newMonsterDefn, monsterDefnList);
            AssetDatabase.SaveAssets();
        }

        if (GUI.changed) EditorUtility.SetDirty(monsterDefnList);
    }
}
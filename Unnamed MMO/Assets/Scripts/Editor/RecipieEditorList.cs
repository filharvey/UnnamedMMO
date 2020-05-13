using Acemobe.MMO.Data.ScriptableObjects;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

namespace Acemobe.MMO
{
    public static class RecipieEditorList
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
}

#endif


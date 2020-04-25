
using Acemobe.MMO.Data.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Acemobe.MMO
{
    public static class GameItemEditorList
    {
        private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
        private static GUIContent
            deleteButtonContent = new GUIContent("-", "Delete"),
            addButtonContent = new GUIContent("+", "Add");

        public static void Show(SerializedProperty list, bool showListSize = true, bool showListLabel = true)
        {
            GameItem gameItem = (GameItem)list.serializedObject.targetObject;

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
                        gameItem.dropItems.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button(addButtonContent, EditorStyles.miniButton))
                {
                    DropItens dropItem = new DropItens();
                    gameItem.dropItems.Add(dropItem);
                }
            }
            EditorGUI.indentLevel -= 1;
        }
    }
}
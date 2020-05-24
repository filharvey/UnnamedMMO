using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

namespace TurnTheGameOn.NPCChat{

	[CustomEditor(typeof(ChatManager))]
	public class Editor_Chatmanager : Editor {

		ChatManager chatManager;

		static bool showNPCIndexes;
		static int conversationIndexes;

		public override void OnInspectorGUI(){
			chatManager = (ChatManager)target;
			if (conversationIndexes == 0) {
				conversationIndexes = chatManager.npcConversationIndexes.Length;
			}

			EditorGUILayout.BeginVertical("Box");

			SerializedProperty playerName = serializedObject.FindProperty ("playerName");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (playerName, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();

			if (GUILayout.Button ("Delete Player Name From PlayerPrefs")) {
				chatManager.playerName = "";
				playerName.stringValue = "";
				PlayerPrefs.DeleteKey ("PLAYERNAME");
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
				serializedObject.ApplyModifiedProperties ();
			}

			EditorGUILayout.Space ();

			// SerializedProperty targetNPC = serializedObject.FindProperty ("targetNPC");
			// EditorGUI.BeginChangeCheck ();
			// EditorGUILayout.PropertyField (targetNPC, true);
			// if (EditorGUI.EndChangeCheck ())
			// 	serializedObject.ApplyModifiedProperties ();

			EditorGUILayout.BeginHorizontal ();
			SerializedProperty conversationIndex = serializedObject.FindProperty ("npcConversationIndexes");
			conversationIndexes = EditorGUILayout.IntField ("NPC Indexes", conversationIndexes);			
			if (GUILayout.Button ("Update")) {
				conversationIndex.arraySize = conversationIndexes;
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
				serializedObject.ApplyModifiedProperties ();
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginVertical("Box");
			showNPCIndexes = EditorGUI.Foldout (EditorGUILayout.GetControlRect(), showNPCIndexes, "   NPC Conversation Indexes", true);
			EditorGUILayout.EndVertical();
			if(showNPCIndexes){
				int size = conversationIndex.arraySize;
				for(int i = 0; i < size; i++){
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("NPC Number " + i, GUILayout.MaxWidth (90));
						SerializedProperty currentDialogue = serializedObject.FindProperty ("npcConversationIndexes").GetArrayElementAtIndex (i);
						EditorGUI.BeginChangeCheck ();
						EditorGUILayout.PropertyField (currentDialogue, GUIContent.none, true);
						if (EditorGUI.EndChangeCheck ())
							serializedObject.ApplyModifiedProperties ();
						EditorGUILayout.EndHorizontal ();
				}
			}


			EditorGUILayout.EndVertical ();
		}
	}
}
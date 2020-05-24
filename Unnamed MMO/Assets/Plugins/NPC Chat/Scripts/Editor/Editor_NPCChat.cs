namespace TurnTheGameOn.NPCChat
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using System.Collections;
	using UnityEditor;
	using System;

	[CustomEditor(typeof(NPCChat))]
	public class Editor_NPCChatUpdate : Editor
	{		
		[MenuItem("GameObject/UI/NPC Chat/NPC Chat Object")]
		public static void CreateNPCChat(){
			GameObject NPC = Instantiate(Resources.Load("NPC Chat")) as GameObject;
			NPC.name = "NPC Chat";
			Selection.activeObject = NPC;
			Debug.Log("NPC Chat Game Object added to scene.");
		}

		[MenuItem("GameObject/UI/NPC Chat/Default Chat Box")]
		static void Create(){
			GameObject canvasCheck = Selection.activeGameObject;
			if (canvasCheck == null) {
				Canvas findCanvas = GameObject.FindObjectOfType<Canvas> ();
				if (findCanvas != null) {
					canvasCheck = findCanvas.gameObject;
				} else {
					canvasCheck = new GameObject ("Canvas", typeof(Canvas));
					canvasCheck.GetComponent<Canvas> ().renderMode = RenderMode.ScreenSpaceOverlay;
					canvasCheck.AddComponent <GraphicRaycaster>();
				}
				GameObject instance = Instantiate (Resources.Load ("Default Chat Box", typeof(GameObject))) as GameObject;
				instance.transform.SetParent(canvasCheck.transform);
				instance.name = "Default Chat Box";
				instance.transform.localPosition = new Vector3(0,0,0);
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = instance;
				canvasCheck = null;
				canvasCheck = GameObject.Find ("Event System");
				if(canvasCheck == null){
					canvasCheck = new GameObject ("Event System", typeof(EventSystem));
					canvasCheck.AddComponent<StandaloneInputModule> ();
				}
				canvasCheck = null;
				instance = null;		
			} else {
				Canvas findCanvas = GameObject.FindObjectOfType<Canvas> ();
				if (findCanvas != null && canvasCheck.GetComponent<Canvas>() == null) {
					canvasCheck = findCanvas.gameObject;
				}
				GameObject instance = Instantiate (Resources.Load ("Default Chat Box", typeof(GameObject))) as GameObject;
				instance.name = "Default Chat Box";
				instance.transform.SetParent(canvasCheck.transform);
				instance.transform.localPosition = new Vector3(0,0,0);
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = instance;
				canvasCheck = null;
				canvasCheck = GameObject.Find ("Event System");
				if(canvasCheck == null){
					canvasCheck = new GameObject ("Event System", typeof(EventSystem));
					canvasCheck.AddComponent<StandaloneInputModule> ();
				}
				canvasCheck = null;
				instance = null;
			}
			Debug.Log("NPC Chat Box Object added to scene.");
		}

		int editorConversation;
		int editorPage;
		static bool showButtonSettings;
		static bool showPageEventSettings;
		static bool showChatEvents;
		static bool showPageDialogue;
		static bool showDialogue;
		static bool showOutline;
		
		int instanceID;

		public override void OnInspectorGUI()
		{
			NPCChat chatComponent = (NPCChat)target;

			int _instanceID = target.GetInstanceID ();
			if (_instanceID != instanceID)
			{
				instanceID = _instanceID;
				editorConversation = 0;
				editorPage = 0;
			}


			EditorGUI.BeginChangeCheck ();

			///Chat Manager
			SerializedProperty chatManager = serializedObject.FindProperty ("chatManager");
			EditorGUILayout.PropertyField (chatManager, true);
			///NPC Chat Options
			SerializedProperty _NPCChatOptions = serializedObject.FindProperty ("_NPCChatOptions");
			EditorGUILayout.PropertyField (_NPCChatOptions, true);
			///Player Transform
			SerializedProperty player = serializedObject.FindProperty ("player");
			EditorGUILayout.PropertyField (player, true);

			if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();


			#region section line
			var rect = EditorGUILayout.BeginHorizontal();
			Handles.color = Color.gray;
			Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
			EditorGUILayout.EndHorizontal();
			#endregion



			EditorGUILayout.BeginVertical ("box");
			//ChatType
			SerializedProperty chatType = serializedObject.FindProperty ("chatType");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (chatType, true);
			if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();


			

			if (chatComponent.chatType == NPCChat.ChatType.Default)
			{

			}
			else if (chatComponent.chatType == NPCChat.ChatType.Structured)
			{
				///NPC Number
				SerializedProperty nPCNumber = serializedObject.FindProperty ("nPCNumber");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (nPCNumber, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
				//Conversations
				EditorGUILayout.BeginHorizontal ();
				SerializedProperty conversations = serializedObject.FindProperty ("conversations");
				if (conversations.intValue == 0) conversations.intValue = 1;
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (conversations, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
				if (GUILayout.Button ("Update"))
				{
					editorConversation = 0;
					editorPage = 0;
					chatComponent.UpdateConversations ();
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndVertical ();
				
				#region Conversation Options
				EditorGUILayout.BeginHorizontal ();

				EditorGUILayout.BeginVertical ("box");

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Conversation Index:    " + editorConversation.ToString ());
					if (editorConversation >= 1) {
						if (GUILayout.Button ("-", GUILayout.MaxWidth (30))) {
							editorConversation -= 1;
							editorPage = 0;
							chatComponent.tempConv = editorConversation;
							chatComponent.tempInt = chatComponent._NPCDialogue [editorConversation].pagesOfChat;
							EditorGUI.FocusTextInControl (null);
							GUIUtility.hotControl = 0;
							GUIUtility.keyboardControl = 0;
						}
					} else {
						GUILayout.Box ("", GUILayout.MaxWidth (30));
					}
					if (editorConversation < chatComponent._NPCDialogue.Length - 1) {
						if (GUILayout.Button ("+", GUILayout.MaxWidth (30))) {
							editorConversation += 1;
							editorPage = 0;
							chatComponent.tempConv = editorConversation;
							chatComponent.tempInt = chatComponent._NPCDialogue [editorConversation].pagesOfChat;
							EditorGUI.FocusTextInControl (null);
							GUIUtility.hotControl = 0;
							GUIUtility.keyboardControl = 0;
						}
					} else {
						GUILayout.Box ("", GUILayout.MaxWidth (30));
					}
					
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.Space ();

					EditorGUILayout.BeginHorizontal ();
					chatComponent.tempInt = EditorGUILayout.IntField ("Chat Pages", chatComponent.tempInt);
					if (chatComponent.tempInt < 1)
						chatComponent.tempInt = 1;
					if (GUILayout.Button ("Update")) {				
						chatComponent.tempConv = editorConversation;
						chatComponent.canUpdatePages = true;
						chatComponent.UpdateConversations ();
						editorPage = 0;
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
					}
					EditorGUILayout.EndHorizontal ();

				///Set Next Conversation - Conversation set in Chat Manager after this conversation is completed
				SerializedProperty useNextDialogue = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("useNextDialogue");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (useNextDialogue, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
				

				chatComponent.CalculateArrays ();
				serializedObject.UpdateIfRequiredOrScript ();
				if (chatComponent._NPCDialogue [editorConversation].useNextDialogue == true)
				{
					SerializedProperty nextDialogue = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("nextDialogue");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (nextDialogue, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
				}

			
			EditorGUILayout.EndHorizontal ();

			
			#endregion
			}
			EditorGUILayout.EndVertical();


			#region section line
			rect = EditorGUILayout.BeginHorizontal();
			Handles.color = Color.gray;
			Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
			EditorGUILayout.EndHorizontal();

			#endregion

			

			#region Page Options

			if (chatComponent.chatType == NPCChat.ChatType.Default)
			{

			}
			else if (chatComponent.chatType == NPCChat.ChatType.Structured)
			{
				EditorGUILayout.LabelField ("Page Dialogue", EditorStyles.centeredGreyMiniLabel);
				EditorGUILayout.BeginHorizontal ();
				
				EditorGUILayout.LabelField ("Conversation Index " + editorConversation.ToString () + " - Page " + editorPage.ToString ());

				if (editorPage >= 1)
				{
					if (GUILayout.Button ("-", GUILayout.MaxWidth (30)))
					{
						editorPage -= 1;
						EditorGUI.FocusTextInControl (null);
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
					}
				}
				else
				{
					GUILayout.Box ("", GUILayout.MaxWidth (30));
				}
				if (editorPage < chatComponent._NPCDialogue [editorConversation].pagesOfChat - 1)
				{
					if (GUILayout.Button ("+", GUILayout.MaxWidth (30)))
					{
						editorPage += 1;
						EditorGUI.FocusTextInControl (null);
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
					}
				}
				else
				{
					GUILayout.Box ("", GUILayout.MaxWidth (30));
				}

				EditorGUILayout.EndHorizontal ();

			}

			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.BeginVertical ();
				

EditorGUILayout.BeginVertical ("box");
			EditorGUI.BeginChangeCheck ();

				SerializedProperty page_ChatBox = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("page_ChatBox").GetArrayElementAtIndex (editorPage);
				EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (page_ChatBox, new GUIContent("Chat Box"), true);
				if (EditorGUI.EndChangeCheck ())serializedObject.ApplyModifiedProperties ();
EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical ("Box");
				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Title Text");
				EditorGUILayout.EndHorizontal ();

				chatComponent._NPCDialogue [editorConversation].NPCName [editorPage] = EditorGUILayout.TextField (chatComponent._NPCDialogue [editorConversation].NPCName [editorPage]);

			EditorGUILayout.EndVertical ();


			EditorGUILayout.BeginVertical ("Box");
				EditorStyles.textField.wordWrap = true;
				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Dialogue Text");
				EditorGUILayout.EndHorizontal ();

				chatComponent._NPCDialogue [editorConversation].chatPages [editorPage] = EditorGUILayout.TextArea (chatComponent._NPCDialogue [editorConversation].chatPages [editorPage]);

			EditorGUILayout.EndVertical ();


			/// Page Audio
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("Page Audio", GUILayout.MaxWidth(90));
			SerializedProperty pageAudio = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("pageAudio").GetArrayElementAtIndex(editorPage);
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (pageAudio, GUIContent.none, true);
			if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndHorizontal ();
			

			if (chatComponent._NPCDialogue [editorConversation].pageAudio [editorPage] != null)
			{
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Loop AudioClip", GUILayout.MaxWidth(90));
				SerializedProperty loopAudio = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("loopAudio").GetArrayElementAtIndex(editorPage);
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (loopAudio, GUIContent.none, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.EndVertical ();



			/// Page Events
			if (chatComponent.chatType == NPCChat.ChatType.Structured)
			{
				EditorGUILayout.BeginVertical ("Box");
				showPageEventSettings = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showPageEventSettings, "  Page Events", true);
				if (showPageEventSettings) {
					EditorGUILayout.LabelField ("On Page Start Event");
					SerializedProperty OnPageStartEvent = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("OnPageStartEvent").GetArrayElementAtIndex (editorPage);

					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (OnPageStartEvent, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					EditorGUILayout.LabelField ("On Page End Event");
					SerializedProperty OnPageEndEvent = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("OnPageEndEvent").GetArrayElementAtIndex (editorPage);

					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (OnPageEndEvent, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
				}
				EditorGUILayout.EndVertical ();
			}
			


			///
			/// Page Buttons
			///
			EditorGUILayout.BeginVertical ("Box");
			showButtonSettings = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showButtonSettings, "  Page Buttons", true);
			if (showButtonSettings)
			{
				EditorGUILayout.Space ();
				chatComponent._NPCDialogue [editorConversation].enableButtonAfterDialogue [editorPage] = EditorGUILayout.Toggle ("Enable After Dialogue", chatComponent._NPCDialogue [editorConversation].enableButtonAfterDialogue [editorPage]);

				EditorGUILayout.Space ();

				for (int i = 0; i < 6; i++) {

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.Space ();
					SerializedProperty enableButton = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("NPCButtons").GetArrayElementAtIndex (editorPage).FindPropertyRelative ("buttonComponent").GetArrayElementAtIndex (i);
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (enableButton, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.Space ();

					if (chatComponent._NPCDialogue[editorConversation].NPCButtons[editorPage].buttonComponent[i] == NPCDialogueButtons.ItemType.enableButton) {
						SerializedProperty buttonString = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("NPCButtons").GetArrayElementAtIndex (editorPage).FindPropertyRelative ("buttonString").GetArrayElementAtIndex (i);
						EditorGUI.BeginChangeCheck ();
						EditorGUILayout.PropertyField (buttonString, true);
						if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

						SerializedProperty NPCClick = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("NPCButtons").GetArrayElementAtIndex (editorPage).FindPropertyRelative ("NPCClick").GetArrayElementAtIndex (i);

						EditorGUI.BeginChangeCheck ();
						EditorGUILayout.PropertyField (NPCClick, true);
						if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					}
				}
			}
			EditorGUILayout.EndVertical ();

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
			#endregion

			#region section line
			rect = EditorGUILayout.BeginHorizontal();
			Handles.color = Color.gray;
			Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
			EditorGUILayout.EndHorizontal();
			#endregion

			//EditorGUILayout.LabelField ("Settings", EditorStyles.centeredGreyMiniLabel);

			EditorGUILayout.BeginVertical ("box");
			showChatEvents = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showChatEvents, "OnChat Start/Stop Events", true);
			if (showChatEvents)
			{
				EditorGUI.BeginChangeCheck ();
				///OnChatEvent
				SerializedProperty OnChatEvent = serializedObject.FindProperty ("OnChatEvent");
				EditorGUILayout.PropertyField (OnChatEvent, true);
				///OnStopChatEvent
				SerializedProperty OnStopChatEvent = serializedObject.FindProperty ("OnStopChatEvent");
				EditorGUILayout.PropertyField (OnStopChatEvent, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
			}
			EditorGUILayout.EndVertical();

			if (chatComponent._NPCChatOptions != null)
			{
				if (chatComponent._NPCChatOptions.outline)
				{
					EditorGUILayout.BeginVertical ("box");
					showOutline = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showOutline, "Outline", true);
					if (showOutline)
					{
						EditorGUI.BeginChangeCheck ();
						///Mesh Renderer Objects
						SerializedProperty meshRend = serializedObject.FindProperty ("meshRend");
						EditorGUILayout.PropertyField (meshRend, true);
						///Skinned Mesh Renderer Objects
						SerializedProperty skinnedMeshRend = serializedObject.FindProperty ("skinnedMeshRend");
						EditorGUILayout.PropertyField (skinnedMeshRend, true);
						///Sprite Renderer Objects
						SerializedProperty spriteRenderer = serializedObject.FindProperty ("spriteRenderer");
						EditorGUILayout.PropertyField (spriteRenderer, true);
						if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
					}
					EditorGUILayout.EndVertical();
				}

			}

		}
	}
}
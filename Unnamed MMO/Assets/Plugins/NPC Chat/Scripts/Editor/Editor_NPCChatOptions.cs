namespace TurnTheGameOn.NPCChat
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using UnityEditor;

	[CustomEditor(typeof(NPCChatOptions))]
	public class Editor_NPCChatOptions : Editor {

		NPCChatOptions chatManager;

		static bool showNPCIndexes;
		static bool showHelp;
		static int conversationIndexes;
        static bool showChatStartStopOptions;
        static bool showMeshOutlineSettings;
		static bool showSpriteOutlineSettings;
		static bool showOutlineColorSettings;

		public override void OnInspectorGUI()
        {
            NPCChatOptions _NPCChatOptions = (NPCChatOptions)target;

			EditorGUILayout.BeginVertical("Box");
			///Disable On Complete
			SerializedProperty disableOnComplete = serializedObject.FindProperty ("disableOnComplete");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (disableOnComplete, true);
			if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

			///Destroy On Complete
			SerializedProperty destroyOnComplete = serializedObject.FindProperty ("destroyOnComplete");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (destroyOnComplete, true);
			if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

			///Allow Chat If Other Is Active
			SerializedProperty allowChatIfOtherIsActive = serializedObject.FindProperty ("allowChatIfOtherIsActive");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (allowChatIfOtherIsActive, true);
			if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

            ///Allow Chat If Other Is Active
			SerializedProperty resetChatManagerIndexOnStart = serializedObject.FindProperty("resetChatManagerIndexOnStart");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(resetChatManagerIndexOnStart, true);
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical ();

            #region Start/Stop Chat
			EditorGUILayout.BeginVertical ("box");
            showChatStartStopOptions = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showChatStartStopOptions, "Start/Stop Chat Trigger Options", true);
			if (showChatStartStopOptions)
            {	
				///Chat On Collision
				SerializedProperty chatOnCollision = serializedObject.FindProperty ("chatOnCollision");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (chatOnCollision, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

				///Close Chat On Trigger Exit
				SerializedProperty closeOnTriggerExit = serializedObject.FindProperty ("closeOnTriggerExit");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (closeOnTriggerExit, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

				///Close Chat On Mouse Or Key Up
				SerializedProperty closeOnMouseOrKeyUp = serializedObject.FindProperty ("closeOnMouseOrKeyUp");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (closeOnMouseOrKeyUp, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

				///Chat On Mouse Up
				SerializedProperty chatOnMouseUp = serializedObject.FindProperty ("chatOnMouseUp");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (chatOnMouseUp, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

				///Chat On Key Up
				SerializedProperty chatOnKeyUp = serializedObject.FindProperty ("chatOnKeyUp");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (chatOnKeyUp, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

				///Chat On Key Down
				SerializedProperty chatOnKeyDown = serializedObject.FindProperty ("chatOnKeyDown");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (chatOnKeyDown, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

				///Chat On Joystick Up
				_NPCChatOptions._chatOnJoystickUp = (Joystick)EditorGUILayout.EnumPopup ("Chat On Joystick Up", _NPCChatOptions._chatOnJoystickUp);
				switch (_NPCChatOptions._chatOnJoystickUp) {
				case Joystick.None:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.None;
					break;
				case Joystick.JoystickButton0:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton0; 
					break;
				case Joystick.JoystickButton1:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton1; 
					break;
				case Joystick.JoystickButton2:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton2; 
					break;
				case Joystick.JoystickButton3:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton3; 
					break;
				case Joystick.JoystickButton4:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton4; 
					break;
				case Joystick.JoystickButton5:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton5; 
					break;
				case Joystick.JoystickButton6:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton6; 
					break;
				case Joystick.JoystickButton7:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton7; 
					break;
				case Joystick.JoystickButton8:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton8; 
					break;
				case Joystick.JoystickButton9:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton9; 
					break;
				case Joystick.JoystickButton10:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton10; 
					break;
				case Joystick.JoystickButton11:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton11; 
					break;
				case Joystick.JoystickButton12:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton12; 
					break;
				case Joystick.JoystickButton13:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton13; 
					break;
				case Joystick.JoystickButton14:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton14; 
					break;
				case Joystick.JoystickButton15:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton15; 
					break;
				case Joystick.JoystickButton16:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton16; 
					break;
				case Joystick.JoystickButton17:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton17; 
					break;
				case Joystick.JoystickButton18:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton18; 
					break;
				case Joystick.JoystickButton19:
					_NPCChatOptions.chatOnJoystickUp = KeyCode.JoystickButton19;
					break;
				}
				///Chat On Joystick Down
				_NPCChatOptions._chatOnJoystickDown = (Joystick)EditorGUILayout.EnumPopup ("Chat On Joystick Down", _NPCChatOptions._chatOnJoystickDown);
				switch (_NPCChatOptions._chatOnJoystickDown) {
				case Joystick.None:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.None;
					break;
				case Joystick.JoystickButton0:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton0; 
					break;
				case Joystick.JoystickButton1:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton1; 
					break;
				case Joystick.JoystickButton2:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton2; 
					break;
				case Joystick.JoystickButton3:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton3; 
					break;
				case Joystick.JoystickButton4:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton4; 
					break;
				case Joystick.JoystickButton5:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton5; 
					break;
				case Joystick.JoystickButton6:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton6; 
					break;
				case Joystick.JoystickButton7:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton7; 
					break;
				case Joystick.JoystickButton8:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton8; 
					break;
				case Joystick.JoystickButton9:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton9; 
					break;
				case Joystick.JoystickButton10:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton10; 
					break;
				case Joystick.JoystickButton11:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton11; 
					break;
				case Joystick.JoystickButton12:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton12; 
					break;
				case Joystick.JoystickButton13:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton13; 
					break;
				case Joystick.JoystickButton14:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton14; 
					break;
				case Joystick.JoystickButton15:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton15; 
					break;
				case Joystick.JoystickButton16:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton16; 
					break;
				case Joystick.JoystickButton17:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton17; 
					break;
				case Joystick.JoystickButton18:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton18; 
					break;
				case Joystick.JoystickButton19:
					_NPCChatOptions.chatOnJoystickDown = KeyCode.JoystickButton19;
					break;
				}
			}
			EditorGUILayout.EndVertical();
            #endregion

            #region Distance Check
			EditorGUILayout.BeginVertical ("box");

			///Use Distance Check
			SerializedProperty distanceCheck = serializedObject.FindProperty ("distanceCheck");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (distanceCheck, true);
			if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

			if (_NPCChatOptions.distanceCheck)
            {			
				///Distance To Chat
				SerializedProperty distanceToChat = serializedObject.FindProperty ("distanceToChat");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (distanceToChat, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

				///Chat Radius Offset
				SerializedProperty distanceCheckOffset = serializedObject.FindProperty ("distanceCheckOffset");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (distanceCheckOffset, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
			}
			EditorGUILayout.EndVertical ();
            #endregion
            
            #region Scrolling Text
			EditorGUILayout.BeginVertical ("box");
			///Enable Text Scrolling
			SerializedProperty scrollingText = serializedObject.FindProperty ("scrollingText");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (scrollingText, true);
			if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

			if(_NPCChatOptions.scrollingText)
			{
				///Text Scroll Speed
				SerializedProperty textScrollSpeed = serializedObject.FindProperty ("textScrollSpeed");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (textScrollSpeed, true);
				if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
			}
			EditorGUILayout.EndVertical ();
            #endregion
            
            #region Outline
			EditorGUILayout.BeginVertical ("box");
            //NPCOutline Toggle
			SerializedProperty outline = serializedObject.FindProperty ("outline");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (outline, true);
			if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

			if (!_NPCChatOptions.distanceCheck && _NPCChatOptions.outline)
				EditorGUILayout.LabelField ("Requires Use Distance Check");

			if (_NPCChatOptions.outline && _NPCChatOptions.distanceCheck)
			{
				showOutlineColorSettings = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showOutlineColorSettings, "Outline Colors", true);
				if (showOutlineColorSettings)
				{	
					///inRangeColor
					SerializedProperty inRangeColor = serializedObject.FindProperty ("inRangeColor");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (inRangeColor, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					///outOfRangeColor
					SerializedProperty outOfRangeColor = serializedObject.FindProperty ("outOfRangeColor");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (outOfRangeColor, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					///dialogueColor
					SerializedProperty dialogueColor = serializedObject.FindProperty ("dialogueColor");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (dialogueColor, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					///mouseOverColor
					SerializedProperty mouseOverColor = serializedObject.FindProperty ("mouseOverColor");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (mouseOverColor, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
				}

				showMeshOutlineSettings = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showMeshOutlineSettings, "Mesh Outline Settings", true);
				if (showMeshOutlineSettings)
				{
					///inRangeSize
					SerializedProperty inRangeSize = serializedObject.FindProperty ("inRangeSize");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (inRangeSize, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
			
					///outOfRangeSize
					SerializedProperty outOfRangeSize = serializedObject.FindProperty ("outOfRangeSize");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (outOfRangeSize, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					///mouseOverSize
					SerializedProperty mouseOverSize = serializedObject.FindProperty ("mouseOverSize");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (mouseOverSize, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					///dialogueSize
					SerializedProperty dialogueSize = serializedObject.FindProperty ("dialogueSize");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (dialogueSize, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					///NPCOutline
					SerializedProperty NPCOutline = serializedObject.FindProperty ("NPCOutline");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (NPCOutline, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
				}

				showSpriteOutlineSettings = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showSpriteOutlineSettings, "Sprite Outline Settings", true);
				if (showSpriteOutlineSettings)
				{
					///inRangeSize
					SerializedProperty inRangeSpriteSize = serializedObject.FindProperty ("inRangeSpriteSize");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (inRangeSpriteSize, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
				
					///outOfRangeSize
					SerializedProperty outOfRangeSpriteSize = serializedObject.FindProperty ("outOfRangeSpriteSize");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (outOfRangeSpriteSize, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					///mouseOverSize
					SerializedProperty mouseOverSpriteSize = serializedObject.FindProperty ("mouseOverSpriteSize");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (mouseOverSpriteSize, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					///dialogueSize
					SerializedProperty dialogueSpriteSize = serializedObject.FindProperty ("dialogueSpriteSize");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (dialogueSpriteSize, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();

					///NPCOutlineSprite
					SerializedProperty NPCOutlineSprite = serializedObject.FindProperty ("NPCOutlineSprite");
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (NPCOutlineSprite, true);
					if (EditorGUI.EndChangeCheck ()) serializedObject.ApplyModifiedProperties ();
				}
			}
			EditorGUILayout.EndVertical();
            #endregion

		}
	}
}
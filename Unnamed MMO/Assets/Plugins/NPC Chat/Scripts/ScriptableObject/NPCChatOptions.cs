namespace TurnTheGameOn.NPCChat
{
	using UnityEngine;
	using System.Collections;

	[CreateAssetMenu(fileName = "NPCChatOptions", menuName = "TurnTheGameOn/NPC Chat/NPCChatOptions")]
	public class NPCChatOptions : ScriptableObject 
	{
		public bool allowChatIfOtherIsActive;
		public bool disableOnComplete;
		public bool destroyOnComplete;
        public bool resetChatManagerIndexOnStart;
		#region Start/Stop Chat
        [Tooltip("Enable / Disable triggering chat on player collision.")] public bool chatOnCollision;
		[Tooltip("Enable / Disable triggering chat on mouse-up when hovering over NPC Chat collider.")] public bool chatOnMouseUp;
		public bool closeOnTriggerExit;
		public bool closeOnMouseOrKeyUp;
		[Tooltip("Set a KeyCode to be used as a chat button")] public KeyCode chatOnKeyUp;
		[Tooltip("Set a KeyCode to be used as a chat button")] public KeyCode chatOnKeyDown;
		public KeyCode chatOnJoystickUp;
		[HideInInspector] public Joystick _chatOnJoystickUp;
		public KeyCode chatOnJoystickDown;
		[HideInInspector] public Joystick _chatOnJoystickDown;
		#endregion

		#region  Distance Check
		public bool distanceCheck;
		[Tooltip("The radius around NPC Chat used to determine if the player is close enough to trigger chat, used mainly to prevent mouse clicks from triggering chat. NPC Chat will close if the player leaves this radius.")] [Range (0.01f,100)] public float distanceToChat = 5;
		public Vector3 distanceCheckOffset;
		#endregion

		#region Scrolling Text
		public bool scrollingText;
		[Tooltip("Lower this setting to display text faster.")] [Range (0.0001f,20)] public float textScrollSpeed = 10;
		#endregion

		#region Outline
		public bool outline;
		public Material NPCOutline, NPCOutlineSprite;
		public float inRangeSize, outOfRangeSize, mouseOverSize, dialogueSize;
		public float inRangeSpriteSize, outOfRangeSpriteSize, mouseOverSpriteSize, dialogueSpriteSize;
		public Color inRangeColor, outOfRangeColor, mouseOverColor, dialogueColor;
		#endregion
	}

    public enum Joystick
	{
		None, JoystickButton0, JoystickButton1, JoystickButton2, JoystickButton3, JoystickButton4,
		JoystickButton5, JoystickButton6, JoystickButton7, JoystickButton8, JoystickButton9, JoystickButton10,
		JoystickButton11, JoystickButton12, JoystickButton13, JoystickButton14, JoystickButton15, JoystickButton16,
		JoystickButton17, JoystickButton18, JoystickButton19
	}
}
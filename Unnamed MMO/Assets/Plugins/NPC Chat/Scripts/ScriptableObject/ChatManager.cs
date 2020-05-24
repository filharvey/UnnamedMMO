namespace TurnTheGameOn.NPCChat
{
	using UnityEngine;
	using System.Collections;	

	[System.Serializable][CreateAssetMenu(fileName = "NPCChatOptions", menuName = "TurnTheGameOn/NPC Chat/ChatManager")]
	public class ChatManager : ScriptableObject
	{
		public string playerName;
		public bool dialogueIsActive;
		public int npcNumber;
		[TextArea(5,5)]	public string noteText;
		public int[] npcConversationIndexes;
		[HideInInspector] public int numberOfSlots;

		void Awake ()
		{
			playerName = PlayerPrefs.GetString ("PLAYERNAME", "");
		}

		void Update()
		{
			if (numberOfSlots != npcConversationIndexes.Length)
			{
				numberOfSlots = npcConversationIndexes.Length;
			}
		}

		public void SetNPCNumberToEdit(int _npcNumber)
		{
			npcNumber = _npcNumber;
		}
		
		public void SetNewConversationForNPC(int newConversation)
		{
			npcConversationIndexes[npcNumber] = newConversation;
		}

		public void SetConversationIndexForNPC(int npcNumber, int newConversation)
		{
			npcConversationIndexes[npcNumber] = newConversation;
		}

		public void SetPlayerName(string newText)
		{
			playerName = newText;
			PlayerPrefs.SetString("PLAYERNAME", playerName);
		}

	}
}
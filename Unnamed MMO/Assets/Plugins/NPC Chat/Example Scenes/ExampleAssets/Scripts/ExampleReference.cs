using UnityEngine;
using System.Collections;

public class ExampleReference : MonoBehaviour {

	public TurnTheGameOn.NPCChat.ChatManager chatManager;

	public void ChangeTarget(int targetNPC){
		chatManager.SetNPCNumberToEdit(targetNPC);
	}

	public void ChangeConversation(int newConversation){
		chatManager.SetNewConversationForNPC(newConversation);
	}

}

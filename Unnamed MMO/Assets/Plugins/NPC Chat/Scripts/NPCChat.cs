namespace TurnTheGameOn.NPCChat
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Events;
	using System.Collections;

	[System.Serializable]
	public class NPCDialogue{
		[Range(1,1000)]
		public int pagesOfChat = 1;
		public bool useNextDialogue;
		public int nextDialogue;
		public bool[] enableButtonAfterDialogue;
		public ChatBox[] page_ChatBox;
		[TextArea(3,5)] public string[] chatPages;
		public string[] NPCName;
		public NPCDialogueButtons[] NPCButtons;
		public AudioClip[] pageAudio;
		public bool[] loopAudio;
		public UnityEvent[] OnPageStartEvent;
		public UnityEvent[] OnPageEndEvent;
	}

	[System.Serializable]
	public class NPCDialogueButtons{
		public enum ItemType { none, enableButton }
		public ItemType[] buttonComponent;
		public Button.ButtonClickedEvent[] NPCClick;
		public string[] buttonString;
	}

	public class NPCChat : MonoBehaviour
	{
		[Tooltip("A reference to the chat manager scriptable object. NPC Chat checks the index of the array 'Current Dialogue' for its 'NPC Number' for structured dialogue.")] public ChatManager chatManager;
		public NPCChatOptions _NPCChatOptions;
		[Tooltip("A reference to the player object, used for distance check. NOTE: This object should also have a tag set to Player.")] public Transform player;
		[Tooltip("Used by the chat manager scriptable object to set the curent conversation for the NPC Chat object.")] public int nPCNumber;
		public UnityEvent OnChatEvent, OnStopChatEvent;

		#region NPC Chat Options Start/Stop Options
		public KeyCode chatOnJoystickUp;
		[HideInInspector] public Joystick _chatOnJoystickUp;
		public KeyCode chatOnJoystickDown;
		[HideInInspector] public Joystick _chatOnJoystickDown;
		#endregion
		
		[Tooltip("Set the total number of conversations this NPC will have. Use the chat manager 'Current Dialogue' array with the NPC Number as an index to set a new conversation.")] public int conversations;
		public NPCDialogue[] _NPCDialogue;
		public bool startConversation, talking;		
		[HideInInspector] public int tempInt, tempConv; //editor helper
		[HideInInspector] public bool canUpdatePages; //editor helper
		private int currentPage;
		private bool canChat, textIsScrolling, buttonPage;
		private Text chatText, speakerNameText;
		private GameObject tempClip;
		private Vector3 offsetValue;
		public Color gizmoColor;
		public MeshRenderer[] meshRend;
		public SkinnedMeshRenderer[] skinnedMeshRend;
		public SpriteRenderer[] spriteRenderer;
		bool mouseOver;
		bool keyDown;
		int connversationIndex;
		int startLine;
		string displayText = "";
		int currentIndex;
		//Rich Text markup variables generated at runtime to allow for scrolling text markup
		int charactersFormatted;
		bool bold, boldOpen;
		bool italic, italicOpen;
		bool color, colorOpen;
		bool size, sizeOpen;
		string boldPrefix, boldSuffix;
		string italicPrefix, italicSuffix;
		string colorString, colorPrefix, colorSuffix;
		string sizeString, sizePrefix, sizeSuffix;
		string tempFormattedString1, tempFormattedString2, finalFormattedString;
		public enum ChatType
		{
			Default,
			Structured
		}
		public ChatType chatType;

	    void Awake()
		{
			if (chatType == ChatType.Structured && chatManager == null)
			{
				Debug.LogWarning ("ChatManager reference is missing - NPC Chat will be disabled.");
				transform.gameObject.SetActive(false);
			}
			if (_NPCChatOptions != null)
			{
                if (chatManager != null)
                {
                    chatManager.dialogueIsActive = false;
                    if (_NPCChatOptions.resetChatManagerIndexOnStart)
                    {
                        chatManager.npcConversationIndexes[nPCNumber] = 0;
                    }
                }
				if (_NPCChatOptions.outline && Application.isPlaying) SetupOutline ();
				if (Application.isPlaying) SetupChatBoxes ();
				canChat = true;
				conversations = _NPCDialogue.Length;
				if(player == null)
				{
					Debug.LogWarning ("This NPC Chat object does not have a player transform refernce, it's required for NPC Chat to function. This NPC Chat object will be disabled.");
				}
			}
			else
			{
				Debug.LogWarning ("NPCChatOptions reference is missing - NPC Chat will be disabled.");
				transform.gameObject.SetActive(false);
			}
		}

		void Update()
		{		
			CheckInput ();
			CheckScrollingTextStatus ();
			CheckPlayerStatus ();
			CheckIfChatPageShouldClose ();
		}

		#region Awake/Initialization Methods
		private Material outlineMaterial;
		private Material outlineSpriteMaterial;
		void SetupOutline ()
		{
			outlineMaterial = new Material(_NPCChatOptions.NPCOutline);
			outlineSpriteMaterial = new Material(_NPCChatOptions.NPCOutlineSprite);
			//Setup Mesh Renderer outline
			for(int i = 0; i < meshRend.Length; i++){
				Material[] matArray = new Material[meshRend [i].sharedMaterials.Length + 1];
				for(int ii = 0; ii < matArray.Length - 1; ii++){
					matArray [ii] = meshRend [i].sharedMaterials[ii];
				}
				matArray [matArray.Length - 1] = outlineMaterial;
				meshRend [i].materials = matArray;
			}
			//Setup Skinned Mesh Renderer outline
			for(int i = 0; i < skinnedMeshRend.Length; i++){
				Material[] matArray = new Material[skinnedMeshRend [i].sharedMaterials.Length + 1];
				for(int ii = 0; ii < matArray.Length - 1; ii++){
					matArray [ii] = skinnedMeshRend [i].sharedMaterials[ii];
				}
				matArray [matArray.Length - 1] = outlineMaterial;
				skinnedMeshRend [i].materials = matArray;
			}
			//Setup Sprite Renderer outline
			for(int i = 0; i < spriteRenderer.Length; i++){
				Material[] matArray = new Material[spriteRenderer [i].sharedMaterials.Length + 1];
				for(int ii = 0; ii < matArray.Length - 1; ii++){
					matArray [ii] = spriteRenderer [i].sharedMaterials[ii];
				}
				matArray [matArray.Length - 1] = outlineSpriteMaterial;
				spriteRenderer [i].materials = matArray;
			}
			SetOutline (_NPCChatOptions.outOfRangeColor, _NPCChatOptions.outOfRangeSize, _NPCChatOptions.outOfRangeColor, _NPCChatOptions.outOfRangeSpriteSize);
		}

		void SetupChatBoxes ()
		{
			if (chatType == ChatType.Structured)
			{
				for (int i = 0; i < _NPCDialogue[0].page_ChatBox.Length; i++)
				{
					if(_NPCDialogue[0].page_ChatBox[i] == null)
					{
						int temp = i + 1;	Debug.Log("NPC Chat: You need to assign a Chat Box for page " + temp.ToString() );
					}
					_NPCDialogue[0].page_ChatBox[i].CloseChatBox();// _gameObject.SetActive(false);
				}
			}
			else if (chatType == ChatType.Structured)
			{
				for (int i = 0; i < _NPCDialogue[ chatManager.npcConversationIndexes[nPCNumber] ].page_ChatBox.Length; i++)
				{
					if(_NPCDialogue[ chatManager.npcConversationIndexes[nPCNumber] ].page_ChatBox[i] == null)
					{
						int temp = i + 1;	Debug.Log("NPC Chat: You need to assign a Chat Box for page " + temp.ToString() );
					}
					_NPCDialogue[ chatManager.npcConversationIndexes[ nPCNumber ] ].page_ChatBox[i].CloseChatBox();// _gameObject.SetActive(false);
				}
			}
			
		}
		#endregion

		#region Update Methods
		void CheckInput ()
		{
			if(!keyDown && ( Input.GetKeyUp(_NPCChatOptions.chatOnKeyUp) || Input.GetKeyDown(chatOnJoystickUp) ) ) OnChatKeyUp ();
			if ( Input.GetKeyDown (_NPCChatOptions.chatOnKeyDown) || Input.GetKeyDown (chatOnJoystickDown) ) {
				OnChatKeyDown ();
				keyDown = true;
			} else {
				keyDown = false;
			}
		}

		void CheckScrollingTextStatus ()
		{
			if(startConversation) StartConversation ();
			if (textIsScrolling) {
				if(chatText.text == _NPCDialogue[connversationIndex].chatPages[currentPage]){
					textIsScrolling = false;
					if (buttonPage && _NPCDialogue[connversationIndex].enableButtonAfterDialogue[currentPage]   ) {
						for(int i = 0; i < (_NPCDialogue[connversationIndex].NPCButtons[currentPage].buttonComponent.Length); i++){
							if(_NPCDialogue[connversationIndex].NPCButtons[currentPage].buttonComponent[i] == NPCDialogueButtons.ItemType.enableButton){
								_NPCDialogue[connversationIndex].page_ChatBox[currentPage].chatBoxComponents.buttonComponents[i].buttonObject.SetActive(true);
								Button tempButton = _NPCDialogue[connversationIndex].page_ChatBox[currentPage].chatBoxComponents.buttonComponents[i].button;
								tempButton.onClick = _NPCDialogue[connversationIndex].NPCButtons[currentPage].NPCClick[i];
								Text tempButtonText = tempButton.GetComponentInChildren<Text>();
								tempButtonText.text = _NPCDialogue[connversationIndex].NPCButtons[currentPage].buttonString[i];
								tempButton = null;
								tempButtonText = null;
							}
						}
						_NPCDialogue[connversationIndex].OnPageEndEvent[currentPage].Invoke();
						Invoke ("EnableEventSystemNavigation", 0.1f);
					}						
				}
			}
		}

		void CheckPlayerStatus ()
		{
			if(player){
				if (_NPCChatOptions.distanceCheck) {
					offsetValue = new Vector3 (transform.position.x - _NPCChatOptions.distanceCheckOffset.x, transform.position.y - _NPCChatOptions.distanceCheckOffset.y, transform.position.z - _NPCChatOptions.distanceCheckOffset.z);
					var dist = Vector3.Distance (player.position, offsetValue);
					if (dist <= _NPCChatOptions.distanceToChat) {
						canChat = true;
						if (_NPCChatOptions.outline) {
							if (talking) {
								SetOutline (_NPCChatOptions.dialogueColor, _NPCChatOptions.dialogueSize, _NPCChatOptions.dialogueColor, _NPCChatOptions.dialogueSpriteSize);
							} else {
								if (mouseOver && !talking && canChat) SetOutline (_NPCChatOptions.mouseOverColor, _NPCChatOptions.mouseOverSize, _NPCChatOptions.mouseOverColor, _NPCChatOptions.mouseOverSpriteSize);
								else SetOutline (_NPCChatOptions.inRangeColor, _NPCChatOptions.inRangeSize, _NPCChatOptions.inRangeColor, _NPCChatOptions.inRangeSpriteSize);
							}
						}
					} else {
						canChat = false;
					}
				}
				if (canChat == false) {
					if (_NPCChatOptions.outline) SetOutline (_NPCChatOptions.outOfRangeColor, _NPCChatOptions.outOfRangeSize, _NPCChatOptions.outOfRangeColor, _NPCChatOptions.outOfRangeSpriteSize);
					if(talking){
						chatManager.dialogueIsActive = false;
						if(tempClip) Destroy (tempClip);
						chatText.text = "";
						talking = false;
						textIsScrolling = false;
						currentPage = 0;
						OnStopChatEvent.Invoke ();
						for (var i = 0; i < _NPCDialogue[connversationIndex].page_ChatBox.Length; i++) _NPCDialogue[connversationIndex].page_ChatBox[i].CloseChatBox(); //_gameObject.SetActive(false);
					}
				}
			}
		}

		void CheckIfChatPageShouldClose ()
		{
			if ((_NPCChatOptions.closeOnMouseOrKeyUp && talking) && (Input.GetMouseButtonUp(0) || Input.GetKeyUp(_NPCChatOptions.chatOnKeyUp) || Input.GetKeyUp(_NPCChatOptions.chatOnKeyDown))) FinishPage ();
		}

		void NPCChatUpdate()
		{
			buttonPage = false;
			speakerNameText = _NPCDialogue[connversationIndex].page_ChatBox[currentPage].chatBoxComponents.titleText;
			speakerNameText.text = _NPCDialogue[connversationIndex].NPCName[currentPage];
			_NPCDialogue[connversationIndex].OnPageStartEvent[currentPage].Invoke();
			if (tempClip != null) Destroy(tempClip);
			if(_NPCDialogue[connversationIndex].pageAudio[currentPage] != null){
				tempClip = new GameObject ();
				tempClip.transform.parent = transform;
				tempClip.AddComponent<AudioSource> ();
				tempClip.name = "NPC Page Audio";
				if(_NPCDialogue[connversationIndex].loopAudio[currentPage])
					tempClip.GetComponent<AudioSource>().loop = true;
				tempClip.AddComponent<AudioDestroy>();
				tempClip.GetComponent<AudioSource>().clip = _NPCDialogue[connversationIndex].pageAudio[currentPage];
				tempClip.GetComponent<AudioSource> ().Play ();
			}
			if (currentPage > 0) _NPCDialogue [connversationIndex].page_ChatBox [currentPage - 1].CloseChatBox();
			for(int ia = 0; ia < (_NPCDialogue[connversationIndex].NPCButtons[currentPage].buttonComponent.Length); ia++)
			{
				_NPCDialogue[connversationIndex].page_ChatBox[currentPage].chatBoxComponents.buttonComponents[ia].buttonObject.SetActive(false);
			}
			_NPCDialogue[connversationIndex].page_ChatBox[currentPage].chatBoxComponents.dialogueText.text = "";
			chatText = _NPCDialogue[connversationIndex].page_ChatBox[currentPage].chatBoxComponents.dialogueText;
			_NPCDialogue[connversationIndex].page_ChatBox[currentPage].OpenChatBox(); //_gameObject.SetActive(true);
			for(int i = 0; i < (_NPCDialogue[connversationIndex].NPCButtons[currentPage].buttonComponent.Length); i++){
				if(_NPCDialogue[connversationIndex].NPCButtons[currentPage].buttonComponent[i] == NPCDialogueButtons.ItemType.enableButton){
					buttonPage = true;
					if (!_NPCDialogue[connversationIndex].enableButtonAfterDialogue[currentPage] ) {
						_NPCDialogue [connversationIndex].page_ChatBox [currentPage].chatBoxComponents.buttonComponents [i].buttonObject.SetActive (true);
						Invoke ("EnableEventSystemNavigation", 0.1f);
						Button tempButton = _NPCDialogue [connversationIndex].page_ChatBox [currentPage].chatBoxComponents.buttonComponents [i].button;
						tempButton.onClick = _NPCDialogue [connversationIndex].NPCButtons [currentPage].NPCClick [i];
						Text tempButtonText = tempButton.GetComponentInChildren<Text> ();
						tempButtonText.text = _NPCDialogue [connversationIndex].NPCButtons [currentPage].buttonString [i];
						tempButton = null;
						tempButtonText = null;
					}
				}
			}
		}

		void EnableEventSystemNavigation () {
			if (_NPCDialogue [connversationIndex].page_ChatBox [currentPage].eventSystemNavigation.useEventSystemNavigation)
				_NPCDialogue [connversationIndex].page_ChatBox [currentPage].eventSystemNavigation.eventSystem.sendNavigationEvents = true;
		}
		#endregion

		public void FinishPage(){
			if (talking)
			{
				if (textIsScrolling) {
					textIsScrolling = false;
					chatText.text = _NPCDialogue [connversationIndex].chatPages [currentPage];
					if (buttonPage && _NPCDialogue [connversationIndex].enableButtonAfterDialogue [currentPage]) {
						for (int i = 0; i < (_NPCDialogue [connversationIndex].NPCButtons [currentPage].buttonComponent.Length); i++) {
							if (_NPCDialogue [connversationIndex].NPCButtons [currentPage].buttonComponent [i] == NPCDialogueButtons.ItemType.enableButton) {
								_NPCDialogue [connversationIndex].page_ChatBox [currentPage].chatBoxComponents.buttonComponents [i].buttonObject.SetActive (true);
								Button tempButton = _NPCDialogue [connversationIndex].page_ChatBox [currentPage].chatBoxComponents.buttonComponents [i].button;
								tempButton.onClick = _NPCDialogue [connversationIndex].NPCButtons [currentPage].NPCClick [i];
								Text tempButtonText = tempButton.GetComponentInChildren<Text> ();
								tempButtonText.text = _NPCDialogue [connversationIndex].NPCButtons [currentPage].buttonString [i];
								tempButton = null;
								tempButtonText = null;
							}
						}
						_NPCDialogue [connversationIndex].OnPageEndEvent [currentPage].Invoke ();
						Invoke ("EnableEventSystemNavigation", 0.1f);
					}
				} else {
					if (currentPage < _NPCDialogue [connversationIndex].chatPages.Length - 1) {
						currentPage++;
						colorString = tempFormattedString1 = tempFormattedString2 = finalFormattedString = "";
						NPCChatUpdate ();
						StartCoroutine (ScrollPageText ());
					} else {
						if (!buttonPage) CloseChat ();
					}
				}
			}
		}

		public void CloseChat()
		{
			_NPCDialogue[ connversationIndex ].OnPageEndEvent[currentPage].Invoke();
			OnStopChatEvent.Invoke ();
			if(tempClip) Destroy (tempClip);
	        currentPage = 0;
			chatText.text = _NPCDialogue[ connversationIndex ].chatPages[currentPage];
			Invoke ("ConversationComplete", 0.1f);
			talking = false;
			for (var i = 0; i < _NPCDialogue[ connversationIndex ].page_ChatBox.Length; i++){
				_NPCDialogue[ connversationIndex ].page_ChatBox[i].CloseChatBox();
			}
			for (int j = 0; j < (_NPCDialogue[ connversationIndex ].NPCButtons[currentPage].buttonComponent.Length); j++){
				_NPCDialogue[ connversationIndex ].page_ChatBox[currentPage].chatBoxComponents.buttonComponents[j].buttonObject.SetActive(false);
			}
			if (_NPCDialogue[ connversationIndex ].useNextDialogue && !buttonPage)	chatManager.npcConversationIndexes[nPCNumber] = _NPCDialogue[ connversationIndex ].nextDialogue;
			if (chatManager != null) chatManager.dialogueIsActive = false;
			if (_NPCChatOptions.disableOnComplete) gameObject.SetActive(false);
			if (_NPCChatOptions.destroyOnComplete) Destroy(this.gameObject);
		}

		public void ConversationComplete() { talking = false; }
			
		void OnMouseEnter() { mouseOver = true; }

		void OnMouseExit() { mouseOver = false; }

		void OnMouseUp() { if (_NPCChatOptions.chatOnMouseUp) Invoke("StartConversation", 0.1f); }

		void OnTriggerEnter(Collider col) {	if(_NPCChatOptions.chatOnCollision && col.tag == "Player") StartConversation (); }

		void OnTriggerExit(Collider col) { if(_NPCChatOptions.closeOnTriggerExit && col.tag == "Player") CloseChat (); }

		void OnTriggerEnter2D(Collider2D col) { if(_NPCChatOptions.chatOnCollision && col.tag == "Player") StartConversation (); }

		void OnTriggerExit2D(Collider2D col) { if(_NPCChatOptions.closeOnTriggerExit && col.tag == "Player" && talking) CloseChat (); }

		void OnChatKeyUp() { StartConversation (); }

		void OnChatKeyDown(){
			if (keyDown == false) StartConversation ();
			else FinishPage ();
		}

		public void StartConversation()
		{
			if (canChat && talking == false)
			{
				if (chatManager != null && _NPCChatOptions.allowChatIfOtherIsActive == false)
				{
					if (chatManager.dialogueIsActive == false)
					{
						chatManager.dialogueIsActive = true;
						Invoke ("NPCChatStart", 0.1f);
					}
				}
				else
				{
					if (chatManager != null) chatManager.dialogueIsActive = true;
					Invoke ("NPCChatStart", 0.1f);
				}
			}
		}

		public void NPCChatStart()
		{
			colorString = tempFormattedString1 = tempFormattedString2 = finalFormattedString = "";
			if (chatType == ChatType.Default)
			{
				connversationIndex = 0;
			}
			else if (chatType == ChatType.Structured)
			{
				if (chatManager.npcConversationIndexes.Length == 0)
				{
					Debug.LogError
					(
						"This NPC is using structured chat, but the chat manager does not have any conversation indexes defined, NPC Chat will not start."
					);
					return;
				}
				else if (chatManager.npcConversationIndexes.Length < nPCNumber)
				{
					Debug.LogError
					(
						"This NPC is using structured chat, but the chat manager has less conversations defined than required by this NPC's number" + "\n" +
						" NPC Chat will not start."
					);
					return;
				}
				else if (chatManager.npcConversationIndexes [nPCNumber] + 1 > _NPCDialogue.Length)
				{
					Debug.LogError
					(
						"This NPC is using chat manager index " + nPCNumber + " it has a value of " + chatManager.npcConversationIndexes [nPCNumber] + "\n" +
						"but this npc has fewer conversation indexes available, NPC Chat will not start."
					);
					return;
				}
				else
				{
					connversationIndex = chatManager.npcConversationIndexes [nPCNumber];
				}
					
			}
			OnChatEvent.Invoke ();
			startConversation = false;
			talking = true;
			currentPage = 0;
			NPCChatUpdate();
			StartCoroutine (ScrollPageText());
		}
		
		IEnumerator ScrollPageText(){
			textIsScrolling = true;
			startLine = currentPage;
			if (_NPCChatOptions.scrollingText) {
				if (chatManager != null && !string.IsNullOrEmpty (_NPCDialogue [connversationIndex].chatPages [currentPage])) {
					_NPCDialogue [connversationIndex].chatPages [currentPage] = _NPCDialogue [connversationIndex].chatPages [currentPage].Replace ("<PLAYERNAME>", chatManager.playerName);
				}
				for (int i = 0; i < _NPCDialogue [connversationIndex].chatPages [currentPage].Length; i++) {
					if (talking && textIsScrolling && currentPage == startLine) {
						currentIndex = RTFCharacterCheck (i);
						if (currentIndex != i) i = currentIndex;
						for (int j = 0; j < 4; j++) {
							if(boldOpen || italicOpen || colorOpen || sizeOpen){
								currentIndex = RTFCharacterCheck (i);
								if (currentIndex != i) i = currentIndex;
							}
						}
						if (boldOpen && !bold) {
							SetFormattedString ();
							boldOpen = false;
							boldPrefix = boldSuffix = "";
						}
						if (italicOpen && !italic) {
							SetFormattedString ();
							italicOpen = false;
							italicPrefix = italicSuffix = "";
						}
						if (colorOpen && !color) {
							SetFormattedString ();
							colorOpen = false;
							colorString = colorPrefix = colorSuffix = "";
						}
						if (sizeOpen && !size) {
							SetFormattedString ();
							sizeOpen = false;
							sizeString = sizePrefix = sizeSuffix = "";
						}
						if (boldOpen || italicOpen || colorOpen || sizeOpen) {
							tempFormattedString1 += _NPCDialogue [connversationIndex].chatPages [currentPage] [i];
							tempFormattedString2 = boldPrefix + italicPrefix + colorPrefix + sizePrefix + tempFormattedString1 + sizeSuffix + colorSuffix + italicSuffix + boldSuffix;
							charactersFormatted += 1;
						} else {
							SetFormattedString ();
							finalFormattedString += _NPCDialogue [connversationIndex].chatPages [currentPage] [i];
						}
						chatText.text = finalFormattedString + tempFormattedString2;
						yield return new WaitForSeconds (_NPCChatOptions.textScrollSpeed / 100f);
					}
				}
			} else {
				displayText = _NPCDialogue [connversationIndex].chatPages [currentPage];
				chatText.text = displayText;
			}
			textIsScrolling = false;
		}

		void SetOutline (Color _outlineColor, float _outlineSize, Color _outOfRangeColor, float _OutOfRangeSpriteSize)
		{
			outlineMaterial.SetColor ("_OutlineColor", _outlineColor);
			outlineMaterial.SetFloat ("_OutlineSize", _outlineSize);
			outlineSpriteMaterial.SetColor ("_OutlineColor", _outOfRangeColor);
			outlineSpriteMaterial.SetFloat ("_OutlineSize", _OutOfRangeSpriteSize);
		}

		#region Text Formatting
		void SetFormattedString(){
			finalFormattedString += tempFormattedString2;
			if (tempFormattedString2 != "") tempFormattedString1 = tempFormattedString2 = "";
		}

		int RTFCharacterCheck(int index){
			int newIndex;
			newIndex = RTFBoldCheck (index);
			newIndex = RTFItalicCheck (newIndex);
			newIndex = RTFColorCheck (newIndex);
			newIndex = RTFSizeCheck (newIndex);
			return newIndex;
		}

		int RTFBoldCheck(int index){
			string s = _NPCDialogue [connversationIndex].chatPages [currentPage] [index].ToString ();
			int newIndex = index;
			if (s == "<") {
				s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 1].ToString ();
				if (s == "<b") {
					s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 2].ToString ();
					if (s == "<b>") {
						newIndex += 3;
						bold = boldOpen = true;
						boldPrefix = "<b>";
						boldSuffix = "</b>";
						if(charactersFormatted > 0) SetFormattedString ();
						charactersFormatted = 0;
					}
				} else if (s == "</") {
					s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 2].ToString ();
					if (s == "</b") {
						s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 3].ToString ();
						if (s == "</b>") {
							newIndex += 4;
							bold = false;
							boldSuffix = "";
						}
					}
				}
			}
			return newIndex;
		}

		int RTFItalicCheck(int index){
			string s = _NPCDialogue [connversationIndex].chatPages [currentPage] [index].ToString ();
			int newIndex = index;
			if (s == "<") {
				s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 1].ToString ();
				if (s == "<i") {
					s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 2].ToString ();
					if (s == "<i>") {
						newIndex += 3;
						italic = italicOpen = true;
						italicPrefix = "<i>";
						italicSuffix = "</i>";
						if(charactersFormatted > 0) SetFormattedString ();
						charactersFormatted = 0;
					}
				} else if (s == "</") {
					s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 2].ToString ();
					if (s == "</i") {
						s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 3].ToString ();
						if (s == "</i>") {
							newIndex += 4;
							italic = false;
							italicSuffix = "";

						}
					}
				}
			}
			return newIndex;
		}

		int RTFColorCheck(int index){
			string s = _NPCDialogue [connversationIndex].chatPages [currentPage] [index].ToString ();
			int newIndex = index;
			if (s == "<") {
				s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 1].ToString ();
				if (s == "<c") {
					s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 2].ToString ();
					if (s == "<co") {
						color = colorOpen = true;
						newIndex += 7;
						StringCheck (_NPCDialogue [connversationIndex].chatPages [currentPage] [newIndex].ToString (), newIndex, "color");
						newIndex += colorString.Length + 1;
						colorPrefix = "<color=" + colorString + ">";
						colorSuffix = "</color>";
						if(charactersFormatted > 0) SetFormattedString ();
						charactersFormatted = 0;
					}
				} else if (s == "</") {
					s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 2].ToString ();
					if (s == "</c") {
						s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 3].ToString ();
						if (s == "</co") {
							newIndex += 8;
							color = false;
							colorSuffix = "";
						}
					}
				}
			}
			return newIndex;
		}

		int RTFSizeCheck(int index){
			string s = _NPCDialogue [connversationIndex].chatPages [currentPage] [index].ToString ();
			int newIndex = index;
			if (s == "<") {
				s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 1].ToString ();
				if (s == "<s") {
					s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 2].ToString ();
					if (s == "<si") {
						size = sizeOpen = true;
						newIndex += 6;
						StringCheck (_NPCDialogue [connversationIndex].chatPages [currentPage] [newIndex].ToString (), newIndex, "size");
						newIndex += sizeString.Length + 1;
						sizePrefix = "<size=" + sizeString + ">";
						sizeSuffix = "</size>";
						if(charactersFormatted > 0) SetFormattedString ();
						charactersFormatted = 0;
					}
				} else if (s == "</") {
					s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 2].ToString ();
					if (s == "</s") {
						s += _NPCDialogue [connversationIndex].chatPages [currentPage] [index + 3].ToString ();
						if (s == "</si") {
							newIndex += 7;
							size = false;
							sizeSuffix = "";
						}
					}
				}
			}
			return newIndex;
		}

		void StringCheck(string s1, int index, string typeCheck){
			if (typeCheck == "color") {
				if (s1 != ">") {
					colorString += s1;
					index += 1;
					StringCheck (_NPCDialogue [connversationIndex].chatPages [currentPage] [index].ToString (), index, "color");
				}
			}else if (typeCheck == "size"){
				if (s1 != ">") {
					sizeString += s1;
					index += 1;
					StringCheck (_NPCDialogue [connversationIndex].chatPages [currentPage] [index].ToString (), index, "size");
				}
			}
		}
		#endregion

		void OnDrawGizmosSelected()
		{
			if (_NPCChatOptions != null)
			{
				if (_NPCChatOptions.distanceCheck)
				{
					Gizmos.color = gizmoColor;
					offsetValue = new Vector3 (transform.position.x - _NPCChatOptions.distanceCheckOffset.x, transform.position.y - _NPCChatOptions.distanceCheckOffset.y, transform.position.z - _NPCChatOptions.distanceCheckOffset.z);
					Gizmos.DrawSphere (offsetValue, _NPCChatOptions.distanceToChat);
				}
			}
		}

		public void CalculateArrays(){			
			for(int i = 0; i < _NPCDialogue.Length; i++){
				if(_NPCDialogue[i] != null){
					if(canUpdatePages && tempConv == i){
						_NPCDialogue [i].pagesOfChat = tempInt;
						canUpdatePages = false;
					}
					System.Array.Resize (ref _NPCDialogue[i].page_ChatBox, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].enableButtonAfterDialogue, _NPCDialogue[i].pagesOfChat);
					//System.Array.Resize (ref _NPCDialogue[i].chatBoxes, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].chatPages, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].NPCName, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].NPCButtons, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].pageAudio, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].loopAudio, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].OnPageStartEvent, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].OnPageEndEvent, _NPCDialogue[i].pagesOfChat);
					for (int ia = 0; ia < _NPCDialogue[i].pagesOfChat; ia++){
						if(_NPCDialogue[i].NPCButtons[ia] != null){
							System.Array.Resize (ref _NPCDialogue[i].NPCButtons[ia].buttonComponent, 6);
							System.Array.Resize (ref _NPCDialogue[i].NPCButtons[ia].NPCClick, 6);
							System.Array.Resize (ref _NPCDialogue[i].NPCButtons[ia].buttonString, 6);
						}
					}
				}
			}
		}

		public void UpdateConversations(){
			if (conversations == 0) conversations = 1;				
			System.Array.Resize (ref _NPCDialogue, conversations);
			CalculateArrays();
		}



	}
}
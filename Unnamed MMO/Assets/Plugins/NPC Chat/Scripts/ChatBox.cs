namespace TurnTheGameOn.NPCChat
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using System.Collections;

	[ExecuteInEditMode()] public class ChatBox : MonoBehaviour
	{
		public bool useRenderTexture;
		[System.Serializable] public class ChatBoxComponents
		{
			public Text titleText;
			public Text dialogueText;
			public Camera renderTextureCamera;
			public ButtonComponents[] buttonComponents;
		}
		
		[System.Serializable] public class ButtonComponents
		{
			public GameObject buttonObject;
			public Button button;
		}
		private GameObject gameObjectCached;		
		public ChatBoxComponents chatBoxComponents;
		[System.Serializable] public class EventSystemNavigation
		{
			public bool useEventSystemNavigation;
			public EventSystem eventSystem;
			public GameObject firstSelection;
			public GameObject selected;
		}
		public EventSystemNavigation eventSystemNavigation;
		[System.Serializable] public class LerpSettings
		{
			public bool useLerpIn;
			public bool useLerpOut;
			public AnimationCurve lerpCurve;
			public Vector2 startPosition;
			public Vector2 endPosition;
			public float lerpDuration = 0.5f;
		}
		public LerpSettings lerpSettings;
		private RectTransform rectTransformCached;
		private float elapsedTime;
		private float lerpProgress;
		private Vector2 tempPosition;


		void OnDisable ()
		{
			if (eventSystemNavigation.eventSystem) eventSystemNavigation.eventSystem.sendNavigationEvents = true;
			StopAllCoroutines();
		}	

		void Update ()
		{
			if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1) || Input.GetMouseButtonDown (2))
			{
				if (!eventSystemNavigation.eventSystem)
				{
					SetChatBoxButtonSelection ();
				}
				else
				{
					eventSystemNavigation.eventSystem.SetSelectedGameObject (null);
					eventSystemNavigation.eventSystem.SetSelectedGameObject (eventSystemNavigation.selected);
					//selected.GetComponent<Button> ().Select ();
				}
			}
			else if (eventSystemNavigation.eventSystem)
			{
				if (eventSystemNavigation.eventSystem.currentSelectedGameObject)
				{
					eventSystemNavigation.selected = eventSystemNavigation.eventSystem.currentSelectedGameObject;
					eventSystemNavigation.eventSystem.SetSelectedGameObject (null);
					eventSystemNavigation.eventSystem.SetSelectedGameObject (eventSystemNavigation.selected);
					eventSystemNavigation.selected.GetComponent<Button> ().Select ();
				}
				else
				{
					SetChatBoxButtonSelection ();
				}
			}
			else
			{
				SetChatBoxButtonSelection ();
			}
		}

		public void OpenChatBox ()
		{
			if (useRenderTexture) chatBoxComponents.renderTextureCamera.gameObject.SetActive(true);
			if (gameObjectCached == null) gameObjectCached = gameObject;
			gameObjectCached.SetActive (true);
			if (lerpSettings.useLerpIn)
			{
				if (rectTransformCached == null) rectTransformCached = GetComponent<RectTransform> ();
				StartCoroutine(LerpRectFromStartPositionToEndPosition(lerpSettings.startPosition, lerpSettings.endPosition, lerpSettings.lerpDuration));
			}
		}
		
		private IEnumerator LerpRectFromStartPositionToEndPosition(Vector2 startPosition, Vector2 endPosition, float time)
		{
			elapsedTime = 0;
			while (elapsedTime < time)
			{
				lerpProgress = lerpSettings.lerpCurve.Evaluate ( NPCChat_Math.LinearDistance (0.0f, lerpSettings.lerpDuration, (elapsedTime / time) )    );
				tempPosition = Vector2.Lerp(startPosition, endPosition, lerpProgress);
				rectTransformCached.anchoredPosition = tempPosition;
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			tempPosition = endPosition;
			rectTransformCached.anchoredPosition = tempPosition;
		}

		public void CloseChatBox ()
		{
			if (useRenderTexture) chatBoxComponents.renderTextureCamera.gameObject.SetActive(false);
			if (gameObjectCached == null) gameObjectCached = gameObject;
			if (lerpSettings.useLerpOut && gameObjectCached.activeInHierarchy)
			{
				if (rectTransformCached == null) rectTransformCached = GetComponent<RectTransform>();
				StartCoroutine(LerpRectFromEndPositionToStartPosition(lerpSettings.startPosition, lerpSettings.endPosition, lerpSettings.lerpDuration));
			}
			else
			{
				gameObjectCached.SetActive (false);
			}
		}

		private IEnumerator LerpRectFromEndPositionToStartPosition(Vector2 startPosition, Vector2 endPosition, float time)
		{
			elapsedTime = 0;
			while (elapsedTime < time)
			{
				lerpProgress = lerpSettings.lerpCurve.Evaluate ( NPCChat_Math.LinearDistance (0.0f, lerpSettings.lerpDuration, (elapsedTime / time) )    );
				tempPosition = Vector2.Lerp(endPosition, startPosition, lerpProgress);
				rectTransformCached.anchoredPosition = tempPosition;
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			tempPosition = startPosition;
			rectTransformCached.anchoredPosition = tempPosition;
			gameObjectCached.SetActive (false);
		}

		[ContextMenu("SetChatBoxButtonSelection")] public void SetChatBoxButtonSelection ()
		{
			if (eventSystemNavigation.useEventSystemNavigation)
			{
				if (!eventSystemNavigation.eventSystem) eventSystemNavigation.eventSystem = GameObject.FindObjectOfType<EventSystem> ();
				eventSystemNavigation.eventSystem.sendNavigationEvents = false;
				if (eventSystemNavigation.firstSelection) eventSystemNavigation.eventSystem.firstSelectedGameObject = eventSystemNavigation.firstSelection;
				if (!eventSystemNavigation.eventSystem.firstSelectedGameObject) Debug.LogWarning ("The chat box has useeventSystemNavigation enabled but the first selection is not assigned.");
				eventSystemNavigation.eventSystem.SetSelectedGameObject (eventSystemNavigation.firstSelection);
				eventSystemNavigation.selected = eventSystemNavigation.firstSelection;
				eventSystemNavigation.selected.GetComponent<Button> ().Select ();
			}
		}
		
		
	
	}
}
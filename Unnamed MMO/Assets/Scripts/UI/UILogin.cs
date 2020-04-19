using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Acemobe.MMO.UI
{
    public class UILogin : MonoBehaviour
    {
        static UILogin _instance;
        public static UILogin instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public NetworkManager manager;

        public TMP_InputField username;
        public TMP_InputField password;

        public TextMeshProUGUI error;

        public GameObject connectionLayer;
        public GameObject debug;

        public List<TMP_InputField> tabInput;
        int curTabInput = 0;

        // Start is called before the first frame update
        void Start()
        {
            _instance = this;

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                debug.SetActive(true);
            }
            else
            {
                debug.SetActive(false);
            }

            username.text= PlayerPrefs.GetString("username");
            password.text = PlayerPrefs.GetString("password");

            tabInput[curTabInput].OnPointerClick(new UnityEngine.EventSystems.PointerEventData(EventSystem.current));
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                curTabInput = (++curTabInput) % tabInput.Count;
                tabInput[curTabInput].OnPointerClick(new UnityEngine.EventSystems.PointerEventData(EventSystem.current));
            }
        }

        public void onConnect()
        {
            connectionLayer.SetActive(false);

            PlayerPrefs.SetString("username", username.text);
            PlayerPrefs.SetString("password", password.text);

            manager.StartClient();
        }

        public void onStartLocalServer()
        {
            manager.StartHost();
            gameObject.SetActive(false);

            connectionLayer.SetActive(false);
        }

        public void onConnectLocalServer()
        {
            manager.networkAddress = "localhost";
            manager.StartClient();

            connectionLayer.SetActive(false);
        }
    }
}
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
        public MMOAuthenticator authenticator;

        public TMP_InputField username;
        public TMP_InputField password;

        public TMP_Text error;

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

//            debug.SetActive(true);

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

        // show connection error
        public void onBadAuth()
        {
            gameObject.SetActive(true);
            connectionLayer.SetActive(true);

            manager.StopHost();

            error.text = "Invalid Username / Password combination";
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
            PlayerPrefs.SetString("username", username.text);
            PlayerPrefs.SetString("password", password.text);

            manager.StartHost();

            connectionLayer.SetActive(false);
        }

        public void onConnectLocalServer()
        {
            PlayerPrefs.SetString("username", username.text);
            PlayerPrefs.SetString("password", password.text);

            manager.networkAddress = "localhost";
            manager.StartClient();

            connectionLayer.SetActive(false);
        }
    }
}
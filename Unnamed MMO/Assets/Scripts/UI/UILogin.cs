using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Acemobe.MMO
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

        // Start is called before the first frame update
        void Start()
        {
            _instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void onConnect()
        {
            manager.StartClient();

            connectionLayer.SetActive(false);
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
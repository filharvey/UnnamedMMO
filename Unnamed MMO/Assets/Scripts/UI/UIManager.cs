using Acemobe.MMO.Data;
using Acemobe.MMO.UI.UIItems;
using Mirror;
using UnityEngine;

namespace Acemobe.MMO.UI
{
    public class UIManager : MonoBehaviour
    {
        static UIManager _instance;

        public UIActionBar actionBarUI;

        public UIInventory inventoryUI;

        public UILogin loginUI;

        public UIGame gameUI;

        public UICraft craftUI;

        public GameObject mobileUI;

        public GameObject dragLayer;

        public TMPro.TMP_Text ping;

        public static UIManager instance
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

        // Start is called before the first frame update
        void Start()
        {
            _instance = this;
            actionBarUI.gameObject.SetActive(false);
            inventoryUI.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(false);
            craftUI.gameObject.SetActive(false);

            mobileUI.SetActive(false);

            if (MMONetworkManager.isHeadless)
                loginUI.gameObject.SetActive(false);
            else
                loginUI.gameObject.SetActive(true);
        }

        private void Update()
        {
            ping.text = "Ping: " + (int)(NetworkTime.rtt * 1000);
        }

        public void showItemGain (MMOInventoryItem item)
        {
            gameUI.addNotification(item);
        }
    }
}
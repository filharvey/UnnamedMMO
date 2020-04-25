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

        public UIChat chatUI;

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
            gameUI.gameObject.SetActive(false);
            mobileUI.SetActive(false);

            hideAllUIPanels();

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

        public void hideAllUIPanels ()
        {
            UIManager.instance.craftUI.gameObject.SetActive(false);
            UIManager.instance.chatUI.gameObject.SetActive(false);
            UIManager.instance.inventoryUI.gameObject.SetActive(false);
        }
    }
}
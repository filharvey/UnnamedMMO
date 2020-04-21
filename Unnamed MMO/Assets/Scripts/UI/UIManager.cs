using Acemobe.MMO.Data;
using Acemobe.MMO.UI.UIItems;
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

            loginUI.gameObject.SetActive(true);
        }

        public void showItemGain (MMOInventoryItem item)
        {
            gameUI.addNotification(item);
        }
    }
}
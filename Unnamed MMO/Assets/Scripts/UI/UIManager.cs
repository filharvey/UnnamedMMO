using Acemobe.MMO.Data;
using Acemobe.MMO.UI.UIItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class UIManager : MonoBehaviour
    {
        static UIManager _instance;

        public UIActionBar actionBar;

        public UIInventory inventory;

        public UILogin login;

        public UIGame gameUI;

        public GameObject mobileUI;

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
            actionBar.gameObject.SetActive(false);
            inventory.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(false);

            mobileUI.SetActive(false);

            login.gameObject.SetActive(true);
        }

        public void showItemGain (MMOInventoryItem item)
        {
            gameUI.addNotification(item);
        }
    }
}
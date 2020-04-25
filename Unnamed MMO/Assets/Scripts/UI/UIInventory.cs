using Acemobe.MMO.Data;
using Acemobe.MMO.Objects;
using Acemobe.MMO.UI.UIItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.UI
{
    public class UIInventory : MonoBehaviour
    {
        public GameObject itemContainer;
        public List<UIItemInfo> inventoryItems;

        void Start()
        {
            for (var a = 0; a < itemContainer.transform.childCount; a++)
            {
                Transform obj = itemContainer.transform.GetChild(a);
                UIItemInfo itemInfo = obj.GetComponent<UIItemInfo>();

                itemInfo.idx = a;
                itemInfo.isInventory = true;

                itemInfo.setParent(this);
                inventoryItems.Add(itemInfo);
            }
        }

        public void onShow ()
        {
            if (gameObject.activeInHierarchy == true)
            {
                onClose();
            }
            else
            {
                UIManager.instance.hideAllUIPanels();

                gameObject.SetActive(true);
                updateInventory();
            }
        }

        public void onClose()
        {
            gameObject.SetActive(false);
        }

        void Update()
        {
            updateInventory();

            // need to move
            if (MMOPlayer.localPlayer.player.GetButtonDown("Escape"))
            {
                onClose();
            }
        }

        public void onPressed(UIItemInfo item)
        {

        }

        public void updateInventory ()
        {
            if (MMOPlayer.localPlayer)
            {
                for (var a = 0; a < inventoryItems.Count; a++)
                {
                    UIItemInfo bar = inventoryItems[a];

                    if (a < MMOPlayer.localPlayer.inventory.inventory.Count)
                    {
                        MMOInventoryItem itemInfo = MMOPlayer.localPlayer.inventory.inventory[a];

                        bar.updateItem(itemInfo);
                    }
                    else
                    {
                        bar.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
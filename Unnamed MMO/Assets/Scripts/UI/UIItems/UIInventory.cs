using Acemobe.MMO.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class UIInventory : MonoBehaviour
    {
        public GameObject itemContainer;
        public List<UIItemInfo> inventoryItems;

        // Start is called before the first frame update
        void Start()
        {
            for (var a = 0; a < itemContainer.transform.childCount; a++)
            {
                Transform obj = itemContainer.transform.GetChild(a);
                UIItemInfo itemInfo = obj.GetComponent<UIItemInfo>();

                inventoryItems.Add(itemInfo);
            }
        }

        void Update()
        {
            updateInventory();
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
                        bar.clear();
                    }
                }
            }
        }

        public void onClose ()
        {
            gameObject.SetActive (false);
        }
    }
}
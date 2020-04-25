using Acemobe.MMO.Data;
using Acemobe.MMO.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.UI.UIItems
{
    public class UIActionBar : MonoBehaviour
    {
        public List<UIActionBarItemInfo> actionBarItems;

        // Start is called before the first frame update
        void Start()
        {
            for (var a = 0; a < actionBarItems.Count; a++)
            {
                UIActionBarItemInfo bar = actionBarItems[a];

                bar.setParent(this);
                bar.idx = a;
                bar.isInventory = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            updateActionBar();
        }

        public void onPressed (UIItemInfo item)
        {

        }

        public void updateActionBar ()
        {
            if (MMOPlayer.localPlayer)
            {
                int count = MMOPlayer.localPlayer.inventory.actionBar.Count;
                for (var a = 0; a < actionBarItems.Count; a++)
                {
                    UIActionBarItemInfo bar = actionBarItems[a];
                        
                    if (a < MMOPlayer.localPlayer.inventory.actionBar.Count)
                    {
                        MMOInventoryItem itemInfo = MMOPlayer.localPlayer.inventory.actionBar[a];

                        bar.updateItem(itemInfo);
                    }
                }
            }
        }
    }
}
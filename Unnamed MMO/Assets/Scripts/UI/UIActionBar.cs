using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
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
            }
        }

        // Update is called once per frame
        void Update()
        {
            int a = 0;

            if (MMOPlayer.localPlayer)
            {
                // go through items in action bar, and update the UI
                for (; a < MMOPlayer.localPlayer.inventory.actionBar.Count; a++)
                {
                    UIActionBarItemInfo bar = actionBarItems[a];
                    Item itemInfo = MMOPlayer.localPlayer.inventory.actionBar[a];

                    bar.updateItem(itemInfo);
                }

                for (; a < 10; a++)
                {
                    UIActionBarItemInfo bar = actionBarItems[a];
                    bar.clear();
                }
            }
        }

        public void onPressed (UIItemInfo item)
        {

        }

        public void updateActionBar ()
        {
            for (var a = 0; a < actionBarItems.Count; a++)
            {
                UIActionBarItemInfo bar = actionBarItems[a];

                if (a < MMOPlayer.localPlayer.inventory.actionBar.Count)
                {
                    Item itemInfo = MMOPlayer.localPlayer.inventory.actionBar[a];

                    bar.updateItem(itemInfo);
                }
            }
        }
    }
}
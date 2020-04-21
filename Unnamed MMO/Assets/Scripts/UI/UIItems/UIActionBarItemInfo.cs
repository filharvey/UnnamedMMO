using Acemobe.MMO.Data;
using Acemobe.MMO.MMOObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Acemobe.MMO.UI.UIItems
{
    public class UIActionBarItemInfo : UIItemInfo
    {
        public Image timer;

        UIActionBar actionBar;

        public override void updateItem(MMOInventoryItem itemInfo)
        {
            base.updateItem(itemInfo);

            if (itemInfo.type == MMOItemType.None)
                timer.gameObject.SetActive (false);  
            else
                timer.gameObject.SetActive(false);
        }

        public void setParent(UIActionBar actionBar)
        {
            this.actionBar = actionBar;
        }

    }
}

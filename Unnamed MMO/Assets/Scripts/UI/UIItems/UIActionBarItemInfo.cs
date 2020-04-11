using Acemobe.MMO.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Acemobe.MMO
{
    public class UIActionBarItemInfo : UIItemInfo
    {
        public Image timer;

        public override void clear()
        {
            base.clear();

            timer.enabled = false;
        }

        public override void updateItem(MMOInventoryItem itemInfo)
        {
            base.updateItem(itemInfo);

            timer.enabled = false;
        }
    }
}

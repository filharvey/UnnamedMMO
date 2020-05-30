using Acemobe.MMO.Data;
using Acemobe.MMO.Objects;
using UnityEngine.UI;

namespace Acemobe.MMO.UI.UIItems
{
    public class UIActionBarItemInfo : UIItemInfo
    {
        public Image timer;

        public Image selected;

        UIActionBar actionBar;

        public override void updateItem(MMOInventoryItem itemInfo)
        {
            base.updateItem(itemInfo);

            if (itemInfo.type == MMOItemType.None)
                timer.gameObject.SetActive (false);  
            else
                timer.gameObject.SetActive(false);

            // if selected
            if (MMOPlayer.localPlayer.inventory.activeItem == idx)
                selected.gameObject.SetActive(true);
            else
                selected.gameObject.SetActive(false);
        }

        public void setParent(UIActionBar actionBar)
        {
            this.actionBar = actionBar;
        }

    }
}

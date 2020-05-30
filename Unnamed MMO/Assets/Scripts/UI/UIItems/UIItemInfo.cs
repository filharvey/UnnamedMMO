using Acemobe.MMO.Data;
using Acemobe.MMO.Data.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Acemobe.MMO.UI.UIItems
{
    public class UIItemInfo : MonoBehaviour
    {
        public Image back;
        public Image item;
        public TextMeshProUGUI count;
        public int idx;
        public bool isInventory;

        public UIInventory inventoryBar;

        public MMOInventoryItem invetoryItem;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void onPressed ()
        {
            inventoryBar.onPressed(this);
        }

        public void setParent (UIInventory inventoryBar)
        {
            this.inventoryBar = inventoryBar;
        }

        public virtual void updateItem (MMOInventoryItem mmoItemInfo)
        {
            invetoryItem = mmoItemInfo;
            if (mmoItemInfo.type == MMOItemType.None)
            {
                item.gameObject.SetActive(false);
                count.gameObject.SetActive(false);
                return;
            }

            GameItem gameItem = MMOResourceManager.instance.getItemByType(invetoryItem.type);
            Sprite sprite = gameItem.icon;

            item.gameObject.SetActive(true);
            item.sprite = sprite;

            if (gameItem.maxStack > 1)
            {
                count.gameObject.SetActive(true);
                count.text = invetoryItem.amount.ToString();
            }
            else
            {
                count.gameObject.SetActive(false);
            }
        }
    }
}

using Acemobe.MMO.Data;
using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.UI.UIItems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Acemobe.MMO
{
    public class UIItemInfo : MonoBehaviour
    {
        public Image back;
        public Image item;
        public TextMeshProUGUI count;

        UIActionBar actionBar;

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
            actionBar.onPressed(this);
        }

        public void setParent (UIActionBar actionBar)
        {
            this.actionBar = actionBar;
        }

        public virtual void clear()
        {
            item.enabled = false;
            count.enabled = false;
        }

        public virtual void updateItem (MMOInventoryItem itemInfo)
        {
            GameItem gameItem = MMOResourceManager.instance.getItemByType(itemInfo.type);
            Sprite sprite = gameItem.icon;

            item.enabled = true;
            count.enabled = true;

            item.sprite = sprite;
            count.text = itemInfo.amount.ToString ();
        }
    }
}

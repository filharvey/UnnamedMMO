using System.Collections;
using System.Collections.Generic;
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

        public virtual void updateItem (Item itemInfo)
        {
            Sprite sprite = Resources.Load<Sprite>("icons/stone");

            item.enabled = true;
            count.enabled = true;

            switch (itemInfo.itemID)
            {
                case MMOResource.stone:
                    sprite = Resources.Load<Sprite>("icons/stone");
                    break;
                case MMOResource.wood:
                    sprite = Resources.Load<Sprite>("icons/wood");
                    break;
                case MMOResource.ironOre:
                    sprite = Resources.Load<Sprite>("icons/wood");
                    break;
            }

            item.sprite = sprite;
            count.text = itemInfo.amount.ToString ();
        }
    }
}

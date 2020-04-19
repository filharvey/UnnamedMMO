﻿using Acemobe.MMO.Data;
using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.UI.UIItems;
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
            item.gameObject.SetActive(false);
            count.gameObject.SetActive(false);
        }

        public virtual void updateItem (MMOInventoryItem itemInfo)
        {
            GameItem gameItem = MMOResourceManager.instance.getItemByType(itemInfo.type);
            Sprite sprite = gameItem.icon;

            item.gameObject.SetActive(true);
            item.sprite = sprite;

            if (gameItem.maxStack > 1)
            {
                count.gameObject.SetActive(true);
                count.text = itemInfo.amount.ToString();
            }
            else
            {
                count.gameObject.SetActive(false);
            }
        }
    }
}

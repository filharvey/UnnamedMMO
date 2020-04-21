using Acemobe.MMO.Data;
using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.MMOObjects;
using Acemobe.MMO.UI;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class SyncListItem : SyncList<MMOInventoryItem> { }

    public class MMOPlayerInventory : NetworkBehaviour
    {
        public SyncListItem inventory = new SyncListItem();
        public SyncListItem actionBar = new SyncListItem();

        public int activeItem = -1;

        void Start()
        {
            inventory.Callback += OnInventoryUpdated;
            actionBar.Callback += OnActionBarUpdated;
            syncMode = SyncMode.Owner;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            // add empty inventory
            for (var a = 0; a < 10; a++)
            {
                actionBar.Add(new MMOInventoryItem());
            }

            for (var a = 0; a < 40; a++)
            {
                inventory.Add(new MMOInventoryItem());
            }
        }

        // on client
        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        void OnActionBarUpdated(SyncListItem.Operation op, int index, MMOInventoryItem oldItem, MMOInventoryItem newItem)
        {
        }

        void OnInventoryUpdated(SyncListItem.Operation op, int index, MMOInventoryItem oldItem, MMOInventoryItem newItem)
        {
            switch (op)
            {
                case SyncListItem.Operation.OP_ADD:
                    // index is where it got added in the list
                    // item is the new item
                    break;
                case SyncListItem.Operation.OP_CLEAR:
                    // list got cleared
                    break;
                case SyncListItem.Operation.OP_INSERT:
                    // index is where it got added in the list
                    // item is the new item
                    break;
                case SyncListItem.Operation.OP_REMOVEAT:
                    // index is where it got removed in the list
                    // item is the item that was removed
                    break;
                case SyncListItem.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // item is the previous item
                    break;
            }
        }

        public bool addItem (MMOInventoryItem item)
        {
            GameItem gameItem = MMOResourceManager.instance.getItemByType(item.type);

            if (isServer)
            {
                bool found = false;
                for (var i = 0; i < inventory.Count; i++)
                {
                    if (!found && item.type == inventory[i].type)
                    {
                        item.amount += inventory[i].amount;
                        return true;
                    }
                }

                if (!found)
                {
                    for (var i = 0; i < inventory.Count; i++)
                    {
                        if (!found && inventory[i].type == MMOItemType.None)
                        {
                            inventory[i] = item;
                            return true;
                        }
                    }

                    if (!found)
                    {
                        inventory.Add(item);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool addActionItem(MMOInventoryItem item)
        {
            GameItem gameItem = MMOResourceManager.instance.getItemByType(item.type);

            if (isServer)
            {
                bool found = false;
                for (var i = 0; i < actionBar.Count; i++)
                {
                    if (!found && item.type == actionBar[i].type)
                    {
                        item.amount += actionBar[i].amount;
                        return true;
                    }
                }

                if (!found)
                {
                    for (var i = 0; i < actionBar.Count; i++)
                    {
                        if (!found && actionBar[i].type == MMOItemType.None)
                        {
                            actionBar[i] = item;
                            return true;
                        }
                    }

                    if (!found)
                    {
                        actionBar.Add(item);
                        return true;
                    }
                }
            }

            return false;
        }

        public void removeItem(MMOItemType itemType, int count)
        {
            MMOInventoryItem item;

            for (var i = 0; i < inventory.Count; i++)
            {
                if (itemType == inventory[i].type &&
                    inventory[i].amount >= count)
                {
                    item = new MMOInventoryItem
                    {
                        type = inventory[i].type,
                        amount = inventory[i].amount - count
                    };

                    if (item.amount > 0)
                        inventory[i] = item;
                    else
                        inventory.RemoveAt(i);
                }
            }

            for (var i = 0; i < actionBar.Count; i++)
            {
                if (itemType == actionBar[i].type &&
                    actionBar[i].amount >= count)
                {
                    item = new MMOInventoryItem
                    {
                        type = actionBar[i].type,
                        amount = actionBar[i].amount - count
                    };

                    if (item.amount > 0)
                        actionBar[i] = item;
                    else
                        actionBar.RemoveAt(i);
                }
            }
        }

        public void changeItem(int idx)
        {
            if (actionBar[idx].type != MMOItemType.None)
            {
                activeItem = idx;

                UIManager.instance.actionBarUI.updateActionBar();
            }
        }

        public bool checkRecipe(Recipies recipe)
        {
            if (!hasItemCount(recipe.material1.itemType, recipe.count1))
                return false;

            if (!hasItemCount(recipe.material2.itemType, recipe.count2))
                return false;

            if (!hasItemCount(recipe.material3.itemType, recipe.count3))
                return false;

            return true;
        }

        public bool hasItemCount (MMOItemType itemType, int count)
        {
            int inventoryCount = 0;

            for (var i = 0; i < inventory.Count; i++)
            {
                if (itemType == inventory[i].type)
                {
                    inventoryCount += inventory[i].amount;
                }
            }

            for (var i = 0; i < actionBar.Count; i++)
            {
                if (itemType == actionBar[i].type)
                {
                    inventoryCount += actionBar[i].amount;
                }
            }

            return inventoryCount >= count;
        }

        public MMOInventoryItem getItem(MMOItemType type)
        {
            for (var i = 0; i < inventory.Count; i++)
            {
                if (type == inventory[i].type)
                {
                    return inventory[i];
                }
            }

            for (var i = 0; i < actionBar.Count; i++)
            {
                if (type == actionBar[i].type)
                {
                    return actionBar[i];
                }
            }

            return new MMOInventoryItem
                {
                    type = MMOItemType.None,
                    amount = -1
                };
        }

        public int getItemCount (MMOItemType type)
        {
            int count = 0;

            for (var i = 0; i < inventory.Count; i++)
            {
                if (type == inventory[i].type)
                {
                    count += inventory[i].amount;
                }
            }

            for (var i = 0; i < actionBar.Count; i++)
            {
                if (type == actionBar[i].type)
                {
                    count += actionBar[i].amount;
                }
            }

            return count;
        }

        public GameItem getActiveItem()
        {
            if (activeItem != -1)
            {
                MMOInventoryItem item = actionBar[activeItem];

                GameItem gameItem = MMOResourceManager.instance.getItemByType(item.type);

                return gameItem;
            }

            return null;
        }
    }
}
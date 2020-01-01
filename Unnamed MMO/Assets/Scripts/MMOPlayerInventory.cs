using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public struct Item
    {
        public string name;
        public MMOResource itemID;
        public int amount;
    }

    public class SyncListItem : SyncList<Item> { }

    public class MMOPlayerInventory : NetworkBehaviour
    {
        public SyncListItem inventory = new SyncListItem();
        public SyncListItem actionBar = new SyncListItem();

        void Start()
        {
            inventory.Callback += OnInventoryUpdated;
            actionBar.Callback += OnActionBarUpdated;
            syncMode = SyncMode.Owner;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        // on client
        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        void OnActionBarUpdated(SyncListItem.Operation op, int index, Item item)
        {
        }

        void OnInventoryUpdated(SyncListItem.Operation op, int index, Item item)
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
                case SyncListItem.Operation.OP_DIRTY:
                    // index is the index of the item that was updated
                    // item is the previous item
                    break;
            }
        }

        public void addItem (Item item)
        {
            if (isServer)
            {
                bool found = false;
                for (var i = 0; i < inventory.Count; i++)
                {
                    if (!found && inventory[i].itemID == item.itemID)
                    {
                        item.amount += inventory[i].amount;
                        inventory[i] = item;
                        found = true;
                    }
                }

                if (!found)
                {
                    inventory.Add(item);
                }
            }
        }
    }
}
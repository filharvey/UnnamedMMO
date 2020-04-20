using Acemobe.MMO.MMOObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.Data.ScriptableObjects
{
    [CreateAssetMenu]
    public class GameItem : ScriptableObject
    {
        public new string name;
        public Sprite icon;
        public GameObject prefab;

        public MMOObjectTypes type;
        public MMOResourceAction actionType;

        public MMOItemType itemType;
        public int maxStack;

        public bool isHarvestable;
        public bool isStorable;
        public bool isPickable;
    }
}
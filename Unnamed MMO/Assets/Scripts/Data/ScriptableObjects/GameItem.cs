using System;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.Data.ScriptableObjects
{
    [Serializable]
    public class DropItens
    {
        public GameItem material;
        public int min;
        public int max;
        public int chance;
    }

    [CreateAssetMenu]
    public class GameItem : ScriptableObject
    {
        public new string name;
        public Sprite icon;
        public GameObject prefab;

        public MMOItemType itemType;

        public MMOObjectTypes type;
        public MMOResourceAction actionType;

        public int maxStack;

        public bool isHarvestable;              //
        public bool isPickable;                 // pickup
        public bool isBuildingBase;
        public bool isWall;
        public bool requiresBase;

        // items from harvesting
        public List<DropItens> dropItems;

        // crop
        public int growthTime;
    }
}
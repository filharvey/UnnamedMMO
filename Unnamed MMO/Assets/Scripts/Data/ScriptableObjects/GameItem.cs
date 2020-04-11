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

        public MMOObjectTypes type;

        public bool isResource;
        public MMOItemType itemType;

        public int maxStack;
    }
}
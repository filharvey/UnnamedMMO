using System;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.Data.ScriptableObjects
{
    [Serializable]
    public class RecipieMaterial
    {
        public GameItem material;
        public int count;
    }

    [CreateAssetMenu]
    public class Recipies : ScriptableObject
    {
        public new string name;
        public string description;
        public Sprite icon;

        public GameItem item;

        public MMOItemType requiredItem;

        public List<RecipieMaterial> materials;
    }
}

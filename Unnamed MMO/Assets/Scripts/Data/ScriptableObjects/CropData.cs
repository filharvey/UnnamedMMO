﻿using Acemobe.MMO.MMOObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.Data.ScriptableObjects
{
    [CreateAssetMenu]
    public class CropData : ScriptableObject
    {
        public new string name;
        public List<GameObject> growthStates;

        public int growthTime;
        public MMOObjectTypes type;
        public MMOItemType itemType;
    }
}
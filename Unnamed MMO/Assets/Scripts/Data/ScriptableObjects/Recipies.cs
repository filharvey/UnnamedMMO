using Acemobe.MMO.MMOObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.Data.ScriptableObjects
{
    [CreateAssetMenu]
    public class Recipies : ScriptableObject
    {
        public new string name;
        public string description;
        public Sprite icon;

        public GameItem item;

        [Header("Material 1")]
        public GameItem material1;
        public int count1;

        [Header("Material 2")]
        public GameItem material2;
        public int count2;

        [Header("Material 3")]
        public GameItem material3;
        public int count3;
    }
}
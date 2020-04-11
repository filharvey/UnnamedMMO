using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.MMOObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MMOResourceManager : MonoBehaviour
    {
        static MMOResourceManager _instance;

        public static MMOResourceManager instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public List<GameItem> gameItems;

        public List<CropData> gameCrops;

        // Start is called before the first frame update
        void Start()
        {
            _instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public GameItem getItem(string name)
        {
            return gameItems.Find((x) => x.name == name);
        }

        public GameItem getItemByType (MMOItemType type)
        {
            return gameItems.Find((x) => x.itemType == type);
        }

        public CropData getCropObject (string name)
        {
            return gameCrops.Find((x) => x.name == name);
        }
    }
}
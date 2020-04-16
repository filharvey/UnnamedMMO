using Acemobe.MMO.MMOObjects;
using UnityEngine;

namespace Acemobe.MMO.MapData
{
    public class TerrainData : MonoBehaviour
    {
        public bool isWalkable = false;
        public bool isWater = false;
        public bool isPublic = true;
        public bool isInUse = false;

        public MMOObject obj;

        private void Awake()
        {
            if (MMOTerrainManager.instance.isServer)
                MMOTerrainManager.instance.registerTerrainData(this);
        }
    }
}

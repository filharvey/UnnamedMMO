using Acemobe.MMO.Objects;
using UnityEngine;

namespace Acemobe.MMO.Data.MapData
{
    public class TerrainData : MonoBehaviour
    {
        public bool isWalkable = false;
        public bool isWater = false;
        public bool isPublic = true;
        public bool isInUse = false;
        public bool canUse = true;

        public MMOObject obj;

        private void Awake()
        {
            if (MMOTerrainManager.instance.isServer)
                MMOTerrainManager.instance.registerTerrainData(this);
        }
    }
}

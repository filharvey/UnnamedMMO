using Acemobe.MMO.Objects;
using UnityEngine;

namespace Acemobe.MMO.Data.MapData
{
    public enum MAP_DIRECTION
    {
        NORTH = 0,
        EAST,
        SOUTH,
        WEST
    }

    public class TerrainData : MonoBehaviour
    {
        public bool isWalkable = false;
        public bool isWater = false;
        public bool isPublic = true;
        public bool isInUse = false;
        public bool canUse = true;

        public MMOObject obj;
        public MMOObject[] walls;

        private void Awake()
        {
            if (MMOTerrainManager.instance.isServer)
                MMOTerrainManager.instance.registerTerrainData(this);

            walls = new MMOObject[4];

        }
    }
}

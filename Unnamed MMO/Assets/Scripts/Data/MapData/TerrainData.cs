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

        public bool hasWall (float dir)
        {
            if (dir < 45 || dir >= 315)
            {
                if (walls[(int)MAP_DIRECTION.NORTH])
                    return true;
            }
            else if (dir < 135)
            {
                if (walls[(int)MAP_DIRECTION.EAST])
                    return true;
            }
            else if (dir == 225)
            {
                if (walls[(int)MAP_DIRECTION.SOUTH])
                    return true;
            }
            else
            {
                if (walls[(int)MAP_DIRECTION.WEST])
                    return true;
            }

            return false;
        }
    }
}

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
        public MMOObject[] walls = new MMOObject[4];

        #region Terrain Manager Finder
        MMOTerrainManager terrainManager;

        MMOTerrainManager getLocalTerrainManager
        {
            get
            {
                if (terrainManager)
                    return terrainManager;

                GameObject[] objs = gameObject.scene.GetRootGameObjects();

                for (var a = 0; a < objs.Length; a++)
                {
                    GameObject obj = objs[a];

                    MMOTerrainManager terrainMgr = obj.GetComponent<MMOTerrainManager>();

                    if (terrainMgr)
                    {
                        terrainManager = terrainMgr;
                        return terrainManager;
                    }
                }

                return terrainManager;
            }
        }
        #endregion

        private void Awake()
        {
            if (getLocalTerrainManager.isServer)
                getLocalTerrainManager.registerTerrainData(this);
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

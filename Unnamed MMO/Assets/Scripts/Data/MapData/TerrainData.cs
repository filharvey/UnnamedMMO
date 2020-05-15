using Acemobe.MMO.Objects;
using SimpleJSON;
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

                terrainManager = findTerrainManager(gameObject);

                return terrainManager;
            }
        }

        MMOTerrainManager findTerrainManager(GameObject obj)
        {
            MMOTerrainManager terrain = obj.GetComponent<MMOTerrainManager>();

            if (terrain)
                return terrain;

            if (obj.transform.parent)
                terrain = findTerrainManager(obj.transform.parent.gameObject);

            return terrain;
        }

        #endregion

        private void Awake()
        {
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

        public JSONClass writeData ()
        {
            JSONClass data = new JSONClass();

            data["flags"] = new JSONClass();
            data["flags"]["walkable"].AsBool = isWalkable;

            // add object
            if (obj)
            {
                data["obj"] = obj.writeData ();


                // add walls
                data["walls"] = new JSONClass();
                for (var a = 0; a < walls.Length; a++)
                {
                    if (walls[a])
                    {
                        data["walls"][a] = walls[a].writeData ();
                    }
                }
            }


            return data;
        }

        public void readData (JSONClass json)
        {

        }

    }
}

using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.Objects;
using Mirror;
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
            // add object
            if (obj)
            {
                int count = 0;
                JSONClass data = new JSONClass();

                data["pos"] = new JSONClass();
                data["pos"]["x"].AsInt = Mathf.FloorToInt(transform.localPosition.x);
                data["pos"]["z"].AsInt = Mathf.FloorToInt(transform.localPosition.z);

                if (obj.manager == null)
                {
                    data["obj"] = obj.writeData();
                    count++;
                }

                // add walls
                for (var a = 0; a < walls.Length; a++)
                {
                    if (walls[a])
                    {
                        if (data["walls"].AsObject == null)
                            data["walls"] = new JSONClass();

                        data["walls"][a] = walls[a].writeData ();
                        count++;
                    }
                }

                if (count > 0)
                    return data;
            }

            return null;
        }

        public void readData(JSONClass json)
        {
            JSONClass objJson = json["obj"].AsObject;
            JSONClass wallJson = json["walls"].AsObject;

            if (objJson != null)
            {
                int x = objJson["pos"]["x"].AsInt;
                int z = objJson["pos"]["z"].AsInt;
                float angle = objJson["angle"].AsFloat;
                int gI = objJson["gameItem"].AsInt;

                Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                Quaternion rotation = new Quaternion();
                rotation.eulerAngles = new Vector3(0, angle, 0);

                GameItem gameItem = MMOResourceManager.instance.getItemByType((MMOItemType)gI);

                GameObject newObj = Instantiate(gameItem.prefab, pos, rotation);
                MMOObject mmoObj = newObj.GetComponent<MMOObject>();

                mmoObj.readData(objJson);

                getLocalTerrainManager.addObjectAt((int)pos.x, (int)pos.z, mmoObj);

                NetworkServer.Spawn(newObj);
            }

            if (wallJson != null)
            {
                for (var a = 0; a < walls.Length; a++)
                {
                    JSONClass wall = wallJson[a].AsObject;
                    if (wall != null)
                    {
                        int x = wall["pos"]["x"].AsInt;
                        int z = wall["pos"]["z"].AsInt;
                        float angle = wall["angle"].AsFloat;
                        int gI = wall["gameItem"].AsInt;

                        Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                        Quaternion rotation = new Quaternion();
                        rotation.eulerAngles = new Vector3(0, angle, 0);

                        GameItem gameItem = MMOResourceManager.instance.getItemByType((MMOItemType)gI);

                        GameObject newObj = Instantiate(gameItem.prefab, pos, rotation);
                        MMOObject mmoObj = newObj.GetComponent<MMOObject>();

                        mmoObj.readData(wall);

                        getLocalTerrainManager.addObjectAt((int)pos.x, (int)pos.z, mmoObj);

                        NetworkServer.Spawn(newObj);
                    }
                }
            }
        }
    }
}

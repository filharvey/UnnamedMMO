using System;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using Acemobe.MMO.Objects;
using Acemobe.MMO.Data.MapData;

namespace Acemobe.MMO
{
    public class MMOTerrainManager : NetworkBehaviour
    {
        static MMOTerrainManager _instance;

        public static MMOTerrainManager instance
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

        [Header("Components")]
        public GameObject terrainBase;  // holds all the 
        public Dictionary<string, Data.MapData.TerrainData> terrain = new Dictionary<string, Data.MapData.TerrainData>();
        public Data.MapData.TerrainData[,] mapData;
        public GameObject startMap;

        public MMOMap islandMap;

        // map loaded in
        Bounds bounds;
        int mapWidth;
        int mapDepth;

        private void Awake()
        {
            _instance = this;
        }

        public void registerTerrainData (Data.MapData.TerrainData terrainData)
        {
            string name = terrainData.transform.localPosition.x + ":" + terrainData.transform.localPosition.y + ":" + terrainData.transform.localPosition.z;

            if (terrain.ContainsKey(name))
            {
                Debug.Log("Duplicate location: " + terrainData.gameObject.name + " - " + name);
                terrainData.transform.gameObject.SetActive(false);
            }
            else
                terrain.Add(name, terrainData);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            startMap = Resources.Load<GameObject>("Maps/Map1");

            GameObject obj = Instantiate(startMap);
            obj.transform.SetParent(terrainBase.transform);
            islandMap = obj.GetComponent<MMOMap>();

            //obj.SetActive(true);

            this.createTerrain();
            /*
                        // add the quest giver
                        int x = 0;
                        int z = 8;
                        GameItem npc = MMOResourceManager.instance.getItem("NPC");

                        Vector3 pos = new Vector3(x + 0.5f, 0f, z + 0.5f);
                        Quaternion rotation = new Quaternion();
                        rotation.eulerAngles = new Vector3(0, 90, 0);

                        obj = Instantiate(npc.prefab, pos, rotation);
                        MMOObject spawnObj = obj.GetComponent<MMOObject>();

                        addObjectAt(x, z, spawnObj);
                        NetworkServer.Spawn(obj);
            */
        }

        // on client
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isClientOnly)
            {
                startMap = Resources.Load<GameObject>("Maps/Map1");

                GameObject obj = Instantiate(startMap);
                obj.transform.SetParent(terrainBase.transform);
            }
        }

        public void createTerrain ()
        {
            bounds = new Bounds();

            foreach (var k in terrain)
            {
                Data.MapData.TerrainData terrain = k.Value;

                bounds.Encapsulate(terrain.transform.localPosition);
            }

            mapWidth = (int)(bounds.max.x - bounds.min.x) + 1;
            mapDepth = (int)(bounds.max.z - bounds.min.z) + 1;

            Debug.Log("Size: " + mapWidth + ", " + mapDepth);

            mapData = new Data.MapData.TerrainData[mapWidth, mapDepth];

            foreach (var k in terrain)
            {
                Data.MapData.TerrainData terrain = k.Value;
                int x = (int)(terrain.transform.localPosition.x - bounds.min.x);
                int z = (int)(terrain.transform.localPosition.z - bounds.min.z);

                if (mapData[x,z] == null)
                {
                    mapData[x, z] = terrain;
                }
                else
                {
                    if (terrain.transform.localPosition.y > mapData[x,z].transform.localPosition.y)
                    {
                        mapData[x, z] = terrain;
                    }
                }
            }

            for (int x = 0; x < mapWidth; x++)
            {
                for (int z = 0; z < mapDepth; z++)
                {
                    if (mapData[x, z] != null)
                    {
                    }
                }
            }
        }

        internal void addObj(float x, float z, MMOObject mmoObj)
        {
            throw new NotImplementedException();
        }

        public bool checkPosition(int x, int z, int width, int height)
        {
            for (int w = 0; w < width; w++)
            {
                for (int d = 0; d < height; d++)
                {
                    var posX = (int)((x + w - width / 2) - bounds.min.x);
                    var posZ = (int)((z + d - height / 2) - bounds.min.z);

                    if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth)
                    {
                        if (!mapData[posX, posZ])
                        {
                            return false;
                        }

                        if (mapData[posX, posZ] && 
                            (mapData[posX, posZ].obj || mapData[posX, posZ].isInUse || !mapData[posX, posZ].canUse))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Data.MapData.TerrainData getTerrainData (int x, int z)
        {
            var posX = (int)(x - bounds.min.x);
            var posZ = (int)(z - bounds.min.z);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
                return mapData[posX, posZ];

            return null;
        }

        public MMOObject getObjectAt (Vector3 pos)
        {
            return getObjectAt ((int)Mathf.Floor (pos.x), (int)Mathf.Floor (pos.z));
        }

        public MMOObject getObjectAt (int x, int z)
        {
            var posX = (int)(x - bounds.min.x);
            var posZ = (int)(z - bounds.min.z);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                return mapData[posX, posZ].obj;
            }

            return null;
        }

        public void addObjectAt (int x, int z, MMOObject obj)
        {
            var posX = (int)(x - bounds.min.x);
            var posZ = (int)(z - bounds.min.z);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                mapData[posX, posZ].obj = obj;
            }
        }

        public void removeObjectAt (int x, int z)
        {
            var posX = (int)(x - bounds.min.x);
            var posZ = (int)(z - bounds.min.z);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                mapData[posX, posZ].obj = null;
            }
        }

        public MMOObject getWallAt(int x, int z, MAP_DIRECTION dir)
        {
            var posX = (int)(x - bounds.min.x);
            var posZ = (int)(z - bounds.min.z);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                if (mapData[posX, posZ].obj &&
                    mapData[posX, posZ].obj.gameItem.itemType == Data.MMOItemType.Building_Base)
                {

                }
            }

            return null;
        }

        public void addWallAt(int x, int z, MAP_DIRECTION dir, MMOObject obj)
        {
            var posX = (int)(x - bounds.min.x);
            var posZ = (int)(z - bounds.min.z);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                if (mapData[posX, posZ].obj &&
                    mapData[posX, posZ].obj.gameItem.itemType == Data.MMOItemType.Building_Base)
                {

                }
            }
        }

        public void addWallAt(int x, int z, int dir, MMOObject obj)
        {
            if (dir < 45 || dir >= 315)
                addWallAt(x, z, MAP_DIRECTION.NORTH, obj);
            else if (dir < 135)
                addWallAt(x, z, MAP_DIRECTION.EAST, obj);
            else if (dir == 225)
                addWallAt(x, z, MAP_DIRECTION.SOUTH, obj);
            else
                addWallAt(x, z, MAP_DIRECTION.WEST, obj);
        }

        public void removeWallAt(int x, int z, MAP_DIRECTION dir)
        {
            var posX = (int)(x - bounds.min.x);
            var posZ = (int)(z - bounds.min.z);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                if (mapData[posX, posZ].obj &&
                    mapData[posX, posZ].obj.gameItem.itemType == Data.MMOItemType.Building_Base)
                {

                }
            }
        }
    }
}
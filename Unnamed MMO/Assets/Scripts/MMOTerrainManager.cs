using System;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using Acemobe.MMO.Objects;
using Acemobe.MMO.Data.MapData;
using Acemobe.MMO.Data.ScriptableObjects;
using SimpleJSON;
using BestHTTP;

namespace Acemobe.MMO
{
    public class MMOTerrainManager : MonoBehaviour
    {
        public static Dictionary<string, MMOTerrainManager> terrains = new Dictionary<string, MMOTerrainManager>();

        [Header("Components")]
        public GameObject terrainBase;  // holds all the 
        public Dictionary<string, Data.MapData.TerrainData> terrain = new Dictionary<string, Data.MapData.TerrainData>();
        public Data.MapData.TerrainData[,] mapData;

        public GameObject map;
        public GameObject startMap;
        public MMOMap islandMap;

        // map loaded in
        public Bounds bounds = new Bounds();
        public Bounds worldBounds = new Bounds();
        public int mapWidth = 0;
        public int mapDepth = 0;
        public bool isLoaded = false;

        public Dictionary<string, MMOSpawnManager> spawnManagers = new Dictionary<string, MMOSpawnManager>();

        public void Start()
        {
            if (!startMap)
            {
                startMap = Resources.Load<GameObject>("Maps/Map1");

                map = Instantiate(startMap);
                map.transform.SetParent(terrainBase.transform);
            }
        }

        // added but only used on server
        public void registerTerrainData (Data.MapData.TerrainData terrainData)
        {
            string name = terrainData.transform.localPosition.x + ":" + terrainData.transform.localPosition.y + ":" + terrainData.transform.localPosition.z;

            if (terrain.ContainsKey(name))
            {
                Debug.Log("Duplicate location: " + terrainData.gameObject.name + " - " + name);
                terrainData.transform.gameObject.SetActive(false);
            }
            else
            {
                if (terrain.Count == 0)
                {
                    bounds = new Bounds(terrainData.transform.localPosition, Vector3.one);
                    worldBounds = new Bounds(terrainData.transform.position, Vector3.one);
                }
                else
                {
                    var pos = new Vector3(
                        Mathf.FloorToInt(terrainData.transform.localPosition.x), 
                        0,
                        Mathf.FloorToInt(terrainData.transform.localPosition.z));

                    bounds.Encapsulate(pos);
                    worldBounds.Encapsulate(pos);
                }

                terrain.Add(name, terrainData);
                
                mapWidth = (int)(bounds.max.x - bounds.min.x) + 1;
                mapDepth = (int)(bounds.max.z - bounds.min.z) + 1;
            }
        }

        public void createTerrain ()
        {
            if (!islandMap)
            {
                islandMap = map.GetComponent<MMOMap>();
            }

            islandMap.navMeshSurface.RemoveData();
            islandMap.navMeshSurface.BuildNavMesh();

            worldBounds.Encapsulate(worldBounds.center + new Vector3(0, -10, 0));
            worldBounds.Encapsulate(worldBounds.center + new Vector3(0, 10, 0));

            name = bounds.ToString();
            terrains.Add(name, this);

            mapData = new Data.MapData.TerrainData[mapWidth, mapDepth];

            foreach (var k in terrain)
            {
                Data.MapData.TerrainData terrain = k.Value;
                terrain.name = (int)terrain.transform.localPosition.x + ":" + (int)terrain.transform.localPosition.z;

                int x = (int)(Mathf.FloorToInt(terrain.transform.localPosition.x) - bounds.min.x);
                int z = (int)(Mathf.FloorToInt(terrain.transform.localPosition.z) - bounds.min.z);

                if (mapData[x, z] == null)
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

            islandMap.originX = Mathf.FloorToInt(transform.position.x);
            islandMap.originZ = Mathf.FloorToInt(transform.position.z);
        }

        internal void addObj(float x, float z, MMOObject mmoObj)
        {
            throw new NotImplementedException();
        }

        public bool checkPosition(int x, int z, int width, int height)
        {
            x -= islandMap.originX;
            z -= islandMap.originZ;

            int w2 = width / 2;
            int h2 = height / 2;

            for (int w = -w2; w < width; w++)
            {
                for (int d = -h2; d < height; d++)
                {
                    var posX = Mathf.FloorToInt((x + w) - bounds.min.x);
                    var posZ = Mathf.FloorToInt((z + d) - bounds.min.z);

                    if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth)
                    {
                        Data.MapData.TerrainData data = mapData[posX, posZ];

                        if (!data)
                        {
                            return false;
                        }
                        else if (data.obj || data.isInUse || !data.canUse)
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
            var posX = (int)(Mathf.FloorToInt(x) - bounds.min.x - islandMap.originX);
            var posZ = (int)(Mathf.FloorToInt(z) - bounds.min.z - islandMap.originZ);

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
            var posX = (int)(Mathf.FloorToInt(x) - bounds.min.x - islandMap.originX);
            var posZ = (int)(Mathf.FloorToInt(z) - bounds.min.z - islandMap.originZ);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                return mapData[posX, posZ].obj;
            }

            return null;
        }

        public void addObjectAt (int x, int z, MMOObject obj)
        {
            var posX = (int)(Mathf.FloorToInt(x) - bounds.min.x - islandMap.originX);
            var posZ = (int)(Mathf.FloorToInt(z) - bounds.min.z - islandMap.originZ);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                mapData[posX, posZ].obj = obj;
            }
        }

        public void removeObjectAt (int x, int z)
        {
            var posX = (int)(Mathf.FloorToInt(x) - bounds.min.x - islandMap.originX);
            var posZ = (int)(Mathf.FloorToInt(z) - bounds.min.z - islandMap.originZ);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                mapData[posX, posZ].obj = null;
            }
        }

        public MMOObject getWallAt(int x, int z, MAP_DIRECTION dir)
        {
            var posX = (int)(Mathf.FloorToInt(x) - bounds.min.x - islandMap.originX);
            var posZ = (int)(Mathf.FloorToInt(z) - bounds.min.z - islandMap.originZ);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                if (mapData[posX, posZ].obj &&
                    mapData[posX, posZ].obj.gameItem.itemType == Data.MMOItemType.Building_Base)
                {
                    return mapData[posX, posZ].walls[(int)dir];
                }
            }

            return null;
        }

        public void addWallAt(int x, int z, MAP_DIRECTION dir, MMOObject obj)
        {
            var posX = (int)(Mathf.FloorToInt(x) - bounds.min.x - islandMap.originX);
            var posZ = (int)(Mathf.FloorToInt(z) - bounds.min.z - islandMap.originZ);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                if (mapData[posX, posZ].obj &&
                    mapData[posX, posZ].obj.gameItem.itemType == Data.MMOItemType.Building_Base)
                {
                    mapData[posX, posZ].walls[(int)dir] = obj;
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
            var posX = (int)(Mathf.FloorToInt(x) - bounds.min.x - islandMap.originX);
            var posZ = (int)(Mathf.FloorToInt(z) - bounds.min.z - islandMap.originZ);

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                if (mapData[posX, posZ].obj &&
                    mapData[posX, posZ].obj.gameItem.itemType == Data.MMOItemType.Building_Base)
                {
                    mapData[posX, posZ].walls[(int)dir] = null;
                }
            }
        }

        public JSONClass writeData ()
        {
            JSONClass data = new JSONClass();
            data["terrains"] = new JSONArray();
            data["spawners"] = new JSONArray();

            foreach (var s in spawnManagers)
            {
                MMOSpawnManager sMgr = s.Value;
                JSONClass spawner = sMgr.writeData();

                data["spawners"].Add(spawner);
            }

            foreach (var t in terrain)
            {
                Data.MapData.TerrainData terrainData = t.Value;
                JSONClass terrain = terrainData.writeData();

                if (terrain != null)
                {
                    data["terrains"].Add(terrain);
                }
            }

            return data;
        }

        public void readData(JSONClass json)
        {
            JSONArray terrain = json["terrains"].AsArray;
            JSONArray spawners = json["spawners"].AsArray;

            for (int s = 0; s < spawners.Count; s++)
            {
                JSONClass spawnJson = spawners[s].AsObject;
                string name = spawnJson["name"];

                spawnManagers[name].readData(spawnJson);
            }

            for (int s = 0; s < terrain.Count; s++)
            {
                JSONClass terrainJson = terrain[s].AsObject;
                int x = terrainJson["pos"]["x"].AsInt;
                int z = terrainJson["pos"]["z"].AsInt;

                Data.MapData.TerrainData terrainData = getTerrainData(x, z);

                terrainData.readData(terrainJson);
            }
        }
    }
}
using System;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using Acemobe.MMO.Objects;
using Acemobe.MMO.Data.MapData;
using Acemobe.MMO.Data.ScriptableObjects;
using SimpleJSON;

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

        public void Start()
        {
            if (!startMap)
            {
                startMap = Resources.Load<GameObject>("Maps/Map1");

                map = Instantiate(startMap);
                map.transform.SetParent(terrainBase.transform);
            }

            if (!islandMap)
            {
                islandMap = map.GetComponent<MMOMap>();
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
                    bounds.Encapsulate(terrainData.transform.localPosition);
                    worldBounds.Encapsulate(terrainData.transform.position);
                }

                terrain.Add(name, terrainData);
                
                mapWidth = (int)(bounds.max.x - bounds.min.x) + 1;
                mapDepth = (int)(bounds.max.z - bounds.min.z) + 1;
            }
        }

        public void createTerrain ()
        {
            islandMap.navMeshSurface.RemoveData();
            islandMap.navMeshSurface.BuildNavMesh();

            worldBounds.Encapsulate(worldBounds.center + new Vector3(0, -10, 0));
            worldBounds.Encapsulate(worldBounds.center + new Vector3(0, 10, 0));

            name = bounds.ToString();
            terrains.Add(name, this);

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
            var posX = (int)(x - bounds.min.x) - islandMap.originX;
            var posZ = (int)(z - bounds.min.z) - islandMap.originZ;

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
            var posX = (int)(x - bounds.min.x) - islandMap.originX;
            var posZ = (int)(z - bounds.min.z) - islandMap.originZ;

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                return mapData[posX, posZ].obj;
            }

            return null;
        }

        public void addObjectAt (int x, int z, MMOObject obj)
        {
            var posX = (int)(x - bounds.min.x) - islandMap.originX;
            var posZ = (int)(z - bounds.min.z) - islandMap.originZ;

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                mapData[posX, posZ].obj = obj;
            }
        }

        public void removeObjectAt (int x, int z)
        {
            var posX = (int)(x - bounds.min.x) - islandMap.originX;
            var posZ = (int)(z - bounds.min.z) - islandMap.originZ;

            if (posX >= 0 && posZ >= 0 && posX < mapWidth && posZ < mapDepth && mapData[posX, posZ])
            {
                mapData[posX, posZ].obj = null;
            }
        }

        public MMOObject getWallAt(int x, int z, MAP_DIRECTION dir)
        {
            var posX = (int)(x - bounds.min.x) - islandMap.originX;
            var posZ = (int)(z - bounds.min.z) - islandMap.originZ;

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
            var posX = (int)(x - bounds.min.x) - islandMap.originX;
            var posZ = (int)(z - bounds.min.z) - islandMap.originZ;

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
            var posX = (int)(x - bounds.min.x) - islandMap.originX;
            var posZ = (int)(z - bounds.min.z) - islandMap.originZ;

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

            foreach (var t in terrain)
            {
                Data.MapData.TerrainData terrainData = t.Value;
                data.Add(terrainData.writeData());
            }

            return data;
        }
    }
}
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using Acemobe.MMO.MapData;
using Acemobe.MMO.MMOObjects;

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
        public Dictionary<string, MapData.TerrainData> terrain = new Dictionary<string, MapData.TerrainData>();
        public MapData.TerrainData[,] mapData;

        // map loaded in
        GameObject map;
        Bounds bounds;
        int mapWidth;
        int mapDepth;

        private void Awake()
        {
            _instance = this;
        }

        public void registerTerrainData (MapData.TerrainData terrainData)
        {
            string name = terrainData.transform.localPosition.x + ":" + terrainData.transform.localPosition.y + ":" + terrainData.transform.localPosition.z;

            if (terrain.ContainsKey(name))
            {
                Debug.Log(name);
                terrainData.transform.parent.gameObject.SetActive(false);
            }
            else
                terrain.Add(name, terrainData);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            map = Resources.Load<GameObject>("Maps/Map1");

            GameObject obj = Instantiate(map);
            obj.transform.SetParent(terrainBase.transform);

            this.createTerrain();
        }

        // on client
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isClientOnly)
            {
                map = Resources.Load<GameObject>("Maps/Map1");

                GameObject obj = Instantiate(map);
                obj.transform.SetParent(terrainBase.transform);
            }
        }

        public void createTerrain ()
        {
            bounds = new Bounds();

            foreach (var k in terrain)
            {
                MapData.TerrainData terrain = k.Value;

                bounds.Encapsulate(terrain.transform.localPosition);
            }

            mapWidth = (int)(bounds.max.x - bounds.min.x) + 1;
            mapDepth = (int)(bounds.max.z - bounds.min.z) + 1;

            mapData = new MapData.TerrainData[mapWidth, mapDepth];

            foreach (var k in terrain)
            {
                MapData.TerrainData terrain = k.Value;
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


            /*            TextAsset jsonData = Resources.Load<TextAsset>("Maps/Map1");

                        if (jsonData)
                        {
                            JSONNode json = JSON.Parse(jsonData.text);

                            JSONNode mapData = json["data"].AsArray;

                            if (mapData != null)
                            {
                                int height = mapData.Count;
                                int width = mapData[0].AsArray.Count;

                                offX = -width / 2;
                                offZ = -height / 2;

                                height = (int)Mathf.Ceil(height / chunkWidth);
                                width = (int)Mathf.Ceil(width / chunkWidth);

                                for (int x = 0; x <= width; x++)
                                {
                                    for (int z = 0; z <= height; z++)
                                    {
                                        int xPos = (int)(x * chunkWidth + offX) + chunkWidth / 2;
                                        int zPos = (int)(z * chunkHeight + offZ) + chunkHeight / 2;

                                        // create Map Chunk 
                                        Vector3 pos = new Vector3(xPos, 0, zPos);
                                        Quaternion rotation = new Quaternion();

                                        GameObject gameobject = Instantiate(mapChunkPrefab, pos, rotation);
                                        MapChunk chunk = gameobject.GetComponent<MapChunk>();
                                        chunk.init(xPos, zPos, chunkWidth, chunkHeight);
                                        gameobject.name = chunk.chunkName = xPos + ":" + zPos;

                                        NetworkServer.Spawn(gameobject);
                                        gameobject.transform.SetParent(MMOTerrainManager.instance.terrainBase.transform);

                                        chunk.create(mapData, (int)(x * chunkWidth), (int)(z * chunkHeight));
                                        mapChunks.Add(chunk.chunkName, chunk);
                                    }
                                }
                            }
                        }
            */
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
                        if (mapData[posX, posZ] && 
                            (mapData[posX, posZ].obj || mapData[posX, posZ].isInUse))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public MapData.TerrainData getTerrainData (int x, int z)
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
    }
}
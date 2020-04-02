using Mirror;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MMOTerrainManager : NetworkBehaviour
    {
        static MMOTerrainManager _instance;

        [Header("Tiles")]
        public GameObject grass;
        public GameObject water;
        public GameObject grassWaterCorner;

        [Header("MapChunk")]
        public GameObject mapChunkPrefab;
        public int chunkWidth;
        public int chunkHeight;

        [Header("Components")]
        public GameObject terrainBase;  // holds all the 

        public Dictionary<string, MapChunk> mapChunks = new Dictionary<string, MapChunk>();

        float offX = 0;
        float offZ = 0;

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

        public override void OnStartServer()
        {
            _instance = this;

            base.OnStartServer();

            this.createTerrain();
        }

        // on client
        public override void OnStartClient()
        {
            _instance = this;

            base.OnStartClient();
        }

        public void createTerrain ()
        {
            TextAsset jsonData = Resources.Load<TextAsset>("Maps/Map1");

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
        }

        internal void addObj(float x, float z, MMOObject mmoObj)
        {
            throw new NotImplementedException();
        }

        public MapChunk getChunk(float x, float z)
        {
            float x1 = (x - offX);
            float z1 = (z - offZ);

            int cellX = (int)Math.Floor(x1 / chunkWidth);
            int cellZ = (int)Math.Floor(z1 / chunkHeight);

            int xPos = (int)(cellX * chunkWidth + offX) + chunkWidth / 2;
            int zPos = (int)(cellZ * chunkHeight + offZ) + chunkHeight / 2;

            var name = xPos + ":" + zPos;

            if (mapChunks.ContainsKey (name))
                return mapChunks[name];

            return null;
        }

        public bool checkPosition(int x, int z, int width, int height)
        {
            for (int w = 0; w < width; w++)
            {
                for (int d = 0; d < height; d++)
                {
                    var posX = x + w - width / 2;
                    var posZ = z + d - height / 2;

                    MapChunk chunk = getChunk(posX, posZ);

                    if (chunk)
                    {
                        // adjust depending on position on chunk
                        posX -= chunk.bound.min.x;
                        posZ -= chunk.bound.min.z;

                        return chunk.checkPosition(posX, posZ);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public MMOObject getObject(Vector3 pos)
        {
            return getObject ((int)Mathf.Floor (pos.x), (int)Mathf.Floor (pos.z));
        }

        public MMOObject getObject(int x, int z)
        {
            MapChunk chunk = getChunk(x, z);

            if (chunk)
            {
                // adjust depending on position on chunk
                x -= chunk.bound.min.x;
                z -= chunk.bound.min.z;

                return chunk.getObject(x, z);
            }
            return null;
        }

        public void addObject (int x, int z, MMOObject obj)
        {
            MapChunk chunk = getChunk(x, z);

            if (chunk)
            {
                // adjust depending on position on chunk
                x -= chunk.bound.min.x;
                z -= chunk.bound.min.z;

                chunk.mapCells[x, z].setObj(obj);
            }
        }

        public void removeObject (int x, int z)
        {
            MapChunk chunk = getChunk(x, z);

            if (chunk)
            {
                // adjust depending on position on chunk
                x -= chunk.bound.min.x;
                z -= chunk.bound.min.z;

                chunk.mapCells[x, z].setObj(null);
            }
        }
    }
}
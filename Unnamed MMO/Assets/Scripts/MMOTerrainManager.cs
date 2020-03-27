using Mirror;
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
        public List<GameObject> grass;

        public List<GameObject> dirt;

        public List<GameObject> water;

        [Header("MapChunk")]
        public GameObject mapChunkPrefab;
        public int chunkWidth;
        public int chunkHeight;

        [Header("Components")]
        public GameObject terrainBase;  // holds all the 

        public Dictionary<string, MapChunk> mapChunks = new Dictionary<string, MapChunk>();

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
            /*int size = 2;

                        for (int x = -size; x <= size; x++)
                        {
                            for (int z = -size; z <= size; z++)
                            {
                                int xPos = x * chunkWidth;
                                int zPos = z * chunkHeight;

                                // create Map Chunk 
                                Vector3 pos = new Vector3(xPos, 0, zPos);
                                Quaternion rotation = new Quaternion();

                                GameObject gameobject = Instantiate(mapChunkPrefab, pos, rotation);
                                MapChunk chunk = gameobject.GetComponent<MapChunk>();
                                chunk.init(xPos, zPos, chunkWidth, chunkHeight);
                                chunk.name = x + ":" + z;

                                NetworkServer.Spawn(gameobject);

                                chunk.create();
                                mapChunks.Add (chunk.name, chunk);
                            }
                        }
            */
        }

        internal void addObj(float x, float z, MMOObject mmoObj)
        {
            throw new NotImplementedException();
        }

        public MapChunk getChunk(float x, float z)
        {
            float x1 = (x + chunkWidth / 2);
            float z1 = (z + chunkHeight / 2);

            int cellX = (int)Math.Floor(x1 / chunkWidth);
            int cellZ = (int)Math.Floor(z1 / chunkHeight);

            var name = cellX + ":" + cellZ;

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
                        posX -= chunk.pos.x;
                        posZ -= chunk.pos.y;

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
                x -= chunk.pos.x;
                z -= chunk.pos.y;

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
                x -= chunk.pos.x;
                z -= chunk.pos.y;

                chunk.mapCells[x, z].setObj(obj);
            }
        }

        public void removeObject (int x, int z)
        {
            MapChunk chunk = getChunk(x, z);

            if (chunk)
            {
                // adjust depending on position on chunk
                x -= chunk.pos.x;
                z -= chunk.pos.y;

                chunk.mapCells[x, z].setObj(null);
            }
        }
    }
}
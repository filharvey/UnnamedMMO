using Mirror;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class SyncListMapInfo: SyncList<MapInfo> { }

    public class MapChunk : NetworkBehaviour
    {
        public SyncListMapInfo chunk = new SyncListMapInfo();

        [SyncVar]
        public string chunkName;

        [SyncVar]
        public Vector2Int pos;

        [SyncVar]
        public int width;

        [SyncVar]
        public int height;

        public BoundsInt bound;

        public MapCell[,] mapCells;

        void Start()
        {
        }

        public override void OnStartServer()
        {
            chunk.Callback += OnChunkUpdated;
            base.OnStartServer();
        }

        // on client
        public override void OnStartClient()
        {
            chunk.Callback += OnChunkUpdated;

            base.OnStartClient();

            if (isClientOnly)
            {
                MMOTerrainManager.instance.mapChunks.Add(this.name, this);

                mapCells = new MapCell[width, height];

                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        var idx = x + (z * width);

                        if (idx < chunk.Count)
                        {
                            mapCells[x, z] = new MapCell(this, chunk[idx].pos.x, chunk[idx].pos.y, chunk[idx].type);
                            mapCells[x, z].createTerrain();
                        }
                    }
                }

                bound = new BoundsInt();
                bound.min = new Vector3Int(pos.x - width / 2, 0, pos.y - height / 2);
                bound.max = new Vector3Int(pos.x + width / 2, 0, pos.y + height / 2);
            }
        }

        public override void OnNetworkDestroy ()
        {
            base.OnNetworkDestroy();

            // kill all map cells
            for (int x1 = 0; x1 < width; x1++)
            {
                for (int z1 = 0; z1 < height; z1++)
                {
                    if (mapCells[x1, z1].terrain)
                    {
                        Destroy(mapCells[x1, z1].terrain);
                    }
                }
            }
        }

        public void init(float x, float z, int width, int height)
        {
            this.width = width;
            this.height = height;

            pos = new Vector2Int((int)x, (int)z);

            mapCells = new MapCell[width, height];

            bound = new BoundsInt();
            bound.min = new Vector3Int(pos.x - width / 2, 0, pos.y - height / 2);
            bound.max = new Vector3Int(pos.x + width / 2, 0, pos.y + height / 2);
        }

        public void create(JSONNode mapData, int offX, int offZ)
        {
            // create 
            for (int z1 = 0; z1 < height; z1++)
            {
                var data = mapData[z1 + offZ].AsArray;

                for (int x1 = 0; x1 < width; x1++)
                {
                    int type = data[x1 + offX].AsInt;
                    int x = (pos.x + x1 - width / 2);
                    int z = (pos.y + z1 - height / 2);

                    mapCells[x1, z1] = new MapCell(this, x1, z1, type);
                    mapCells[x1, z1].name = x + ":" + z;
                    mapCells[x1, z1].createTerrain();

                    chunk.Add(mapCells[x1, z1].info);
                }
            }
        }

        public GameObject CreatePrefab (GameObject prefab, Vector3 pos, Quaternion rotation)
        {
            GameObject obj = Instantiate(prefab, pos, rotation);

            obj.transform.SetParent(transform);
            return obj;
        }

        public void DestroyPrefab(GameObject obj)
        {
            if (obj != null)
                Destroy(obj);
        }

        void OnChunkUpdated(SyncListMapInfo.Operation op, int index, MapInfo oldItem, MapInfo newItem)
        {
            int x = newItem.pos.x;
            int z = newItem.pos.y;

            switch (op)
            {
                case SyncListMapInfo.Operation.OP_ADD:
                    // index is where it got added in the list
                    // item is the new item
                    if (mapCells[x, z] == null)
                        mapCells[x, z] = new MapCell(this, newItem.pos.x, newItem.pos.y, newItem.type);
                    else
                        mapCells[x, z].setTerrain(newItem.type);

                    mapCells[x, z].createTerrain();
                    break;
                case SyncListMapInfo.Operation.OP_CLEAR:
                    // list got cleared
                    mapCells[x, z].createTerrain();
                    mapCells[x, z].terrain = null;

                    if (isClient)
                    {
                    }
                    break;
                case SyncListMapInfo.Operation.OP_INSERT:
                    // index is where it got added in the list
                    // item is the new item

                    // should not happen
                    if (isClient)
                    {

                    }
                    break;
                case SyncListMapInfo.Operation.OP_REMOVEAT:
                    // index is where it got removed in the list
                    // item is the item that was removed

                    // should not happen
                    if (isClient)
                    {

                    }
                    break;
                case SyncListMapInfo.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // item is the previous item
                    mapCells[x, z].setTerrain(newItem.type);
                    mapCells[x, z].createTerrain();
                    break;
            }
        }

        public bool checkPosition(int x, int z)
        {
            if (mapCells[x, z].type != 1)
                return false;

            if (mapCells[x, z].obj != null)
                return false;

            return true;
        }

        public MMOObject getObject(int x, int z)
        {
            return mapCells[x, z].obj;
        }
    }
}
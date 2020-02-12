using Mirror;
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
        public string name;

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
                            mapCells[x, z] = new MapCell(this, chunk[idx].pos.x, chunk[idx].pos.y);
                            mapCells[x, z].setTerrain (chunk[idx].type);
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

            float xStart = x - width / 2;
            float zStart = z - height / 2;

            this.pos = new Vector2Int((int)xStart, (int)zStart);

            mapCells = new MapCell[width, height];

            bound = new BoundsInt();
            bound.min = new Vector3Int(pos.x - width / 2, 0, pos.y - height / 2);
            bound.max = new Vector3Int(pos.x + width / 2, 0, pos.y + height / 2);
        }

        public void create()
        {
            // create 
            for (int x1 = 0; x1 < width; x1++)
            {
                for (int z1 = 0; z1 < height; z1++)
                {
                    mapCells[x1, z1] = new MapCell(this, (int)(this.pos.x + x1), (int)(this.pos.y + z1));
                    mapCells[x1, z1].setTerrain(0);

                    chunk.Add(mapCells[x1, z1].info);
                }
            }
        }

        public GameObject CreatePrefab (GameObject prefab, Vector3 pos, Quaternion rotation)
        {
            GameObject obj = Instantiate(prefab, pos, rotation);

            obj.transform.SetParent(MMOTerrainManager.instance.terrainBase.transform);

            return obj;
        }

        void OnChunkUpdated(SyncListMapInfo.Operation op, int index, MapInfo oldItem, MapInfo newItem)
        {
            int x = newItem.pos.x - this.pos.x;
            int z = newItem.pos.y - this.pos.y;

            switch (op)
            {
                case SyncListMapInfo.Operation.OP_ADD:
                    // index is where it got added in the list
                    // item is the new item
                    if (isClient)
                    {
                        mapCells[x, z] = new MapCell(this, newItem.pos.x, newItem.pos.y);
                        mapCells[x, z].setTerrain(newItem.type);
                    }
                    break;
                case SyncListMapInfo.Operation.OP_CLEAR:
                    // list got cleared
                    if (isClient)
                    {
                        Destroy(mapCells[x, z].terrain);
                        mapCells[x, z].terrain = null;
                    }
                    break;
                case SyncListMapInfo.Operation.OP_INSERT:
                    // index is where it got added in the list
                    // item is the new item
                    if (isClient)
                    {

                    }
                    break;
                case SyncListMapInfo.Operation.OP_REMOVEAT:
                    // index is where it got removed in the list
                    // item is the item that was removed
                    if (isClient)
                    {

                    }
                    break;
                case SyncListMapInfo.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // item is the previous item
                    if (isClient)
                    {

                    }
                    break;
                case SyncListMapInfo.Operation.OP_DIRTY:
                    // index is the index of the item that was updated
                    // item is the previous item
                    if (isClient)
                    {
                        Destroy(mapCells[x, z].terrain);
                        mapCells[x, z].setTerrain(newItem.type);
                    }
                    break;
            }
        }

        public bool checkPosition(int x, int z)
        {
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
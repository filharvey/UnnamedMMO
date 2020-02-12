using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public struct MapInfo
    {
        public Vector2Int pos;
        public int type;
    }

    public class MapCell
    {
        public int x;
        public int z;

        public MMOObject obj;
        public GameObject terrain;

        public MapInfo info;

        public MapChunk parent;

        public MapCell (MapChunk parent, int posX, int posZ)
        {
            this.parent = parent;

            x = posX;
            z = posZ;

            info.pos = new Vector2Int (posX, posZ);
            info.type = 0;

            obj = null;
            terrain = null;
        }

        public void setTerrain (int type)
        {
            info.type = type;
            GameObject prefab;

            switch (type)
            {
                case 0:
                default:
                    prefab = MMOTerrainManager.instance.grass[0];
                    break;
            }

            Vector3 pos = new Vector3(x, 0, z);
            Quaternion rotation = new Quaternion();
            GameObject terrain = parent.CreatePrefab(prefab, pos, rotation);

            this.terrain = terrain;
        }

        public void setObj (MMOObject obj)
        {
            this.obj = obj;
        }

        public void removeObj ()
        {
            this.obj = null;
        }
    }
}

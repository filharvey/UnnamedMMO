using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MapInfo
    {
        public Vector2Int pos;
        public int type = -1;
        public int rotation = 0;
    }

    public class MapCell
    {
        public string name;

        public int x;
        public int z;

        public MMOObject obj;
        public GameObject terrain;

        public MapInfo info = new MapInfo ();
        public MapChunk parent;

        bool isDirty = false;

        public int type = -1;

        public MapCell (MapChunk parent, int posX, int posZ, int type)
        {
            this.parent = parent;

            x = posX;
            z = posZ;

            info.pos = new Vector2Int (posX, posZ);

            obj = null;
            terrain = null;

            this.type = type;
            info.type = type;
            isDirty = true;
        }

        public void setTerrain(int type)
        {
            if (info.type == type)
                return;

            isDirty = true;
            info.type = type;
        }

        public void createTerrain ()
        {

            if (isDirty)
            {
                parent.DestroyPrefab(terrain);

                GameObject prefab;
                Quaternion rotation = new Quaternion();

                switch (info.type)
                {
                    case 1:
                        prefab = MMOTerrainManager.instance.grass;                      break;

                    case 2:
                        prefab = MMOTerrainManager.instance.grassWaterCorner;
                        break;

                    case 0: // water
                    default:
                        prefab = MMOTerrainManager.instance.water;
                        break;
                }

                float posX = x + parent.bound.min.x + 0.5f;
                float posZ = z + parent.bound.min.z + 0.5f;
                Vector3 pos = new Vector3(posX, 0, posZ);
                terrain = parent.CreatePrefab(prefab, pos, rotation);
                isDirty = false;
            }
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

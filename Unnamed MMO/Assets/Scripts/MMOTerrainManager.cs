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

        public GameObject terrainBase;  // holds all the 

        public int width;

        public int height;

        public int offsetX;

        public int offsetZ;

        public MapCell[,] cells;

        public int cellSize = 1;

        Vector3 min;
        Vector3 max;

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

            this.getTerrainCells();
        }

        // on client
        public override void OnStartClient()
        {
            base.OnStartClient();

            this.getTerrainCells();
        }

        void getTerrainCells()
        {
            min = new Vector3(100000, 100000, 10000);
            max = new Vector3(-100000, -100000, -10000);

            Dictionary<string, GameObject> terrain = new Dictionary<string, GameObject>();

            for (var a = 0; a < terrainBase.transform.childCount; a++)
            {
                var child = terrainBase.transform.GetChild(a);

                var pos = child.position;

                if (pos.x < min.x)
                    min.x = pos.x;
                if (pos.y < min.y)
                    min.y = pos.y;
                if (pos.z < min.z)
                    min.z = pos.z;
                if (pos.x > max.x)
                    max.x = pos.x;
                if (pos.y > max.y)
                    max.y = pos.y;
                if (pos.z > max.z)
                    max.z = pos.z;

                int x = (int)Mathf.Floor(pos.x);
                int z = (int)Mathf.Floor(pos.z);
                string name = x + ":" + z;

                terrain.Add(name, child.gameObject);
            }

            Debug.Log(min + ", " + max);

            width = (int)Mathf.Ceil(max.x - min.x) + 1;
            height = (int)Mathf.Ceil (max.z - min.z) + 1;

            offsetX = (int)Mathf.Floor (min.x);
            offsetZ = (int)Mathf.Floor (min.z);

            cells = new MapCell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    string name = (x + offsetX) + ":" + (z + offsetZ);
                    GameObject t = terrain[name];
                    cells[x, z] = new MapCell(x + offsetX, z + offsetZ);

                    if (!t)
                        Debug.Log("error: " + name);
                    else
                        cells[x, z].setTerrain(t);
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
                    var posX = x + w - width / 2;
                    var posZ = z + d - height / 2;

                    if (posX >= min.x && posX <= max.x &&
                        posZ >= min.z && posZ <= max.z &&
                        cells[posX - offsetX, posZ - offsetZ].obj != null)
                        return false;
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
            x += -offsetX;
            z += -offsetZ;

            if (x >= 0 && x <= width && z >= 0 && z <= height)
                return cells[x, z].obj;

            return null;
        }

        public void addObject (int x, int z, MMOObject obj)
        {
            x += -offsetX;
            z += -offsetZ;

            cells[x, z].setObj(obj);
        }

        public void removeObject (int x, int z)
        {
            x += -offsetX;
            z += -offsetZ;

            cells[x, z].setObj(null);
        }
    }
}
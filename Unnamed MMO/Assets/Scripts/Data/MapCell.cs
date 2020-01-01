using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MapCell
    {
        public int x;
        public int z;

        public MMOObject obj;
        public GameObject terrain;

        public int tileType;

        public MapCell (int posX, int posZ)
        {
            x = posX;
            z = posZ;

            tileType = 0;
            obj = null;
        }

        public void setTerrain (GameObject terrain)
        {
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

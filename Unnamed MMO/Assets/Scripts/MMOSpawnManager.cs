using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.Objects;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Acemobe.MMO
{
    public class MMOSpawnManager : NetworkBehaviour
    {
        public int width = 5;
        public int height = 5;

        public GameObject spawnObj;
        public GameItem spawnItem;

        public int maxNum = 5;
        public int respawnTimer = 6;
        public int radius = 1;

        float lastRespawn = 0;

        List<MMOObject> objects = new List<MMOObject>();

        void Start()
        {
            Debug.Log("Spawn");
            int a = 0;

            while (a < 20 && objects.Count < maxNum)
            {
                spawnObject();
                a++;
            }
        }

        void Update()
        {
            if (isServer)
            {
                lastRespawn += Time.deltaTime;

                if (lastRespawn > respawnTimer)
                {
                    lastRespawn = 0;

                    if (objects.Count < maxNum)
                    {
                        spawnObject();
                    }
                }
            }
        }

        void spawnObject()
        {
            if (!spawnItem)
                return;

            if (!spawnItem.prefab)
                return;

            if (!getLocalTerrainManager)
                return;

            Vector3 pos = transform.position;
            Quaternion rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            int checks = 0;

            // want to check terrain manager for other Objects on this location
            do
            {
                pos = transform.position;
                pos.x += (int)Random.Range(-width / 2, width / 2);
                pos.z += (int)Random.Range(-height / 2, height / 2);
            }
            while (++checks < 5 && !getLocalTerrainManager.checkPosition((int)pos.x, (int)pos.z, radius, radius));

            if (checks >= 5)
                return;

            GameObject obj = Instantiate(spawnItem.prefab, pos + new Vector3 (0.5f, 0f, 0.5f), rotation);
            MMOObject mmoObj = obj.GetComponent<MMOObject>();

            // give link to manager
            mmoObj.manager = this;

            getLocalTerrainManager.addObjectAt((int)pos.x, (int)pos.z, mmoObj);
            objects.Add(mmoObj);

            NetworkServer.Spawn(obj);
        }

        public void killObject (MMOObject obj)
        {
            int x = (int)obj.gameObject.transform.position.x;
            int z = (int)obj.gameObject.transform.position.z;

            getLocalTerrainManager.removeObjectAt(x, z);
            NetworkServer.Destroy(obj.gameObject);
            objects.Remove(obj);
        }

        #region Terrain Manager Finder
        MMOTerrainManager terrainManager;

        MMOTerrainManager getLocalTerrainManager
        {
            get
            {
                if (terrainManager)
                    return terrainManager;

                foreach (var t in MMOTerrainManager.terrains)
                {
                    if (t.Value.worldBounds.Contains(transform.position))
                    {
                        terrainManager = t.Value;
                        break;
                    }
                }

                return terrainManager;
            }
        }
        #endregion
    }
}
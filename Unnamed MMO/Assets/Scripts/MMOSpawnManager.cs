using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MMOSpawnManager : NetworkBehaviour
    {
        public int width = 5;
        public int height = 5;

        public GameObject spawnObj;

        public int maxNum = 5;
        public int respawnTimer = 6;
        public int radius = 1;

        float lastRespawn = 0;

        List<MMOObject> objects = new List<MMOObject>();

        void Start()
        {

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
                checks++;
            }
            while (checks < 5 && !MMOTerrainManager.instance.checkPosition((int)pos.x, (int)pos.z, radius, radius));

            if (checks >= 5)
                return;

            if (MMOTerrainManager.instance.getChunk ((int)pos.x, (int)pos.z))
            {
                GameObject obj = Instantiate(spawnObj, pos + new Vector3 (0.5f, 0, 0.5f), rotation);
                MMOObject mmoObj = obj.GetComponent<MMOObject>();

                // give link to manager
                mmoObj.manager = this;

                MMOTerrainManager.instance.addObject((int)pos.x, (int)pos.z, mmoObj);

                objects.Add(mmoObj);

                NetworkServer.Spawn(obj);
            }
        }

        public void killObject (MMOObject obj)
        {
            int x = (int)obj.gameObject.transform.position.x;
            int z = (int)obj.gameObject.transform.position.z;

            objects.Remove(obj);
            MMOTerrainManager.instance.removeObject(x, z);
            NetworkServer.Destroy(obj.gameObject);
        }
    }
}
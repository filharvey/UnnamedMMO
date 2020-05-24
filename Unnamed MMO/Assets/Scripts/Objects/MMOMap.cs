using Acemobe.MMO.Data.ScriptableObjects;
using BestHTTP;
using Mirror;
using SimpleJSON;
using UnityEngine;
using UnityEngine.AI;

namespace Acemobe.MMO.Objects
{
    public class MMOMap : MonoBehaviour
    {
        public NavMeshSurface navMeshSurface;

        public MMOTerrainManager terrainMap;

        public bool isMain;

        float saveTimer = 0.0f;
        bool isSending = false;

        public int originX;
        public int originZ;

        public string owner;

        void Update()
        {
            if (MMOGameManager.instance &&
                MMOGameManager.instance.isServer)
            {
                if (!isSending)
                {
                    saveTimer += Time.deltaTime;

                    if (saveTimer > 20)
                    {
                        isSending = true;
                        saveWorld();
                        saveTimer = 0;
                    }
                }
            }
        }

        public void loadMap (int x, int z)
        {
            originX = x;
            originZ = z;

            if (!isMain)
            {
                terrainMap.createTerrain();

                createSpawner(x, z, MMOResourceManager.instance.getItem("Tree"));
            }
        }

        void saveWorld ()
        {
            JSONClass map = new JSONClass();
            map["terrain"] = terrainMap.writeData ();

            string url = "http://server.happyisland.life:3000";
            HTTPRequest request = new HTTPRequest(new System.Uri(url + "/updateIsland"), HTTPMethods.Post, (req, response) =>
            {
                if (response != null && response.IsSuccess)
                {
                }

                isSending = false;
            });

            var json = map.ToString();

            request.SetHeader("Content-Type", "application/json; charset=UTF-8");
            request.RawData = System.Text.Encoding.UTF8.GetBytes(json);
            request.SetHeader("hash", "Happy2020");
            request.SetHeader("owner", "server");
            request.Send();
        }

        void createSpawner (int x, int z, GameItem item)
        {
            // create a spwaner
            Vector3 pos = new Vector3(x + 0.5f, 0f, z + 0.5f);
            Quaternion rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(0, 0, 0);

            GameObject spawner = MMOResourceManager.instance.spawnerPrefab;
            GameObject obj = Instantiate(spawner, pos, rotation);
            MMOObject mmoObj = obj.GetComponent<MMOObject>();
            MMOSpawnManager spawnObj = obj.GetComponent<MMOSpawnManager>();
            spawnObj.spawnItem = MMOResourceManager.instance.getItem("Tree");

            terrainMap.addObjectAt(0, 0, mmoObj);
            NetworkServer.Spawn(obj);
        }
    }
}
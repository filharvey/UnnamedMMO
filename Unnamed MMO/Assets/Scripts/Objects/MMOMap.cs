﻿using Acemobe.MMO.Data.ScriptableObjects;
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

        public void loadWorld ()
        {
            string url = "http://server.happyisland.life:3000";
            HTTPRequest request = new HTTPRequest(new System.Uri(url + "/getIsland"), HTTPMethods.Post, (request2, response) =>
            {
                if (response != null && response.IsSuccess)
                {
                    JSONNode result = JSON.Parse(response.DataAsText);

                    if (result["ok"].AsBool == true)
                    {
                        JSONObject data = result["json"].AsObject;

                        if (data != null)
                        {
                            terrainMap.readData (data);
                        }
                    }
                }

                terrainMap.isLoaded = true;
            });

            request.SetHeader("hash", "Happy2020");
            request.SetHeader("owner", owner);
            request.Send();
        }

        void saveWorld ()
        {
            JSONObject map = terrainMap.writeData ();

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
            request.SetHeader("owner", owner);
            request.Send();
        }

        public void createSpawner (int x, int z, GameItem item)
        {
            // create a spwaner
            Vector3 pos = new Vector3(x + 0.5f, 0f, z + 0.5f);
            Quaternion rotation = new Quaternion();

            GameObject spawner = MMOResourceManager.instance.spawnerPrefab;
            GameObject obj = Instantiate(spawner, pos, rotation);
            MMOObject mmoObj = obj.GetComponent<MMOObject>();
            MMOSpawnManager spawnObj = obj.GetComponent<MMOSpawnManager>();
            spawnObj.spawnItem = item;

            terrainMap.addObjectAt(x, z, mmoObj);
            NetworkServer.Spawn(obj);
        }

        public void spawnObject(int x, int z, int angle, GameItem item, bool addToTerrain)
        {
            // create a spwaner
            Vector3 pos = new Vector3(x + 0.5f, 0f, z + 0.5f);
            Quaternion rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(0, angle, 0);

            GameObject spawner = item.prefab;
            GameObject obj = Instantiate(spawner, pos, rotation);
            MMOObject mmoObj = obj.GetComponent<MMOObject>();

            if (addToTerrain)
                terrainMap.addObjectAt(x, z, mmoObj);

            NetworkServer.Spawn(obj);
        }
    }
}
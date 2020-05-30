using Acemobe.MMO.Data;
using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.Objects;
using Mirror;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MMOSpawnManager : NetworkBehaviour
    {
        public int width = 5;
        public int height = 5;

        public string saveName;

        public GameItem spawnItem;

        public int maxNum = 5;
        public int respawnTimer = 6;
        public float respawnVariation = 1;
        public int radius = 1;

        float lastRespawn = 0;

        List<MMOObject> objects = new List<MMOObject>();

        void Start()
        {
            int count = 1;
            saveName = "Spawn:" + spawnItem.name;

            while (getLocalTerrainManager.spawnManagers.ContainsKey (name))
            {
                saveName = "Spawn:" + spawnItem.name + ":" + (++count);
            }

            getLocalTerrainManager.spawnManagers.Add(saveName, this);

            lastRespawn = Random.Range(0, respawnVariation);
        }

        void Update()
        {
            if (isServer && getLocalTerrainManager.isLoaded)
            {
                lastRespawn += Time.deltaTime;

                if (lastRespawn > respawnTimer)
                {
                    lastRespawn = Random.Range(0, respawnVariation);

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

            Data.MapData.TerrainData data1 = getLocalTerrainManager.getTerrainData(17, 1);
            Data.MapData.TerrainData data2 = getLocalTerrainManager.getTerrainData(17, 2);
            Data.MapData.TerrainData data3 = getLocalTerrainManager.getTerrainData(Mathf.FloorToInt(17.5f), Mathf.FloorToInt(2.5f));
            Data.MapData.TerrainData data4 = getLocalTerrainManager.getTerrainData(16, 2);
            Data.MapData.TerrainData data5 = getLocalTerrainManager.getTerrainData(Mathf.FloorToInt (-4.5f), Mathf.FloorToInt(-6.5f));

            // want to check terrain manager for other Objects on this location
            do
            {
                pos = transform.position;
                pos.x += Mathf.FloorToInt (Random.Range(-width / 2, width / 2));
                pos.z += Mathf.FloorToInt (Random.Range(-height / 2, height / 2));
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
            int x = Mathf.FloorToInt(obj.gameObject.transform.position.x);
            int z = Mathf.FloorToInt(obj.gameObject.transform.position.z);

            getLocalTerrainManager.removeObjectAt(x, z);
            NetworkServer.Destroy(obj.gameObject);
            objects.Remove(obj);
        }

        public JSONObject writeData()
        {
            JSONObject json = new JSONObject();
            json["name"] = saveName;
            json["pos"] = new JSONObject();
            json["pos"]["x"].AsInt = Mathf.FloorToInt(transform.localPosition.x);
            json["pos"]["z"].AsInt = Mathf.FloorToInt(transform.localPosition.z);
            json["objs"] = new JSONArray();

            foreach (var o in objects)
            {
                JSONObject objData = o.writeData();

                json["objs"].Add(objData);
            }

            return json;
        }

        public void readData (JSONObject json)
        {
            JSONArray objs = json["objs"].AsArray;

            for (int a = 0; a < objs.Count; a++)
            {
                JSONObject obj = objs[a].AsObject;

                if (obj != null)
                {
                    int x = obj["pos"]["x"].AsInt;
                    int z = obj["pos"]["z"].AsInt;
                    float angle = obj["angle"].AsFloat;
                    int gI = obj["gameItem"].AsInt;

                    Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                    Quaternion rotation = new Quaternion();
                    rotation.eulerAngles = new Vector3(0, angle, 0);

                    GameItem gameItem = MMOResourceManager.instance.getItemByType((MMOItemType)gI);

                    GameObject newObj = Instantiate(gameItem.prefab, pos, rotation);
                    MMOObject mmoObj = newObj.GetComponent<MMOObject>();

                    mmoObj.readData(obj);

                    // give link to manager
                    mmoObj.manager = this;

                    getLocalTerrainManager.addObjectAt((int)pos.x, (int)pos.z, mmoObj);
                    objects.Add(mmoObj);

                    NetworkServer.Spawn(newObj);
                }
            }
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
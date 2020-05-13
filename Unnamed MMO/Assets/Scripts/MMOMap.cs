using BestHTTP;
using SimpleJSON;
using UnityEngine;
using UnityEngine.AI;

namespace Acemobe.MMO
{
    public class MMOMap : MonoBehaviour
    {
        public NavMeshSurface navMeshSurface;

        public MMOTerrainManager terrainMap;

        float saveTimer = 0.0f;

        void Update()
        {
            saveTimer += Time.deltaTime;

            if (saveTimer > 5)
            {
//                saveWorld();
                saveTimer = 0;
            }
        }

        void loadWorld ()
        {

        }

        void saveWorld ()
        {
            JSONClass map = new JSONClass();

            map["terrain"] = new JSONArray();

            foreach (var t in terrainMap.terrain)
            {
                Data.MapData.TerrainData terrainData = t.Value;

                JSONClass data = new JSONClass();
                map["terrain"].Add(data);

                // add object
                if (terrainData.obj)
                {
                    data["obj"] = new JSONClass();

                    // add walls
                    data["walls"] = new JSONClass();
                }
            }

            /*            HTTPRequest request = new HTTPRequest(new System.Uri("http://157.245.226.33:3000/updateIsland"), HTTPMethods.Post, (req, response) =>
                        {
                            if (response.IsSuccess)
                            {
                            }
                        });

                        var json = map.ToString();

                        request.SetHeader("Content-Type", "application/json; charset=UTF-8");
                        request.RawData = System.Text.Encoding.UTF8.GetBytes(json);
                        request.Send();
            */
        }
    }
}
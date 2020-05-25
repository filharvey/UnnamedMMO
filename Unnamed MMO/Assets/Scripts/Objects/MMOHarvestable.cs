using Acemobe.MMO.Data;
using Mirror;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.Objects
{
    public class MMOHarvestable : MMOObject
    {
        [Header("Harvesting")]
        public MMOResourceAction    action;             
        public float                harvestTime = 2;
        public int                  harvestGain = 1;
        public MMOItemType          harvestResource;

        [SyncVar]
        [Header("DisplayStates")]
        public int                  meshIdx;
        public List<GameObject>     displayMesh;

        [Header("GrowthStates")]
        public bool                 canGrow = false;                // move to gameItem?
        public int                  growthTime = 20;
        public float                finalSize = 1f;
        public float                startScale = 0.25f;
        public float                variation = 0.1f;

        public override void OnStartServer()
        {
            base.OnStartServer();

            finalSize = finalSize + Random.Range(-variation, variation);
            startScale = Mathf.Min(startScale, finalSize);

            transform.localScale = new Vector3(startScale, startScale, startScale);

            if (displayMesh.Count > 0)
            {
                meshIdx = (int) Mathf.Floor(Random.Range(0, displayMesh.Count - 1));

                for (int a =  0; a < displayMesh.Count; a++)
                {
                    if (a == meshIdx)
                        displayMesh[a].SetActive(true);
                    else
                        displayMesh[a].SetActive(false);
                }
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (displayMesh.Count > 0)
            {
                for (int a = 0; a < displayMesh.Count; a++)
                {
                    if (a == meshIdx)
                        displayMesh[a].SetActive(true);
                    else
                        displayMesh[a].SetActive(false);
                }
            }
        }

        void FixedUpdate()
        {
            if (isServer)
            {
                if (canGrow)
                {
                    if (lifeTime < growthTime)
                    {
                        lifeTime = Mathf.Min(lifeTime + Time.deltaTime, growthTime);

                        float scale = Mathf.Min(finalSize, startScale + (finalSize - startScale) * (lifeTime / growthTime));
                        transform.localScale = new Vector3(scale, scale, scale);
                    }
                }
            }
        }

        public override JSONClass writeData()
        {
            JSONClass data = base.writeData();
            data["finalSize"].AsFloat = finalSize;
            data["startScale"].AsFloat = startScale;

            return data;
        }

        public override void readData(JSONClass json)
        {
            base.readData(json);

            finalSize = json["finalSize"].AsFloat;
            startScale = json["startScale"].AsFloat;
        }
    }
}

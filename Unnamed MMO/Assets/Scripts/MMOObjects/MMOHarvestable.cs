using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.MMOObjects
{
    public class MMOHarvestable : MMOObject
    {
        [Header("Harvesting")]
        public MMOResourceAction    action;
        public float                harvestTime = 2;
        public int                  harvestGain = 1;
        public MMOItemType          harvestResource;

//        [SyncVar]
//        public int                  meshIdx = 0;
        public List<GameObject>     possibleMesh;

        public float variation = 0.1f;

        [Header("GrowthStates")]
        public bool canGrow = false;
        public int growthTime = 20;

        public float finalSize = 1f;
        public float startScale = 0.25f;

        MeshRenderer render;

        public override void OnStartServer()
        {
            base.OnStartServer();

            finalSize = finalSize + Random.Range(-variation, variation);
            startScale = Mathf.Min(startScale, finalSize);

            transform.localScale = new Vector3(startScale, startScale, startScale);

            /*            if (possibleMesh.Count > 0)
                        {
                            meshIdx = (int) Mathf.Floor(Random.Range(0, possibleMesh.Count - 1));

                            for (var a = 0; a < possibleMesh.Count; a++)
                            {
                                if (a == meshIdx)
                                    possibleMesh[a].SetActive(true);
                                else
                                    possibleMesh[a].SetActive(false);
                            }
                        }
            */
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

/*            if (possibleMesh.Count > 0)
            {
                for (var a = 0; a < possibleMesh.Count; a++)
                {
                    if (a == meshIdx)
                        possibleMesh[a].SetActive(true);
                    else
                        possibleMesh[a].SetActive(false);
                }
            }
*/        }

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
    }
}

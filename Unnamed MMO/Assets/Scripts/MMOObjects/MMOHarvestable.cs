﻿using Mirror;
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
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
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
    }
}

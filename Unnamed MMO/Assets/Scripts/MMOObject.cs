using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public enum MMOObjectTypes
    {
        None = 0,
        Tree = 1,
        Rock = 2,
        Stone = 3,
    };

    public enum MMOResource
    {
        wood = 1,
        stone = 2,
        ironOre = 3
    }

    public class MMOObject : NetworkBehaviour
    {
        [SyncVar]
        public int health;
        [SyncVar]
        public int maxHealth;
        [SyncVar]
        public int life;
        [SyncVar]
        public MMOObjectTypes type;

        public float lifeTime = 0;
        public int growthTime = 20;

        public float finalSize = 1f;
        public float startScale = 0.25f;
        public float variation = 0.1f;

        public MMOSpawnManager manager;
        MeshRenderer render;

        public override void OnStartServer()
        {
            base.OnStartServer();

            health = maxHealth;
            life = 1;

            finalSize = finalSize + Random.Range(-variation, variation);
            startScale = Mathf.Min(startScale, finalSize);

            transform.localScale = new Vector3(startScale, startScale, startScale);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            render = gameObject.GetComponent<MeshRenderer>();
            render.enabled = true;
        }

        void FixedUpdate()
        {
            if (isClient)
            {

            }

            if (isServer)
            {
                if (lifeTime < growthTime)
                {
                    lifeTime = Mathf.Min(lifeTime + Time.deltaTime, growthTime);

                    float scale = Mathf.Min(finalSize, startScale + (finalSize - startScale) * (lifeTime / growthTime));
                    transform.localScale = new Vector3(scale, scale, scale);
                }

                if (health <= 0)
                {

                }
            }
        }
    }
}

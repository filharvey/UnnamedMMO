using Acemobe.MMO.MMOObjects;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.MMOObjects
{
    public class MMOCrop : MMOObject
    {
        [SyncVar]
        public int life;
        [SyncVar]
        public bool isWatered;
        [SyncVar]
        public bool isFertalized;

        public float lifeTime = 0;
        public int growthTime = 20;

        // handle watering
        public float waterTime = 0;

        // handle fertalizing
        public float fertalizeTime = 0;

        // Start is called before the first frame update
        void Start()
        {
            life = 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (lifeTime < growthTime)
            {
                lifeTime = Mathf.Min(lifeTime + Time.deltaTime, growthTime);
            }
        }
    }
}
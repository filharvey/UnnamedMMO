using Acemobe.MMO.Data.ScriptableObjects;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.Objects
{
    public class MMOCrop : MMOObject
    {
        [SyncVar]
        public bool isWatered;
        [SyncVar]
        public bool isFertalized;

        public GameItem crop;

        public List<GameObject> states = new List<GameObject>();

        // handle watering
        float waterTime = 0;

        // handle fertalizing
        float fertalizeTime = 0;

        // Start is called before the first frame update
        void Start()
        {
            for (var a = 0; a < states.Count; a++)
            {
                if (a == 0)
                    states[a].SetActive(true);
                else
                    states[a].SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            lifeTime = Mathf.Min(lifeTime + Time.deltaTime, crop.growthTime);

            if (states.Count > 0)
            {
                int count = states.Count;
                float stepTime = crop.growthTime / count;
                int lifeState = (int)(lifeTime / stepTime);

                lifeState = Mathf.Min(lifeState, count - 1);

                for (var a = 0; a < states.Count; a++)
                {
                    if (a == lifeState)
                        states[a].SetActive(true);
                    else
                        states[a].SetActive(false);
                }
            }
        }

        public bool isGrown ()
        {
            if (lifeTime >= crop.growthTime)
                return true;

            return false;
        }
    }
}
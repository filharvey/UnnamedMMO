using Acemobe.MMO.Data.ScriptableObjects;
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
        public bool isWatered;
        [SyncVar]
        public bool isFertalized;

        public CropData cropData;

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
            lifeTime = Mathf.Min(lifeTime + Time.deltaTime, cropData.growthTime);

            if (states.Count > 0)
            {
                int count = states.Count;
                float step = lifeTime / count;
                int lifeState = Mathf.Min((int)(lifeTime / (cropData.growthTime / count)), states.Count - 1);

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
            if (lifeTime >= cropData.growthTime)
                return true;

            return false;
        }
    }
}
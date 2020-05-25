using Acemobe.MMO.Data.ScriptableObjects;
using Mirror;
using SimpleJSON;
using UnityEngine;

namespace Acemobe.MMO.Objects
{
    public class MMOObject : NetworkBehaviour
    {
        [SyncVar]
        public int health;
        [SyncVar]
        public int maxHealth;

        [SyncVar]
        [HideInInspector]
        public float lifeTime;

        public GameItem gameItem;

        [HideInInspector]
        public MMOSpawnManager manager;

        public override void OnStartServer()
        {
            base.OnStartServer();

            health = maxHealth;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        public virtual void AnimComplete()
        {
        }

        public virtual JSONClass writeData ()
        {
            JSONClass data = new JSONClass();

            data["pos"] = new JSONClass();
            data["pos"]["x"].AsInt = Mathf.FloorToInt(transform.localPosition.x);
            data["pos"]["z"].AsInt = Mathf.FloorToInt(transform.localPosition.z);
            data["angle"].AsFloat = transform.eulerAngles.y;

            data["gameItem"].AsInt = (int)gameItem.itemType;
            data["health"].AsInt = health;
            data["maxHealth"].AsInt = maxHealth;
            data["lifeTime"].AsFloat = lifeTime;

            return data;
        }

        public virtual void readData (JSONClass json)
        {
            health = json["health"].AsInt;
            lifeTime = json["lifeTime"].AsFloat;
        }
    }
}

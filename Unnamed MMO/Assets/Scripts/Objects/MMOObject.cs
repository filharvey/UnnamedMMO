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

            data["health"].AsInt = health;
            data["maxHealth"].AsInt = maxHealth;
            data["lifeTime"].AsFloat = lifeTime;
            data["gameItem"].AsInt = (int) gameItem.itemType;

            return data;
        }


    }
}

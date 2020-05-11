using Acemobe.MMO.Data.ScriptableObjects;
using Mirror;
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
    }
}

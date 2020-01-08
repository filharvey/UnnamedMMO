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


        Monster = 10
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
        public MMOObjectTypes type;

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

        void FixedUpdate()
        {
            if (isClient)
            {

            }

            if (isServer)
            {
            }
        }
    }
}

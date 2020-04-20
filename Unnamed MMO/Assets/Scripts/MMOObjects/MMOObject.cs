using Acemobe.MMO.Data.ScriptableObjects;
using Mirror;
using UnityEngine;

namespace Acemobe.MMO.MMOObjects
{
    public enum MMOObjectTypes
    {
        None = 0,
        Object = 1,
        Resource = 2,
        Item = 3,
        Crop = 4,
        AI,
        NPC,

        Monster = 10
    };

    public enum MMOItemType
    {
        None = 0,
        Wood = 1,
        Stone = 2,
        IronOre = 3,
        Gold = 4,

        Carrot = 500,

        Tree = 750,
        Rock,

        Axe = 1000,
        PickAxe,
        Spade,
        WateringCan,

        Sword = 2000
    };

    public enum MMOResourceAction
    {
        None = 0,
        Chop = 1,
        Pickup = 2,
        Mining = 3,
        Plant,
        Gather,
        Talk,
        Craft,
        Attack
    };

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
    }
}

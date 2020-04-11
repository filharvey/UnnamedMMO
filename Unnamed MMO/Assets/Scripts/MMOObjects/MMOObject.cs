using Mirror;

namespace Acemobe.MMO.MMOObjects
{
    public enum MMOObjectTypes
    {
        None = 0,
        Object = 1,
        Resource = 2,
        Item = 3,
        Crop = 4,

        Monster = 10
    };

    public enum MMOItemType
    {
        Wood = 1,
        Stone = 2,
        IronOre = 3,
        Gold = 4,

        Carrot = 500,

        Axe = 1000,
        PickAxe,
        Spade,
        WateringCan,

        Sword = 2000
    };

    public enum MMOResourceAction
    {
        Chop = 1,
        Pickup = 2,
        Mining = 3,
    };

    public class MMOObject : NetworkBehaviour
    {
        [SyncVar]
        public int health;
        [SyncVar]
        public int maxHealth;
        [SyncVar]
        public MMOObjectTypes type;
        [SyncVar]
        public MMOItemType resouce;

        public bool instant = false;

        public MMOResourceAction action;
        public float miningTime = 2;
        public int miningGain = 1;

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

using Mirror;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MMOPlayer : NetworkBehaviour
    {
        static public MMOPlayer localPlayer;

        [Header("Components")]
        public MMOFakePlayer serverobj;
        public MMOPlayerInventory inventory;

        [Header("Body Params")]
        [SyncVar]
        public string head;
        [SyncVar]
        public string torso;
        [SyncVar]
        public string bottom;
        [SyncVar]
        public string feet;
        [SyncVar]
        public string hand;
        [SyncVar]
        public string belt;

        [SyncVar]
        public Weapon weapon;

        [SyncVar]
        public string displayItem;

        [Header("Spawn Objects")]
        public GameObject projectilePrefab;
        public GameObject fakePlayer;

        [Header("Game Vars")]
        [SyncVar]
        public float health;
        [SyncVar]
        public bool isFiring = false;
        [SyncVar]
        public bool isMoving = false;
        [SyncVar]
        public bool isMouseDown = false;
        [SyncVar]
        public float lastFire = 0f;
        [SyncVar]
        public float moveHoriz = 0f;
        [SyncVar]
        public float moveVert = 0f;

        public float speed = 200;

        [Header("Mouse Down Vars")]
        public MMOObject mouseDownTarget;
        public int mouseDownX = -1;
        public int mouseDownZ = -1;
        public float mouseDownTimer = 0;
        public Transform activeItem;

        public float mouseRange = 1.9f;

        // on server
        public override void OnStartServer()
        {
            base.OnStartServer();

            Debug.Log("Player OnStartServer," + torso + "," + head);
            Debug.Log(transform.position);
        }

        // on client
        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        // on local player
        public override void OnStartLocalPlayer()
        {
            MMOPlayer.localPlayer = this;

            base.OnStartLocalPlayer();

            UIManager.instance.actionBar.gameObject.SetActive (true);
        }

        void Update()
        {
        }

        void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                if (isClient)
                {
                    float horizontal = Input.GetAxis("Horizontal");
                    float vertical = Input.GetAxis("Vertical");

                    // rotation
                    Vector3 screenPos = Input.mousePosition;
                    screenPos.x -= Screen.width / 2;
                    screenPos.y -= Screen.height / 2;

                    float angle = Mathf.Atan2(-screenPos.y, screenPos.x);
                    angle *= 180 / Mathf.PI;
                    angle += MMOGameCamera.instance.cameraRotation;

                    // send movement
                    CmdMove(NetworkTime.time, horizontal, vertical, (int)angle * 10);

                    // send fire
                    if (Input.GetMouseButton(0))
                    {
                        CmdFire(true);
                    }
                    else if (isFiring)
                    {
                        CmdFire(false);
                    }

                    // update from serverObj
                    if (serverobj)
                    {
                        transform.position = serverobj.transform.position;
                        transform.rotation = serverobj.transform.rotation;
                    }
                }

                if (isServer)
                {
                    // do action
                    if (isMouseDown)
                    {
                        doMouseDownAction();
                    }
                }
            }

            if (isServer)
            {
                if (serverobj == null)
                {
                    GameObject obj = Instantiate(fakePlayer, transform.position, transform.rotation);
                    serverobj = obj.GetComponent<MMOFakePlayer>();
                    serverobj.owner = obj;
                    serverobj.netIdOwner = netId;
                    serverobj.torso = torso;
                    serverobj.head = head;
                    serverobj.bottom = bottom;
                    serverobj.feet = feet;
                    serverobj.hand = hand;
                    serverobj.belt = belt;
                    serverobj.weapon = weapon;

                    NetworkServer.Spawn(obj);
                }
            }

            if (isClient)
            {
                // display item if they are using one
                if (displayItem != "")
                {
                    if (activeItem != serverobj.weapons[displayItem])
                    {
                        if (activeItem != null)
                        {
                            activeItem.gameObject.SetActive(false);
                        }

                        serverobj.weapons[displayItem].gameObject.SetActive(true);
                        activeItem = serverobj.weapons[displayItem];
                    }
                }
                else if (activeItem)
                {
                    activeItem.gameObject.SetActive(false);
                    activeItem = null;
                }
            }
        }

        bool checkDist (Transform trans, float d)
        {
            float dx = trans.position.x - serverobj.transform.position.x;
            float dz = trans.position.z - serverobj.transform.position.z;
            float dist = (dx * dx) + (dz + dz);

            if (dist < d * d)
                return true;

            return false;
        }

        void doMouseDownAction ()
        {
            if (mouseDownTarget)
            {
                mouseDownTimer += Time.fixedDeltaTime;

                if (checkDist (mouseDownTarget.transform, mouseRange))
                {
                    switch (mouseDownTarget.type)
                    {
                        case MMOObjectTypes.Stone:
                            break;

                        case MMOObjectTypes.Tree:
                            // inventory add wood
                            if (mouseDownTimer > 1.18f)
                            {
                                mouseDownTimer -= 1.18f;
                                mouseDownTarget.health -= 10;
                                Item item = new Item
                                {
                                    name = "Wood",
                                    itemID = MMOResource.wood,
                                    amount = 1
                                };

                                if (!inventory.addItem(item))
                                {
                                    // drop on floor
                                }

                                if (mouseDownTarget.health <= 0)
                                {
                                    mouseDownTarget.manager.killObject(mouseDownTarget);
                                    mouseDownTarget = null;
                                    serverobj.animator.SetBool("Mining", false);
                                    serverobj.weapons["pickaxe"].gameObject.SetActive(false);
                                    displayItem = "";
                                }
                            }
                            break;

                        case MMOObjectTypes.Rock:
                            // inventory add wood
                            if (mouseDownTimer > 1.18f)
                            {
                                mouseDownTimer -= 1.18f;
                                mouseDownTarget.health -= 10;
                                Item item = new Item
                                {
                                    name = "Rock",
                                    itemID = MMOResource.stone,
                                    amount = 1
                                };

                                inventory.addItem(item);

                                if (mouseDownTarget.health <= 0)
                                {
                                    mouseDownTarget.manager.killObject(mouseDownTarget);
                                    mouseDownTarget = null;
                                    serverobj.animator.SetBool("Mining", false);
                                    serverobj.weapons["pickaxe"].gameObject.SetActive(false);
                                    displayItem = "";
                                }
                            }
                            break;
                    }
                }
            }
        }

        void startAction (MMOObject obj)
        {
            mouseDownTarget = obj;
            mouseDownTimer = 0;

            switch (mouseDownTarget.type)
            {
                case MMOObjectTypes.Stone:
                    // inventory add rock
                    {
                        obj.manager.killObject(obj);

                        Item item = new Item
                        {
                            name = "Rock",
                            itemID = MMOResource.stone,
                            amount = 1
                        };

                        inventory.addItem(item);
                    }
                    break;

                case MMOObjectTypes.Tree:
                    // inventory add wood
                    {
                        serverobj.animator.SetBool("Mining", true);
                        serverobj.weapons["pickaxe"].gameObject.SetActive(true);
                        displayItem = "pickaxe";
                    }
                    break;

                case MMOObjectTypes.Rock:
                    // inventory add wood
                    {
                        serverobj.animator.SetBool("Mining", true);
                        serverobj.weapons["pickaxe"].gameObject.SetActive(true);
                        displayItem = "pickaxe";
                    }
                    break;
            }
        }

        void updateAction(MMOObject obj)
        {
            if (obj)
            {
                // if new object
                if (obj != mouseDownTarget)
                {
                    if (checkDist(obj.transform, mouseRange))
                    {
                        startAction(obj);
                    }
                }
                else
                {
                    if (!checkDist(mouseDownTarget.transform, mouseRange))
                    {
                        // stop action
                        mouseDownTarget = null;
                    }
                }
            }
            else
            {
                mouseDownTarget = null;
                serverobj.animator.SetBool("Mining", false);
                serverobj.weapons["pickaxe"].gameObject.SetActive(false);
                displayItem = "";
            }
        }

        [Command]
        void CmdMove(double time, float horiz, float vert, int angle)
        {
            if (serverobj)
            {
                var rotation = serverobj.transform.rotation;
                rotation.eulerAngles = new Vector3(0, angle / 10, 0);
                serverobj.transform.rotation = rotation;

                serverobj.moveHoriz = horiz;
                serverobj.moveVert = vert;
                serverobj.movePlayer(0);
            }
        }

        // this is called on the server
        [Command]
        void CmdFire(bool state)
        {
            serverobj.isFiring = state;
        }

        public void mouseDown(int x, int z)
        {
            if (isLocalPlayer)
            {
                CmdMouseDown(x, z);
            }
        }

        [Command]
        public void CmdMouseDown(int x, int z)
        {
            isMouseDown = true;
            mouseDownX = x;
            mouseDownZ = z;

            // check action on cell to see if something can happen
            MMOObject obj = MMOTerrainManager.instance.getObject (x, z);

            if (obj && serverobj)
            {
                if (checkDist(obj.transform, 1.6f))
                {
                    startAction(obj);
                }
            }
        }

        [Command]
        public void CmdMouseUpdate(int x, int z)
        {
            mouseDownX = x;
            mouseDownZ = z;

            MMOObject obj = MMOTerrainManager.instance.getObject(x, z);

            updateAction(obj);
        }

        [Command]
        public void CmdMouseUp()
        {
            isMouseDown = false;

            if (mouseDownTarget)
            {
                // set animation to false
                switch (mouseDownTarget.type)
                {
                    case MMOObjectTypes.Stone:
                        break;

                    case MMOObjectTypes.Tree:
                        break;

                    case MMOObjectTypes.Rock:
                        break;
                }

                mouseDownTarget = null;
            }

            serverobj.animator.SetBool("Mining", false);
            serverobj.weapons["pickaxe"].gameObject.SetActive(false);
            displayItem = "";
        }

        [ClientRpc]
        void RpcOnFire()
        {
            // show effect
        }

        [ServerCallback]
        void OnTriggerEnter(Collider co)
        {
            health -= 10;
        }
    }
}

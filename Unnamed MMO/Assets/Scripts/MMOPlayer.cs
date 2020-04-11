using Acemobe.MMO.Data;
using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.MMOObjects;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Acemobe.MMO
{
    public class MMOPlayer : NetworkBehaviour
    {
        static public MMOPlayer localPlayer;

        Camera mainCamera;

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
        public Transform activeTool;

        public float mouseRange = 1.9f;

        public int activeItem = 0;

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

            mainCamera = MMOGameCamera.instance.GetComponent<Camera>();
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
                    if (Input.GetKeyDown(KeyCode.I))
                    {
                        UIManager.instance.inventory.gameObject.SetActive(true);
                        UIManager.instance.inventory.updateInventory();
                    }

                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        RaycastHit hit;
                        int layerMask = (1 << 14) + (1 << 13);
                    
                        if (Input.GetMouseButtonDown(0))
                        {
                            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                            // Does the ray intersect any objects excluding the player layer
                            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                            {
                                var gameObject = hit.transform.gameObject;

                                if (gameObject)
                                {
                                    MMOObject mmoObj = gameObject.GetComponent<MMOObject>();

                                    if (mmoObj != null)
                                    {
                                        mouseDownObject(gameObject);
                                    }
                                    else
                                    {
                                        int x = (int)Mathf.Floor(hit.point.x);
                                        int z = (int)Mathf.Floor(hit.point.z);

                                        // send to server command mouse down
                                        mouseDown(x, z);
                                    }
                                }
                            }
                        }
                        else if (Input.GetMouseButton(0))
                        {
                            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                            // Does the ray intersect any objects excluding the player layer
                            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                            {
                                var gameObject = hit.transform.gameObject;

                                if (gameObject)
                                {
                                    MMOObject mmoObj = gameObject.GetComponent<MMOObject>();

                                    if (mmoObj != null)
                                    {
                                        mouseUpdateObject(gameObject);
                                    }
                                    else
                                    {
                                        int x = (int)Mathf.Floor(hit.point.x);
                                        int z = (int)Mathf.Floor(hit.point.z);

                                        // send to server command mouse down
                                        mouseUpdate(x, z);
                                    }
                                }
                            }
                        }
                    }

                    if (!Input.GetMouseButton(0) && MMOPlayer.localPlayer.isMouseDown)
                    {
                        CmdMouseUp();
                    }

                    // handle movement
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
                    if (activeTool != serverobj.weapons[displayItem])
                    {
                        if (activeTool != null)
                        {
                            activeTool.gameObject.SetActive(false);
                        }

                        serverobj.weapons[displayItem].gameObject.SetActive(true);
                        activeTool = serverobj.weapons[displayItem];
                    }
                }
                else if (activeTool)
                {
                    activeTool.gameObject.SetActive(false);
                    activeTool = null;
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
                    // look at object
                    transform.LookAt(mouseDownTarget.transform);

                    // do action
                    switch (mouseDownTarget.type)
                    {
                        case MMOObjectTypes.Object:
                            // timer
                            if (mouseDownTimer >= mouseDownTarget.miningTime)
                            {
                                mouseDownTimer -= mouseDownTarget.miningTime;
                                mouseDownTarget.health -= 10;

                                MMOInventoryItem item = new MMOInventoryItem
                                {
                                    type = mouseDownTarget.resouce,
                                    amount = mouseDownTarget.miningGain
                                };

                                if (!inventory.addItem(item))
                                {
                                    // drop on floor
                                }

                                if (mouseDownTarget.health <= 0)
                                {
                                    switch (mouseDownTarget.action)
                                    {
                                        case MMOResourceAction.Chop:
                                        case MMOResourceAction.Mining:
                                            // stop actions
                                            serverobj.animator.SetBool("Mining", false);
                                            serverobj.weapons["axe"].gameObject.SetActive(false);
                                            break;
                                    }

                                    displayItem = "";
                                    mouseDownTarget.manager.killObject(mouseDownTarget);
                                    mouseDownTarget = null;
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
                case MMOObjectTypes.Object:
                    if (mouseDownTarget.instant)
                    {
                        obj.manager.killObject(obj);

                        MMOInventoryItem item = new MMOInventoryItem
                        {
                            type = mouseDownTarget.resouce,
                            amount = mouseDownTarget.miningGain
                        };

                        inventory.addItem(item);
                    }
                    else
                    {
                        switch (mouseDownTarget.action)
                        {
                            case MMOResourceAction.Chop:
                            case MMOResourceAction.Mining:
                                // show item needed
                                serverobj.animator.SetBool("Mining", true);
                                serverobj.weapons["pickaxe"].gameObject.SetActive(true);
                                displayItem = "pickaxe";
                                break;
                        }
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
                serverobj.lookAngle = angle / 10;
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

        public void mouseDownObject(GameObject obj)
        {
            if (isLocalPlayer)
            {
                NetworkIdentity identity = obj.GetComponent<NetworkIdentity>();

                CmdMouseDownObject(identity.netId);
            }
        }

        [Command]
        public void CmdMouseDownObject(uint netId)
        {
            if (NetworkIdentity.spawned.TryGetValue(netId, out NetworkIdentity identity))
            {
                GameObject obj = identity.gameObject;

                MMOObject mmoObj = obj.GetComponent<MMOObject>();

                if (mmoObj && serverobj)
                {
                    if (checkDist(obj.transform, 1.6f))
                    {
                        startAction(mmoObj);
                    }
                }
            }
        }

        public void mouseUpdateObject(GameObject obj)
        {
            if (isLocalPlayer)
            {
                NetworkIdentity identity = obj.GetComponent<NetworkIdentity>();

                CmdMouseDownObject(identity.netId);
            }
        }

        [Command]
        public void CmdMouseUpdateObject(uint netId)
        {
            if (NetworkIdentity.spawned.TryGetValue(netId, out NetworkIdentity identity))
            {
                GameObject obj = identity.gameObject;

                MMOObject mmoObj = obj.GetComponent<MMOObject>();

                if (mmoObj && serverobj)
                {
                    updateAction(mmoObj);
                }
            }
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
            if (!serverobj)
                return;

            isMouseDown = true;
            mouseDownX = x;
            mouseDownZ = z;

            // check action on cell to see if something can happen
            MMOObject mmoObj = MMOTerrainManager.instance.getObject (x, z);

            if (mmoObj)
            {
                if (checkDist(mmoObj.transform, 1.6f))
                {
                    startAction(mmoObj);
                }
            }
            else
            {
                // we hit empty ground?
                MapData.TerrainData terrainData = MMOTerrainManager.instance.getTerrainData(x, z);

                if (terrainData)
                {
                    if (terrainData.isPublic && terrainData.obj == null)
                    {
                        Vector3 pos = transform.position;
                        Quaternion rotation = new Quaternion();
                        rotation.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

                        // we can make plant a crop
                        CropData cropData = MMOResourceManager.instance.getCropObject("carrots");

                        GameObject obj = Instantiate(cropData.growthStates[0], pos + new Vector3(0.5f, 0.25f, 0.5f), rotation);
                        MMOObject spawnObj = obj.GetComponent<MMOObject>();
                        MMOTerrainManager.instance.addObject((int)pos.x, (int)pos.z, mmoObj);
                        NetworkServer.Spawn(obj);

                    }
                }
            }
        }

        public void mouseUpdate(int x, int z)
        {
            if (isLocalPlayer)
            {
                CmdMouseUpdate(x, z);
            }
        }

        [Command]
        public void CmdMouseUpdate(int x, int z)
        {
            if (!serverobj)
                return;

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

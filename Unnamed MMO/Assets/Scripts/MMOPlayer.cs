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
//        public MMOObject mouseDownTarget;
//        public int mouseDownX = -1;
//        public int mouseDownZ = -1;
//        public float mouseDownTimer = 0;
        public Transform activeTool;

        [Header("Action Vars")]
        public MMOResourceAction curAction = MMOResourceAction.None;
        public bool doingAction = false;
        public int activeItem = 0;
        public MMOObject actionTarget;
        public MMOObject nextActionTarget;
        public int actionX = -1;
        public int actionZ = -1;

        public float mouseRange = 1.9f;

        // on server
        public override void OnStartServer()
        {
            base.OnStartServer();

            Debug.Log("Player OnStartServer," + torso + "," + head);
            Debug.Log(transform.position);

            MMOInventoryItem item = new MMOInventoryItem
            {
                type = MMOItemType.Axe,
                amount = 1
            };
            inventory.addActionItem(item);

            item = new MMOInventoryItem
            {
                type = MMOItemType.PickAxe,
                amount = 1
            };
            inventory.addActionItem(item);

            item = new MMOInventoryItem
            {
                type = MMOItemType.Sword,
                amount = 1
            };
            inventory.addActionItem(item);
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
                    RaycastHit hit;
                    int layerMask = (1 << 14) + (1 << 13);

                    if (Input.GetKeyDown(KeyCode.I))
                    {
                        UIManager.instance.inventory.updateInventory();
                        UIManager.instance.inventory.gameObject.SetActive(true);
                    }

                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
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

                    if (!Input.GetMouseButton(0))
                    {
                        if (MMOPlayer.localPlayer.isMouseDown)
                        {
                            CmdMouseUp();
                        }
                    }

                    if (!doingAction)
                    {
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

                        // update from serverObj
                        if (serverobj)
                        {
                            transform.position = serverobj.transform.position;
                            transform.rotation = serverobj.transform.rotation;
                        }
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

                    serverobj.player = this;
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

        float getDist(Vector3 pos1, Vector3 pos2)
        {
            float dx = pos1.x - pos2.x;
            float dz = pos1.z - pos2.z;
            float dist = Mathf.Sqrt ((dx * dx) + (dz * dz));

            return dist;
        }

        bool checkDist (Vector3 pos1, Vector3 pos2, float d)
        {
            float dx = pos1.x - pos2.x;
            float dz = pos1.z - pos2.z;
            float dist = (dx * dx) + (dz * dz);

            if (dist < (d * d))
                return true;

            return false;
        }

        #region Handle Actions
        void startAction (MMOObject obj)
        {
            if (obj)
            {
                actionTarget = obj;
                serverobj.transform.LookAt(actionTarget.transform, Vector3.up);

                switch (actionTarget.gameItem.type)
                {
                    case MMOObjectTypes.Crop:
                        if (actionTarget.gameItem.isHarvestable)
                        {
                            MMOCrop cropItem = (MMOCrop)actionTarget;

                            if (cropItem)
                            {
                                if (cropItem.isGrown())
                                {
                                    curAction = MMOResourceAction.Gather;
                                }
                            }
                        }
                        break;

                    case MMOObjectTypes.Object:
                        if (actionTarget.gameItem.isHarvestable)
                        {
                            MMOHarvestable harvestItem = (MMOHarvestable)actionTarget;

                            if (harvestItem)
                            {
                                curAction = harvestItem.action;
                            }
                        }
                        else if (actionTarget.gameItem.isPickable)
                        {
                            obj.manager.killObject(obj);

                            MMOInventoryItem item = new MMOInventoryItem
                            {
                                type = actionTarget.gameItem.itemType,
                                amount = 1
                            };

                            inventory.addItem(item);
                        }
                        break;
                }
            }
            else
            {
                // plant crop
                curAction = MMOResourceAction.Plant;
            }

            switch (curAction)
            {
                case MMOResourceAction.Gather:
                case MMOResourceAction.Plant:
                case MMOResourceAction.Chop:
                case MMOResourceAction.Mining:
                    // show item needed
                    serverobj.animator.SetBool("Mining", true);
                    serverobj.weapons["pickaxe"].gameObject.SetActive(true);
                    displayItem = "pickaxe";
                    break;
            }
        }

        void updateAction(MMOObject obj)
        {
            if (obj)
            {
                nextActionTarget = obj;
            }
            else
            {
                nextActionTarget = null;
            }
        }

        public void AnimComplete()
        {
            bool stopAnim = false;
            MMOResourceAction lastAction = curAction;

            if (actionTarget)
            {
                serverobj.transform.LookAt(actionTarget.transform, Vector3.up);
            }

            switch (curAction)
            {
                case MMOResourceAction.Gather:
                    if (actionTarget.gameItem.isHarvestable)
                    {
                        MMOCrop cropItem = (MMOCrop)actionTarget;

                        if (cropItem)
                        {
                            int x = (int)actionTarget.transform.position.x;
                            int z = (int)actionTarget.transform.position.z;

                            MMOTerrainManager.instance.removeObject(x, z);
                            NetworkServer.Destroy(cropItem.gameObject);
                        }
                    }
                    break;

                case MMOResourceAction.Plant:
                    {
                        var x = actionX;
                        var z = actionZ;

                        var tx = Mathf.Floor(transform.position.x) + 0.5f;
                        var tz = Mathf.Floor(transform.position.z) + 0.5f;
                        float dist = getDist(new Vector3(tx, 0, tz), new Vector3(x + 0.5f, 0, z + 0.5f));

                        if (dist < 1.5f)
                        {
                            // we hit empty ground?
                            MapData.TerrainData terrainData = MMOTerrainManager.instance.getTerrainData(x, z);

                            if (terrainData)
                            {
                                if (terrainData.isPublic && terrainData.obj == null)
                                {
                                    // we can make plant a crop
                                    CropData cropData = MMOResourceManager.instance.getCropObject("CarrotPlant");

                                    if (cropData)
                                    {
                                        Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                                        Quaternion rotation = new Quaternion();
                                        rotation.eulerAngles = new Vector3(0, 0, 0);

                                        GameObject obj = Instantiate(cropData.prefab, pos, rotation);
                                        MMOCrop spawnObj = obj.GetComponent<MMOCrop>();

                                        MMOTerrainManager.instance.addObject(x, z, spawnObj);
                                        NetworkServer.Spawn(obj);
                                    }
                                }
                            }
                        }
                    }
                    break;

                case MMOResourceAction.Chop:
                case MMOResourceAction.Mining:
                    if (actionTarget.gameItem.isHarvestable)
                    {
                        MMOHarvestable harvestItem = (MMOHarvestable)actionTarget;

                        if (harvestItem)
                        {
                            lastAction = harvestItem.action;
                            actionTarget.health -= 10;

                            MMOInventoryItem item = new MMOInventoryItem
                            {
                                type = harvestItem.harvestResource,
                                amount = harvestItem.harvestGain
                            };

                            if (!inventory.addItem(item))
                            {
                                // drop on floor
                            }

                            if (harvestItem.health <= 0)
                            {
                                stopAnim = true;

                                actionTarget.manager.killObject(actionTarget);
                                actionTarget = null;
                            }
                        }
                    }
                    break;
            }

            // do we stop action
            if (isMouseDown == false)
            {
                // stop anim
                stopAnim = false;
            }
            else if (nextActionTarget)
            {
                // target next object
                if (nextActionTarget != actionTarget)
                {
                    actionTarget = nextActionTarget;
                }
            }

            if (stopAnim)
            {
                switch (lastAction)
                {
                    case MMOResourceAction.Chop:
                    case MMOResourceAction.Mining:
                        // stop actions
                        serverobj.animator.SetBool("Mining", false);
                        serverobj.weapons["axe"].gameObject.SetActive(false);
                        break;
                }

                displayItem = "";
            }
        }
        #endregion

        #region server Commands
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
            NetworkIdentity identity = obj.GetComponent<NetworkIdentity>();

            if (identity)
            {
                CmdMouseDownObject(identity.netId);
            }
        }

        [Command]
        public void CmdMouseDownObject(uint netId)
        {
            if (!serverobj)
                return;

            handleMouseDownObject(netId);
        }

        public void handleMouseDownObject(uint netId)
        {
            if (NetworkIdentity.spawned.TryGetValue(netId, out NetworkIdentity identity))
            {
                GameObject obj = identity.gameObject;

                isMouseDown = true;
                actionX = (int)obj.transform.position.x;
                actionZ = (int)obj.transform.position.z;

                if (obj)
                {
                    MMOObject mmoObj = obj.GetComponent<MMOObject>();

                    if (mmoObj)
                    {
                        if (checkDist(transform.position, obj.transform.position, 1.6f))
                        {
                            startAction(mmoObj);
                        }
                    }
                }
            }
        }

        public void mouseUpdateObject(GameObject obj)
        {
            NetworkIdentity identity = obj.GetComponent<NetworkIdentity>();

            if (identity)
            {
                CmdMouseDownObject(identity.netId);
            }
        }

        [Command]
        public void CmdMouseUpdateObject(uint netId)
        {
            if (!serverobj)
                return;

            handleMouseUpdateObject(netId);
        }

        public void handleMouseUpdateObject(uint netId)
        {
            if (NetworkIdentity.spawned.TryGetValue(netId, out NetworkIdentity identity))
            {
                GameObject obj = identity.gameObject;

                if (obj)
                {
                    MMOObject mmoObj = obj.GetComponent<MMOObject>();

                    updateAction(mmoObj);
                }
            }
        }

        public void mouseDown(int x, int z)
        {
            CmdMouseDown(x, z);
        }

        [Command]
        public void CmdMouseDown(int x, int z)
        {
            if (!serverobj)
                return;

            handleMouseDown(x, z);
        }

        public void handleMouseDown (int x, int z)
        { 
            isMouseDown = true;
            actionX = x;
            actionZ = z;

            // check action on cell to see if something can happen
            MMOObject mmoObj = MMOTerrainManager.instance.getObject (x, z);

            if (mmoObj)
            {
                if (checkDist(transform.position, mmoObj.transform.position, 1.6f))
                {
                    startAction(mmoObj);
                }
            }
        }

        public void mouseUpdate(int x, int z)
        {
            CmdMouseUpdate(x, z);
        }

        [Command]
        public void CmdMouseUpdate(int x, int z)
        {
            if (!serverobj)
                return;

            handleMouseUpdate(x, z);
        }

        public void handleMouseUpdate(int x, int z)
        {
            actionX = x;
            actionZ = z;

            MMOObject obj = MMOTerrainManager.instance.getObject(x, z);

            updateAction(obj);
        }

        [Command]
        public void CmdMouseUp()
        {
            handleMouseUp();
        }

        public void handleMouseUp()
        {
            nextActionTarget = null;
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

        #endregion
    }
}

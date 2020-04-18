using Acemobe.MMO.Data;
using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.MMOObjects;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using Rewired;

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

//        [SyncVar]
//        public string displayItem;

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

        [Header("Action Vars")]
        public MMOResourceAction curAction = MMOResourceAction.None;
        public int activeItem = 0;
        public MMOObject actionTarget;
        public MMOObject nextActionTarget;
        public int actionX = -1;
        public int actionZ = -1;
//        public Transform activeTool;
        public float mouseRange = 1.9f;

        // Rewired
        private Rewired.Player player; // The Rewired Player
        private CharacterController cc;

        // on server
        public override void OnStartServer()
        {
            base.OnStartServer();

            Debug.Log("Player OnStartServer," + torso + "," + head);

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

            // Get the character controller
            player = ReInput.players.GetPlayer(0);

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
                    Vector3 moveVector = new Vector3 ();

                    moveVector.x = player.GetAxis("Move Horizontal"); // get input by name or action id
                    moveVector.y = player.GetAxis("Move Vertical");

                    RaycastHit hit;
                    int layerMask = (1 << 14) + (1 << 13);

                    if (player.GetButtonDown("Inventory"))
                    {
                        if (UIManager.instance.inventory.gameObject.activeInHierarchy == true)
                        {
                            UIManager.instance.inventory.gameObject.SetActive(false);
                        }
                        else
                        {
                            UIManager.instance.inventory.updateInventory();
                        }
                    }

                    // mouse pointer
//                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (player.GetButtonDown("FireJoy"))
                        {
                            Vector3 pos = transform.position;
                            Vector3 dir = transform.rotation * new Vector3(0, 0, 1);

                            pos += dir;
                            int x = (int)Mathf.Floor(pos.x);
                            int z = (int)Mathf.Floor(pos.z);

                            // send to server command mouse down
                            mouseDown(x, z);
                        }
                        else if (player.GetButton("FireJoy"))
                        {

                        }

                        if (player.GetButtonDown("Fire"))
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
                        else if (player.GetButton ("Fire"))
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
/*                    else
                    {
                        CmdMouseUp();
                    }
                    */
                    if (MMOPlayer.localPlayer.isMouseDown && 
                        !player.GetButton("Fire") && !player.GetButton("FireJoy"))
                    {
                        CmdMouseUp();
                    }

                    if (curAction == MMOResourceAction.None)
                    {
                        // handle movement
//                        float horizontal = Input.GetAxis("Horizontal");
//                        float vertical = Input.GetAxis("Vertical");
                        moveVector.x = player.GetAxis("Move Horizontal"); // get input by name or action id
                        moveVector.y = player.GetAxis("Move Vertical");

                        // rotation
                        Vector3 screenPos = Input.mousePosition;
                        screenPos.x -= Screen.width / 2;
                        screenPos.y -= Screen.height / 2;

                        float angle = Mathf.Atan2(-screenPos.y, screenPos.x);
                        angle *= 180 / Mathf.PI;
                        angle += MMOGameCamera.instance.cameraRotation;

                        // send movement
                        CmdMove(NetworkTime.time, moveVector.x, moveVector.y, (int)angle * 10);

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
                switch (obj.gameItem.type)
                {
                    case MMOObjectTypes.Crop:
                        if (obj.gameItem.isHarvestable)
                        {
                            MMOCrop cropItem = (MMOCrop)obj;

                            if (cropItem && cropItem.isGrown())
                            {
                                curAction = MMOResourceAction.Gather;
                                actionTarget = obj;
                                serverobj.transform.LookAt(actionTarget.transform, Vector3.up);
                            }
                        }
                        break;

                    case MMOObjectTypes.Object:
                        if (obj.gameItem.isHarvestable)
                        {
                            MMOHarvestable harvestItem = (MMOHarvestable)obj;

                            if (harvestItem)
                            {
                                curAction = harvestItem.action;
                                actionTarget = obj;
                                serverobj.transform.LookAt(actionTarget.transform, Vector3.up);
                            }
                        }
                        else if (obj.gameItem.isPickable)
                        {
                            curAction = MMOResourceAction.Pickup;
                            actionTarget = obj;
                            serverobj.transform.LookAt(actionTarget.transform, Vector3.up);
                        }
                        break;
                }
            }
            else
            {
                MapData.TerrainData terrainData = MMOTerrainManager.instance.getTerrainData(actionX, actionZ);

                if (!terrainData.isInUse)
                {
                    // plant crop
                    curAction = MMOResourceAction.Plant;

                    // look at
                    Vector3 pos = new Vector3(actionX + 0.5f, transform.position.y, actionZ + 0.5f);
                    Vector3 dir = (pos - transform.position).normalized;
                    Quaternion rot = Quaternion.LookRotation(dir);
                    serverobj.transform.rotation = rot;

                    terrainData.isInUse = true;
                }
            }

            // set animation
            switch (curAction)
            {
                case MMOResourceAction.Gather:
                    serverobj.animator.SetBool("Gather", true);
                    break;
                case MMOResourceAction.Plant:
                    serverobj.animator.SetBool("Plant", true);
                    serverobj.weapons["plow"].gameObject.SetActive(true);
                    serverobj.displayItem = "plow";
                    break;
                case MMOResourceAction.Chop:
                case MMOResourceAction.Mining:
                case MMOResourceAction.Pickup:
                    // show item needed
                    serverobj.animator.SetBool("Mining", true);
                    serverobj.weapons["pickaxe"].gameObject.SetActive(true);
                    serverobj.displayItem = "pickaxe";
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

                        if (cropItem && cropItem.isGrown())
                        {
                            int x = (int)actionTarget.transform.position.x;
                            int z = (int)actionTarget.transform.position.z;

                            MMOTerrainManager.instance.removeObjectAt(x, z);
                            NetworkServer.Destroy(cropItem.gameObject);
                        }

                        actionTarget = null;
                        stopAnim = true;
                    }
                    break;

                case MMOResourceAction.Plant:
                    {
                        var x = actionX;
                        var z = actionZ;

                        // we hit empty ground?
                        MapData.TerrainData terrainData = MMOTerrainManager.instance.getTerrainData(x, z);

                        if (terrainData && terrainData.isPublic && terrainData.obj == null)
                        {
                            var tx = Mathf.Floor(transform.position.x);
                            var tz = Mathf.Floor(transform.position.z);
                            float dist = getDist(new Vector3(tx, 0, tz), new Vector3(x + 0.5f, 0, z + 0.5f));

                            if (dist < mouseRange)
                            {
                                // get the crop we are planting
                                CropData cropData = MMOResourceManager.instance.getCropObject("CarrotPlant");

                                if (cropData)
                                {
                                    Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                                    Quaternion rotation = new Quaternion();
                                    rotation.eulerAngles = new Vector3(0, 0, 0);

                                    GameObject obj = Instantiate(cropData.prefab, pos, rotation);
                                    MMOCrop spawnObj = obj.GetComponent<MMOCrop>();

                                    MMOTerrainManager.instance.addObjectAt(x, z, spawnObj);
                                    NetworkServer.Spawn(obj);
                                }
                            }

                            terrainData.isInUse = false;
                        }

                        stopAnim = true;
                    }
                    break;

                case MMOResourceAction.Pickup:
                    if (actionTarget.gameItem.isPickable)
                    {
                        actionTarget.manager.killObject(actionTarget);

                        MMOInventoryItem item = new MMOInventoryItem
                        {
                            type = actionTarget.gameItem.itemType,
                            amount = 1
                        };

                                                    RpcInventoryGain(item);

                        inventory.addItem(item);
                        stopAnim = true;
                        actionTarget = null;
                    }
                    break;

                case MMOResourceAction.Chop:
                case MMOResourceAction.Mining:
                    if (actionTarget.gameItem.isHarvestable)
                    {
                        MMOHarvestable harvestItem = (MMOHarvestable)actionTarget;

                        if (harvestItem)
                        {
                            actionTarget.health -= 10;

                            MMOInventoryItem item = new MMOInventoryItem
                            {
                                type = harvestItem.harvestResource,
                                amount = harvestItem.harvestGain
                            };

                            RpcInventoryGain(item);

                            if (!inventory.addItem(item))
                            {
                                // drop on floor
                            }

                            if (harvestItem.health <= 0)
                            {
                                actionTarget.manager.killObject(actionTarget);

                                stopAnim = true;
                                actionTarget = null;
                            }
                        }
                    }
                    break;
            }

            // do we stop action
            if (isMouseDown == false)
            {
                // set no target
                actionTarget = null;
            }
            // do we have a new target
            else if (nextActionTarget)
            {
                if (nextActionTarget != actionTarget)
                {
                    actionTarget = nextActionTarget;
                }
            }

            // if no target
            if (actionTarget == null)
                stopAnim = true;

            // or if stop (ie dead)
            if (stopAnim)
            {
                switch (curAction)
                {
                    case MMOResourceAction.Gather:
                        serverobj.animator.SetBool("Gather", false);
                        break;
                    case MMOResourceAction.Plant:
                        serverobj.animator.SetBool("Plant", false);
                        serverobj.weapons["plow"].gameObject.SetActive(false);
                        break;
                    case MMOResourceAction.Chop:
                    case MMOResourceAction.Mining:
                    case MMOResourceAction.Pickup:
                        // stop actions
                        serverobj.animator.SetBool("Mining", false);
                        serverobj.weapons["axe"].gameObject.SetActive(false);
                        break;
                }

                serverobj.displayItem = "";
                curAction = MMOResourceAction.None;
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
                        if (checkDist(transform.position, obj.transform.position, mouseRange))
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
            MMOObject mmoObj = MMOTerrainManager.instance.getObjectAt (x, z);
            if (mmoObj && checkDist(transform.position, mmoObj.transform.position, mouseRange))
            {
                startAction(mmoObj);
            }
            else
            {
                // we hit empty ground?
                MapData.TerrainData terrainData = MMOTerrainManager.instance.getTerrainData(x, z);

                if (terrainData && terrainData.isPublic && terrainData.obj == null)
                {
                    startAction(null);
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

            MMOObject obj = MMOTerrainManager.instance.getObjectAt(x, z);

            updateAction(obj);
        }

        [Command]
        public void CmdMouseUp()
        {
            handleMouseUp();
        }

        public void handleMouseUp()
        {
            isMouseDown = false;
            nextActionTarget = null;
        }

        [ClientRpc]
        // 
        void RpcInventoryGain(MMOInventoryItem item)
        {
            Debug.Log("-" + item.amount + "x " + item.type);
            UIManager.instance.showItemGain(item);
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

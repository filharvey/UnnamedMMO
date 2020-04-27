using Acemobe.MMO.Data;
using Acemobe.MMO.Data.ScriptableObjects;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using Rewired;
using Acemobe.MMO.UI;
using Acemobe.MMO.AI;
using UnityEngine.SceneManagement;

namespace Acemobe.MMO.Objects
{
    public class MMOPlayer : NetworkBehaviour
    {
        static public MMOPlayer localPlayer;
        static public string userName;
        static public string userHash;
        static public string userData;

        Camera mainCamera;

        [Header("Components")]
        public MMOFakePlayer serverobj;
        public MMOPlayerInventory inventory;

        [Header("Body Params")]
        public string username;
        public string hash;
        public MMOCharacterCustomization characterInfo;

        [Header("Body Params")]
        [SyncVar]
        public int head;
        [SyncVar]
        public int torso;
        [SyncVar]
        public int bottom;
        [SyncVar]
        public int feet;
        [SyncVar]
        public int hand;
        [SyncVar]
        public int belt;

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
        public float mouseRange = 1.9f;
        public Recipies curRecipe;

        // Rewired
        public Rewired.Player player; // The Rewired Player

        // Server Update timer
        float serverUpdateTimer = 0;
        bool isUserDirty = false;
        public bool isUpdating = false;

        // on server
        public override void OnStartServer()
        {
            base.OnStartServer();

            Debug.Log("Player OnStartServer," + torso + "," + head);

            characterInfo.updateInventory(inventory);
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

            UIManager.instance.loginUI.gameObject.SetActive(false);
            UIManager.instance.actionBarUI.gameObject.SetActive (true);

            mainCamera = MMOGameCamera.instance.GetComponent<Camera>();
        }

        enum GAME_LAYERS
        {
            PLANE = 8,
            MAIN_PLAYER = 9,
            PLAYERS = 10,
            MONSTES = 11,
            PROJECTILES = 12,
            OBJECTS =  13,
            TERRAIN = 14,
            GROUND_OBJECTS = 15,
        };

        void Update()
        {
            if (isLocalPlayer)
            {
                if (isClient)
                {
                    Vector3 moveVector = new Vector3();

                    moveVector.x = player.GetAxis("Move Horizontal"); // get input by name or action id
                    moveVector.y = player.GetAxis("Move Vertical");

                    RaycastHit hit;
                    int layerMask = (1 << (int) GAME_LAYERS.TERRAIN) + (1 << (int) GAME_LAYERS.OBJECTS) + (1 << (int)GAME_LAYERS.GROUND_OBJECTS);

                    // show inventory
                    if (player.GetButtonDown("Inventory"))
                    {
                        UIManager.instance.inventoryUI.onShow();
                    }

                    // show crafting
                    if (player.GetButtonDown("Craft"))
                    {
                        UIManager.instance.craftUI.onShow();
                    }

                    // select inventory 1
                    if (player.GetButtonDown("Num1"))
                    {
                        changeActionItem(0);
                    }
                    else if (player.GetButtonDown("Num2"))
                    {
                        changeActionItem(1);
                    }
                    else if (player.GetButtonDown("Num3"))
                    {
                        changeActionItem(2);
                    }
                    else if (player.GetButtonDown("Num4"))
                    {
                        changeActionItem(3);
                    }
                    else if (player.GetButtonDown("Num5"))
                    {
                        changeActionItem(4);
                    }
                    else if (player.GetButtonDown("Num6"))
                    {
                        changeActionItem(5);
                    }
                    else if (player.GetButtonDown("Num7"))
                    {
                        changeActionItem(6);
                    }
                    else if (player.GetButtonDown("Num8"))
                    {
                        changeActionItem(7);
                    }
                    else if (player.GetButtonDown("Num9"))
                    {
                        changeActionItem(8);
                    }
                    else if (player.GetButtonDown("Num0"))
                    {
                        changeActionItem(9);
                    }

                    // mouse pointer
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

                    // if not over UI
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
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
                        else if (player.GetButton("Fire"))
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

                    if (isMouseDown && 
                        !player.GetButton("Fire") && !player.GetButton("FireJoy"))
                    {
                        CmdMouseUp();
                    }

                    if (curAction == MMOResourceAction.None)
                    {
                        // handle movement
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
                    else
                    {
                        CmdMove(NetworkTime.time, 0, 0, 0);
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

                    serverobj.player = this;
                    NetworkServer.Spawn(obj);
                }
                else
                {
                    serverUpdateTimer += Time.deltaTime;

                    if (!isUpdating && 
                        (serverUpdateTimer > 3 || isUserDirty))
                    {
                        isUpdating = true;

                        characterInfo.updatePlayer();

                        // update server
                        serverUpdateTimer = 0;
                        isUserDirty = false;
                    }
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
            GameItem activeItem = inventory.getActiveItem();
            MMOResourceAction curHeldAction = MMOResourceAction.None;

            if (activeItem)
            {
                curHeldAction = activeItem.actionType;
            }

            if (obj)
            {
                switch (obj.gameItem.type)
                {
                    case MMOObjectTypes.NPC:
                        {
                            curAction = MMOResourceAction.Talk;
                            actionTarget = obj;

                            Vector3 pos = actionTarget.transform.position;
                            pos.y = serverobj.transform.position.y;

                            serverobj.transform.LookAt(pos, Vector3.up);

                            MMOChat chat = actionTarget.GetComponent<MMOChat>();

                            if (chat)
                            {
                                chat.startChat();
                            }
                        }
                        break;

                    case MMOObjectTypes.Crop:
                        if (obj.gameItem.isHarvestable &&
                            curHeldAction == MMOResourceAction.None)
                        {
                            MMOCrop cropItem = (MMOCrop)obj;

                            if (cropItem && cropItem.isGrown())
                            {
                                curAction = MMOResourceAction.Gather;
                                actionTarget = obj;
                                Vector3 pos = actionTarget.transform.position;
                                pos.y = serverobj.transform.position.y;

                                serverobj.transform.LookAt(pos, Vector3.up);
                            }
                        }
                        break;

                    case MMOObjectTypes.Object:
                    case MMOObjectTypes.Resource:
                        if (obj.gameItem.isHarvestable)
                        {
                            MMOHarvestable harvestItem = (MMOHarvestable)obj;

                            if (harvestItem &&
                                (curHeldAction == MMOResourceAction.Mining ||
                                curHeldAction == MMOResourceAction.Chop))
                            {
                                curAction = harvestItem.action;
                                actionTarget = obj;
                                Vector3 pos = actionTarget.transform.position;
                                pos.y = serverobj.transform.position.y;

                                serverobj.transform.LookAt(pos, Vector3.up);
                            }
                        }
                        else if (obj.gameItem.isPickable)
                        {
                            curAction = MMOResourceAction.Pickup;
                            actionTarget = obj;
                            Vector3 pos = actionTarget.transform.position;
                            pos.y = serverobj.transform.position.y;

                            serverobj.transform.LookAt(pos, Vector3.up);
                        }
                        break;
                }
            }
            else if (curHeldAction == MMOResourceAction.Plant)
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
                case MMOResourceAction.Talk:
                    serverobj.animator.SetBool("Talk", true);
                    break;
                case MMOResourceAction.Gather:
                case MMOResourceAction.Pickup:
                    serverobj.animator.SetBool("Gather", true);
                    break;
                case MMOResourceAction.Plant:
                    serverobj.animator.SetBool("Plant", true);
                    break;
                case MMOResourceAction.Chop:
                    serverobj.animator.SetBool("Chop", true);
                    break;
                case MMOResourceAction.Mining:
                    // show item needed
                    serverobj.animator.SetBool("Mining", true);
                    break;
            }
        }

        public void startCraft (Recipies recipe)
        {
            if (curAction != MMOResourceAction.None)
                return;

            CmdStartCraft(recipe.name);
        }

        [Command]
        public void CmdStartCraft(string name)
        {
            if (!serverobj)
                return;

            curRecipe = MMOResourceManager.instance.getRecipeObject(name);
            curAction = MMOResourceAction.Craft;
            serverobj.animator.SetBool("Gather", true);
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
            MMOInventoryItem item;

            if (actionTarget)
            {
                serverobj.transform.LookAt(actionTarget.transform, Vector3.up);
            }

            switch (curAction)
            {
                case MMOResourceAction.Talk:
                    return;
                    break;

                case MMOResourceAction.Craft:
                    stopAnim = true;

                    GameItem newItem = curRecipe.item;

                    // add item
                    item = new MMOInventoryItem
                    {
                        type = newItem.itemType,
                        amount = 1
                    };

                    TargetInventoryGain(item);

                    if (!inventory.addItem(item))
                    {
                        // drop on floor
                    }

                    // also remove items
                    inventory.removeRecipeMaterials(curRecipe);
                    break;

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

                            /*                            item = new MMOInventoryItem
                                                        {
                                                            type = newItem.itemType,
                                                            amount = 1
                                                        };

                                                        TargetInventoryGain(item);

                                                        if (!inventory.addItem(item))
                                                        {
                                                            // drop on floor
                                                        }
                            */
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
                                GameItem crop = MMOResourceManager.instance.getItem("CarrotPlant");

                                if (crop)
                                {
                                    Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                                    Quaternion rotation = new Quaternion();
                                    rotation.eulerAngles = new Vector3(0, 0, 0);

                                    GameObject obj = Instantiate(crop.prefab, pos, rotation);
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

                        item = new MMOInventoryItem
                        {
                            type = actionTarget.gameItem.itemType,
                            amount = 1
                        };

                        TargetInventoryGain(item);

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

                            item = new MMOInventoryItem
                            {
                                type = harvestItem.harvestResource,
                                amount = harvestItem.harvestGain
                            };

                            TargetInventoryGain(item);

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
                    case MMOResourceAction.Craft:
                    case MMOResourceAction.Gather:
                    case MMOResourceAction.Pickup:
                        serverobj.animator.SetBool("Gather", false);
                        break;
                    case MMOResourceAction.Plant:
                        serverobj.animator.SetBool("Plant", false);
                        break;
                    case MMOResourceAction.Chop:
                        serverobj.animator.SetBool("Chop", false);
                        break;
                    case MMOResourceAction.Mining:
                        serverobj.animator.SetBool("Mining", false);
                        break;
                }

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

        [TargetRpc]
        void TargetInventoryGain(MMOInventoryItem item)
        {
            UIManager.instance.showItemGain(item);

            isUserDirty = true;
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

        void changeActionItem(int idx)
        {
            Debug.Log("changeActionItem " + idx);
            CmdChangeActionItem(idx);
        }

        [Command]
        void CmdChangeActionItem(int idx)
        {
            Debug.Log("CmdChangeActionItem " + idx);

            HandleChangeActionItem(idx);
        }

        void HandleChangeActionItem(int idx)
        {
            Debug.Log("HandleChangeActionItem " + idx);
            inventory.changeItem(idx);

            GameItem activeItem = inventory.getActiveItem();

            if (activeItem)
            {
                switch (activeItem.actionType)
                {
                    case MMOResourceAction.Plant:
                        serverobj.displayItem = "Plow";
                        break;
                    case MMOResourceAction.Chop:
                        serverobj.displayItem = "axe";
                        break;
                    case MMOResourceAction.Mining:
                        serverobj.displayItem = "pickaxe";
                        break;
                }
            }
            else
            {
                serverobj.displayItem = "";
            }
        }

        public void moveInventory(int srcId, bool srcLoc, int destId, bool destLoc)
        {
            CmdMoveInventory(srcId, srcLoc, destId, destLoc);
        }

        [Command]
        void CmdMoveInventory(int srcId, bool srcLoc, int destId, bool destLoc)
        {
            HandleMoveInventory(srcId, srcLoc, destId, destLoc);
        }

        void HandleMoveInventory(int srcId, bool srcLoc, int destId, bool destLoc)
        {
            MMOInventoryItem src;
            MMOInventoryItem dest;

            if (!srcLoc)
                src = inventory.inventory[srcId];
            else
                src = inventory.actionBar[srcId];

            if (!destLoc)
                dest = inventory.inventory[destId];
            else
                dest = inventory.actionBar[destId];

            var type = src.type;
            var smount = src.amount;

            src.type = dest.type;
            src.amount = dest.amount;
            dest.type = type;
            dest.amount = smount;

            if (!srcLoc)
                inventory.inventory[srcId] = new MMOInventoryItem{
                        type = src.type,
                        amount = src.amount,
                        idx = src.idx,
                        action = src.action
                    };
            else
                inventory.actionBar[srcId] = new MMOInventoryItem
                    {
                        type = src.type,
                        amount = src.amount,
                        idx = src.idx,
                        action = src.action
                    };

            if (!destLoc)
                inventory.inventory[destId] = new MMOInventoryItem
                    {
                        type = dest.type,
                        amount = dest.amount,
                        idx = dest.idx,
                        action = dest.action
                    };
            else
                inventory.actionBar[destId] = new MMOInventoryItem
                    {
                        type = dest.type,
                        amount = dest.amount,
                        idx = dest.idx,
                        action = dest.action
                    };
        }
        #endregion
    }
}

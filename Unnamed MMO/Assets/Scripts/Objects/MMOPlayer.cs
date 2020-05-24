using Acemobe.MMO.Data;
using Acemobe.MMO.Data.ScriptableObjects;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using Rewired;
using Acemobe.MMO.UI;
using Acemobe.MMO.AI;
using UnityEngine.SceneManagement;
using System.Text;

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

        GameObject buildItem;
        GameObject highlightItem;
        public int buildAngle = 0;

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
            UIManager.instance.actionBarUI.gameObject.SetActive(true);

            mainCamera = MMOGameCamera.instance.GetComponent<Camera>();
        }

        enum GAME_LAYERS
        {
            PLANE = 8,
            MAIN_PLAYER = 9,
            PLAYERS = 10,
            MONSTES = 11,
            PROJECTILES = 12,
            OBJECTS = 13,
            TERRAIN = 14,
            GROUND_OBJECTS = 15,
            BUILDING = 16,
            SELECTED = 29,
            HIGHLIGHTED = 30
        };

        void Update()
        {
            if (isLocalPlayer)
            {
                if (isClient)
                {
                    Vector3 moveVector = new Vector3();
                    Vector3 cursorPos = new Vector3();
                    Vector3 forwardPos = new Vector3();
                    MMOObject mmoObj = null;
                    GameItem activeItem = inventory.getActiveItem();

                    Vector3 pos = transform.position + (transform.rotation * new Vector3(0, 0, 1));
                    forwardPos = new Vector3(Mathf.Floor(pos.x), 0, Mathf.Floor(pos.z));

                    moveVector.x = player.GetAxis("Move Horizontal"); // get input by name or action id
                    moveVector.y = player.GetAxis("Move Vertical");

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

                    if (player.GetButtonDown ("Customize"))
                    {
                        UIManager.instance.customizeUI.onShow();
                    }

                    if (player.GetButtonDown("Rotate"))
                    {
                        if (buildItem)
                        {
                            buildAngle = (buildAngle + 90) % 360;
                        }
                    }

                    // select inventory 1
                    if (player.GetButtonDown("Num1"))
                    {
                        TeleportPlayer(500, 0);
//                        changeActionItem(0);
                    }
                    else if (player.GetButtonDown("Num2"))
                    {
                        TeleportPlayer(0, 0);
//                        changeActionItem(1);
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
                        // send to server command mouse down
                        mouseDown(Mathf.FloorToInt(forwardPos.x), Mathf.FloorToInt(forwardPos.z));
                    }

                    // if not over UI
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        RaycastHit hit;
                        int layerMask = (1 << (int)GAME_LAYERS.TERRAIN) + (1 << (int)GAME_LAYERS.OBJECTS) + (1 << (int)GAME_LAYERS.GROUND_OBJECTS) + (1 << (int)GAME_LAYERS.BUILDING) + (1 << 30);
                        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                        GameObject gameObject;

                        // Does the ray intersect any objects excluding the player layer
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                        {
                            gameObject = hit.transform.gameObject;

                            if (gameObject)
                            {
                                mmoObj = gameObject.GetComponent<MMOObject>();

                                cursorPos.x = (int)Mathf.Floor(hit.point.x);
                                cursorPos.z = (int)Mathf.Floor(hit.point.z);
                            }
                        }

                        if (player.GetButtonDown("Fire"))
                        {
                            if (mmoObj != null)
                            {
                                mouseDownObject(mmoObj.gameObject);
                            }
                            else
                            {
                                mouseDown(Mathf.FloorToInt(cursorPos.x), Mathf.FloorToInt(cursorPos.z));
                            }
                        }
                        else if (player.GetButton("Fire"))
                        {
                            if (mmoObj != null)
                            {
                                mouseUpdateObject(mmoObj.gameObject);
                            }
                            else
                            {
                                mouseUpdate(Mathf.FloorToInt(cursorPos.x), Mathf.FloorToInt(cursorPos.z));
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

                        // show item if buildable item is selected
                        if (activeItem &&
                            activeItem.actionType == MMOResourceAction.Build &&
                            activeItem.itemType != MMOItemType.Hammer &&
                            inventory.hasItemCount(MMOItemType.Hammer, 1))
                        {
                            Vector3 targetPos = new Vector3(Mathf.FloorToInt(cursorPos.x) + 0.5f, 0, Mathf.FloorToInt(cursorPos.z) + 0.5f);
                            float dist = getDist(transform.position, targetPos);
                            if (dist >= mouseRange)
                            {
                                targetPos = new Vector3(Mathf.FloorToInt(forwardPos.x) + 0.5f, 0, Mathf.FloorToInt(forwardPos.z) + 0.5f);
                            }

                            // remove object if different
                            if (buildItem)
                            {
                                MMOBuilding building = buildItem.GetComponent<MMOBuilding>();

                                if (activeItem.itemType != building.gameItem.itemType)
                                {
                                    Destroy(buildItem);
                                    buildItem = null;
                                }
                            }

                            // create object
                            if (!buildItem)
                            {
                                Quaternion rotation = new Quaternion();
                                rotation.eulerAngles = new Vector3(0, buildAngle, 0);

                                buildItem = Instantiate(activeItem.prefab, targetPos, rotation);
                                buildItem.layer = (int)GAME_LAYERS.SELECTED;
                                MMOBuilding building = buildItem.GetComponent<MMOBuilding>();
                                building.disableColliders();
                            }

                            buildItem.transform.eulerAngles = new Vector3(0, buildAngle, 0);
                            buildItem.transform.position = targetPos;

                            // no highlight object
                            mmoObj = null;
                        }
                        else
                        {
                            // remove build item
                            if (buildItem)
                            {
                                Destroy(buildItem);
                                buildItem = null;
                            }
                        }

                        // highlight the object
                        if (highlightItem)
                        {
                            if (!mmoObj ||
                                (mmoObj && mmoObj.gameObject != highlightItem))
                            {
                                var outline = highlightItem.GetComponent<Outline>();
                                Destroy(outline);
                                highlightItem = null;
                            }
                        }

                        if (mmoObj)
                        {
                            if (!mmoObj.gameObject.GetComponent<Outline>())
                            {
                                var outline = mmoObj.gameObject.AddComponent<Outline>();
                                outline.OutlineMode = Outline.Mode.OutlineVisible;
                                outline.OutlineColor = Color.yellow;
                                outline.OutlineWidth = 10f;
                                highlightItem = mmoObj.gameObject;
                            }
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
            float dist = Mathf.Sqrt((dx * dx) + (dz * dz));

            return dist;
        }

        bool checkDist(Vector3 pos1, Vector3 pos2, float d)
        {
            float dx = pos1.x - pos2.x;
            float dz = pos1.z - pos2.z;
            float dist = (dx * dx) + (dz * dz);

            if (dist < (d * d))
                return true;

            return false;
        }

        #region Handle Actions
        void startAction(MMOObject obj)
        {
            GameItem activeItem = inventory.getActiveItem();
            MMOResourceAction curHeldAction = MMOResourceAction.None;
            Data.MapData.TerrainData terrainData;

            if (activeItem)
            {
                curHeldAction = activeItem.actionType;
            }

            if (obj)
            {
                int x = Mathf.FloorToInt(obj.transform.position.x);
                int z = Mathf.FloorToInt(obj.transform.position.z);

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

                    case MMOObjectTypes.Monster:
                        if (curHeldAction == MMOResourceAction.Attack)
                        {
                            curAction = MMOResourceAction.Attack;
                            actionTarget = obj;

                            Vector3 pos = actionTarget.transform.position;
                            pos.y = serverobj.transform.position.y;

                            serverobj.transform.LookAt(pos, Vector3.up);
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
                        else if (obj.gameItem.isBuildingBase)
                        {
                            startBuildAction (activeItem, x, z);
                        }
                        break;
                }
            }
            else
            {
                // plant crop
                if (curHeldAction == MMOResourceAction.Plant)
                {
                    startPlantAction(activeItem, actionX, actionZ);
                }
                // build item
                else if (curHeldAction == MMOResourceAction.Build)
                {
                    // if not hammer held
                    // must be building item
                    if (activeItem.itemType != MMOItemType.Hammer)
                    {
                        startBuildAction(activeItem, actionX, actionZ);
                    }
                }
                else if (curHeldAction == MMOResourceAction.Attack)
                {
                    Vector3 pos = new Vector3(actionX + 0.5f, transform.position.y, actionZ + 0.5f);
                    pos.y = serverobj.transform.position.y;

                    serverobj.transform.LookAt(pos, Vector3.up);
                }
            }

            // set animation
            switch (curAction)
            {
                case MMOResourceAction.Talk:
                    serverobj.animator.SetBool("Talk", true);
                    break;
                case MMOResourceAction.Build:
                    serverobj.animator.SetBool("Build", true);
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
                    serverobj.animator.SetBool("Mining", true);
                    break;
                case MMOResourceAction.Attack:
                    serverobj.animator.SetBool("Attack", true);
                    break;
            }
        }

        public void startPlantAction(GameItem activeItem, int x, int z)
        {
            Data.MapData.TerrainData terrainData = getLocalTerrainManager.getTerrainData(x, z);

            if (terrainData && 
                !terrainData.isInUse && 
                terrainData.canUse && 
                terrainData.isPublic)
            {
                curAction = MMOResourceAction.Plant;

                // look at
                Vector3 pos = new Vector3(actionX + 0.5f, transform.position.y, actionZ + 0.5f);
                Vector3 dir = (pos - transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(dir);
                serverobj.transform.rotation = rot;

                terrainData.isInUse = true;
            }
        }

        public void startBuildAction (GameItem activeItem, int x, int z)
        {
            Data.MapData.TerrainData terrainData = getLocalTerrainManager.getTerrainData(x, z);

            bool good = false;

            // if we require a base, is there a base
            if (activeItem.requiresBase)
            {
                if (activeItem.isWall)
                {
                    if (terrainData &&
                        terrainData.isPublic &&
                        terrainData.obj &&
                        terrainData.obj.gameItem.itemType == MMOItemType.Building_Base &&
                        !terrainData.hasWall(buildAngle))
                    {
                        good = true;
                    }
                }
            }
            else if (terrainData &&
                    terrainData.isPublic &&
                    !terrainData.obj)
            {
                good = true;
            }

            if (good)
            {
                // if activeItem is object
                curAction = MMOResourceAction.Build;

                actionX = x;
                actionZ = z;

                // look at
                Vector3 pos = new Vector3(actionX + 0.5f, transform.position.y, actionZ + 0.5f);
                Vector3 dir = (pos - transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(dir);
                serverobj.transform.rotation = rot;

                terrainData.isInUse = true;
            }

            // remove if we don't have it
            if (buildItem)
            {
                Destroy(buildItem);
                buildItem = null;
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

            switch (curAction)
            {
                case MMOResourceAction.Attack:
                    stopAnim = true;
                    break;

                case MMOResourceAction.Talk:
                    return;

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

                            getLocalTerrainManager.removeObjectAt(x, z);
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
                        Data.MapData.TerrainData terrainData = getLocalTerrainManager.getTerrainData(x, z);

                        if (terrainData && terrainData.isPublic && terrainData.obj == null)
                        {
                            float dist = getDist(transform.position, new Vector3(x + 0.5f, 0, z + 0.5f));

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

                                    getLocalTerrainManager.addObjectAt(x, z, spawnObj);
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

                case MMOResourceAction.Build:
                    GameItem activeItem = inventory.getActiveItem();
                    if (activeItem.itemType != MMOItemType.Hammer)
                    {
                        // create item
                        var x = actionX;
                        var z = actionZ;

                        // we hit empty ground?
                        Data.MapData.TerrainData terrainData = getLocalTerrainManager.getTerrainData(x, z);

                        if (terrainData && terrainData.isPublic)
                        {
                            float dist = getDist(transform.position, new Vector3(x + 0.5f, 0, z + 0.5f));
                            if (dist < mouseRange)
                            {
                                Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                                Quaternion rotation = new Quaternion();
                                bool canBuild = false;

                                if (activeItem.requiresBase)
                                {
                                    if (terrainData.obj && terrainData.obj.gameItem.itemType == MMOItemType.Building_Base)
                                    {
                                        rotation.eulerAngles = new Vector3(0, buildAngle, 0);
                                        canBuild = true;
                                    }
                                }
                                else if (terrainData.obj == null)
                                {
                                    canBuild = true;
                                }

                                if (canBuild)
                                {
                                    GameObject obj = Instantiate(activeItem.prefab, pos, rotation);
                                    MMOBuilding spawnObj = obj.GetComponent<MMOBuilding>();

                                    if (activeItem.requiresBase)
                                    {
                                        if (activeItem.isWall)
                                            getLocalTerrainManager.addWallAt(x, z, buildAngle, spawnObj);
                                    }
                                    else
                                        getLocalTerrainManager.addObjectAt(x, z, spawnObj);

                                    NetworkServer.Spawn(obj);

                                    inventory.removeItem(activeItem.itemType, 1);

                                    getLocalTerrainManager.islandMap.navMeshSurface.RemoveData();
                                    getLocalTerrainManager.islandMap.navMeshSurface.BuildNavMesh();
                                }
                            }

                            terrainData.isInUse = false;
                        }

                        // remove if we don't have it
                        if (buildItem)
                        {
                            Destroy(buildItem);
                            buildItem = null;
                        }

                        stopAnim = true;
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
                    case MMOResourceAction.Talk:
                        serverobj.animator.SetBool("Talk", false);
                        break;

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
                    case MMOResourceAction.Build:
                        serverobj.animator.SetBool("Build", false);
                        break;
                    case MMOResourceAction.Attack:
                        serverobj.animator.SetBool("Attack", false);
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
            MMOObject mmoObj = getLocalTerrainManager.getObjectAt (x, z);
            if (mmoObj && checkDist(transform.position, mmoObj.transform.position, mouseRange))
            {
                startAction(mmoObj);
            }
            else
            {
                // we hit empty ground?
                Data.MapData.TerrainData terrainData = getLocalTerrainManager.getTerrainData(x, z);

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

            MMOObject obj = getLocalTerrainManager.getObjectAt(x, z);

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
            bool clearBuilding = true;

            if (activeItem)
            {
                switch (activeItem.actionType)
                {
                    case MMOResourceAction.Plant:
                        serverobj.displayItem = "plow";
                        break;
                    case MMOResourceAction.Chop:
                        serverobj.displayItem = "axe";
                        break;
                    case MMOResourceAction.Mining:
                        serverobj.displayItem = "pickaxe";
                        break;
                    case MMOResourceAction.Attack:
                        serverobj.displayItem = "sword";
                        break;
                    case MMOResourceAction.Build:
                        if (activeItem.itemType == MMOItemType.Hammer ||
                            inventory.hasItemCount(MMOItemType.Hammer, 1))
                        {
                            serverobj.displayItem = "hammer";
                        }
                        else
                        {
                            serverobj.displayItem = "";
                        }
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

        void TeleportPlayer (int x, int z)
        {
            CmdTeleportPlayer(x, z);
        }

        [Command]
        void CmdTeleportPlayer(int x, int z)
        {
            HandleTeleportPlayer(x, z);
        }

        void HandleTeleportPlayer(int x, int z)
        {
            if (serverobj)
            {
                serverobj.navMeshAgent.Warp(new Vector3(x, 0, z));
                transform.position = new Vector3(x, 0, z);
            }
        }

        [Command]
        void CmdUpdateShirt(string shirt)
        {
            HandleUpdateShirt(shirt);
        }

        void HandleUpdateShirt(string shirt)
        {
            serverobj.topTex = shirt;

            RpcUpdateShirtClient(shirt);
        }

        [ClientRpc]
        void RpcUpdateShirtClient (string shirt)
        {
            if (serverobj && MMOPlayer.localPlayer != this)
            {
                serverobj.updateTop(shirt);
            }
        }
        #endregion

        public string shirt = "";
        public void updateShirtColor(int x, int y, Color color, int index)
        {
            if (shirt.Length == 0)
            {
                for (int x1 = 0; x1 < 64; x1++)
                {
                    for (int y1 = 0; y1 < 64; y1++)
                    {
                        shirt += "5";
                    }
                }
            }

            int pos = (x + 8) + (64 * ((64 - 6) - y));

            StringBuilder sb = new StringBuilder(shirt);
            sb[pos] = index.ToString()[0];
            shirt = sb.ToString();

            CmdUpdateShirt(shirt);

            serverobj.playerTopTexture.SetPixel(x + 8, (64 - 6) - y, color);
            serverobj.playerTopTexture.Apply();

            if (!serverobj.playerTopMaterial)
                serverobj.playerTopMaterial = serverobj.playerTop.material;
            serverobj.playerTopMaterial.mainTexture = serverobj.playerTopTexture;
        }

        #region Terrain Manager Finder
        MMOTerrainManager terrainManager;

        MMOTerrainManager getLocalTerrainManager
        {
            get
            {
                if (terrainManager)
                    return terrainManager;

                foreach (var t in MMOTerrainManager.terrains)
                {
                    if (t.Value.worldBounds.Contains (transform.position))
                    {
                        terrainManager = t.Value;
                        break;
                    }
                }

                return terrainManager;
            }
        }
        #endregion


    }
}

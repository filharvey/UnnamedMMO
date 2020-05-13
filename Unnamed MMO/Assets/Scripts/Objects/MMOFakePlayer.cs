using Acemobe.MMO.Data;
using Acemobe.MMO.UI;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Acemobe.MMO.Objects
{
    public class MMOFakePlayer : NetworkBehaviour
    {
        [Header("Components")]
        public GameObject owner;
        public Rigidbody rigidBody;
        public GameObject mesh;
        public Animator animator;
        public NavMeshAgent navMeshAgent;

        [Header("Params")]
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

        [SyncVar]
        public float health;

        [SyncVar]
        public bool isFiring = false;
        [SyncVar]
        public bool isMoving = false;

        [SyncVar]
        public float lastFire = 0f;
        [SyncVar]
        public float moveHoriz = 0f;
        [SyncVar]
        public float moveVert = 0f;
        [SyncVar]
        public float lookAngle = 0f;

        [SyncVar]
        public string displayItem;

        [SyncVar]
        public string topTex = "";

        Transform activeTool;

        public float cameraRotation = 0;

        public float speed = 300;

        [SyncVar]
        public uint netIdOwner;

        public Dictionary<string, Transform> weapons = new Dictionary<string, Transform>();

        public MMOPlayer player;

        public SkinnedMeshRenderer playerTop;
        public Material playerTopMaterial;
        public Texture2D playerTopTexture;

        public override void OnStartServer()
        {
            base.OnStartServer();

            rigidBody.isKinematic = false;

            Debug.Log("OnStartServer," + torso + "," + head);
            handleMeshStartup();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            // find parent object
            Debug.Log("OnStartClient," + torso + "," + this.head);

            if (isClientOnly)
            {
                handleMeshStartup();
            }

            // set the top texture
            if (topTex != "")
            {
                updateTop(topTex);
            }
        }

        private void OnDestroy()
        {
            if (playerTopMaterial)
            {
                Destroy(playerTopMaterial);
            }
        }

        void handleMeshStartup ()
        {
            /*            for (var a = 0; a < mesh.transform.childCount; a++)
                        {
                            var child = mesh.transform.GetChild(a);

                            if (child.gameObject.name.Contains("Torso") ||
                                child.gameObject.name.Contains("Bottom") ||
                                child.gameObject.name.Contains("Feet") ||
                                child.gameObject.name.Contains("Hand") ||
                                child.gameObject.name.Contains("Belt"))
                            {
                                child.gameObject.SetActive(false);
                            }
                        }

                        string _head = "01 Head " + this.head.ToString("D2");
                                    string _torso = "02 Torso " + this.torso.ToString("D2");
                                    string _bottom = "03 Bottom " + this.bottom.ToString("D2");
                                    string _feet = "04 Feet " + this.feet.ToString("D2");
                                    string _hand = "05 Hand " + this.hand.ToString("D2");
                                    string _belt = "06 Belt " + this.belt.ToString("D2");

                                    Transform head = mesh.transform.Find (_head);
                                    Transform body = mesh.transform.Find (_torso);
                                    Transform bottom = mesh.transform.Find (_bottom);
                                    Transform feet = mesh.transform.Find (_feet);
                                    Transform hands = mesh.transform.Find (_hand);
                                    Transform belt = mesh.transform.Find (_belt);
                                                            
                                    if (body)
                                    {
                                        body.gameObject.SetActive(true);
                                        SkinnedMeshRenderer skin = body.GetComponent<SkinnedMeshRenderer>();
                                    }

                                    if (bottom)
                                    {
                                        bottom.gameObject.SetActive(true);
                                    }

                                    if (feet)
                                    {
                                        feet.gameObject.SetActive(true);
                                    }

                                    if (hands)
                                    {
                                        hands.gameObject.SetActive(true);
                                    }

                                    if (belt)
                                    {
                                        belt.gameObject.SetActive(true);
                                    }

             */
  
            weapons.Add("pickaxe", mesh.transform.Find("RigPelvis/RigSpine1/RigSpine2/RigSpine3/RigRibcage/RigRCollarbone/RigRUpperarm/RigRForearm/RigRPalm/+R Hand/VillagerPickaxe"));
            weapons.Add("plow", mesh.transform.Find("RigPelvis/RigSpine1/RigSpine2/RigSpine3/RigRibcage/RigRCollarbone/RigRUpperarm/RigRForearm/RigRPalm/+R Hand/VillagerPlow"));
            weapons.Add("axe", mesh.transform.Find("RigPelvis/RigSpine1/RigSpine2/RigSpine3/RigRibcage/RigRCollarbone/RigRUpperarm/RigRForearm/RigRPalm/+R Hand/VillagerAxe"));
            weapons.Add("hammer", mesh.transform.Find("RigPelvis/RigSpine1/RigSpine2/RigSpine3/RigRibcage/RigRCollarbone/RigRUpperarm/RigRForearm/RigRPalm/+R Hand/VillagerHammer"));
            weapons.Add("sword", mesh.transform.Find("RigPelvis/RigSpine1/RigSpine2/RigSpine3/RigRibcage/RigRCollarbone/RigRUpperarm/RigRForearm/RigRPalm/+R Hand/Sword 02A Blue"));

            foreach (var weapon in weapons)
            {
                weapon.Value.gameObject.SetActive(false);
            }

            playerTopTexture = new Texture2D(64, 64, TextureFormat.ARGB32, false);
            playerTopTexture.filterMode = FilterMode.Point;
        }

        void FixedUpdate()
        {
            if (isClient)
            {
                // make sure localPlayer is set
                if (MMOPlayer.localPlayer &&
                    MMOPlayer.localPlayer.serverobj == null &&
                    MMOPlayer.localPlayer.netId == netIdOwner)
                {
                    MMOPlayer.localPlayer.serverobj = this;

                    UILogin.instance.gameObject.SetActive(false);
                }

                // display item if they are using one
                if (displayItem != "") 
                {
                    if (activeTool != weapons[displayItem])
                    {
                        if (activeTool != null)
                        {
                            activeTool.gameObject.SetActive(false);
                        }

                        weapons[displayItem].gameObject.SetActive(true);
                        activeTool = weapons[displayItem];
                    }
                }
                else if (activeTool)
                {
                    activeTool.gameObject.SetActive(false);
                    activeTool = null;
                }
            }

            if (isServer)
            {
                movePlayer(Time.fixedDeltaTime);
                mesh.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                rigidBody.angularVelocity = Vector3.zero;
            }
        }

        public void movePlayer(float dt)
        {
            if (player && player.curAction != MMOResourceAction.None)
                return;

            Quaternion quatUp = new Quaternion();
            Quaternion quatLeft = new Quaternion();
            quatUp.eulerAngles = new Vector3(0, cameraRotation - 90, 0);
            quatLeft.eulerAngles = new Vector3(0, cameraRotation, 0);

            Vector3 up = quatUp * Vector3.forward;
            Vector3 left = quatLeft * Vector3.forward;

            Vector3 move = (up * moveVert) + (left * moveHoriz);
            move.Normalize();
            move *= (speed * Time.fixedDeltaTime);

            bool newMove = (move != Vector3.zero);

            // send move flag
            if (isMoving != newMove)
            {
                isMoving = newMove;
                animator.SetBool("Walk", isMoving);
            }

            var rotation = transform.rotation;
            if (isMoving)
            {
                float angle = Mathf.Atan2(-moveHoriz, -moveVert) * 180 / Mathf.PI;
                rotation.eulerAngles = new Vector3(0, angle + 90, 0);
            }

            navMeshAgent.velocity = move;

            rigidBody.angularVelocity = Vector3.zero;
            transform.rotation = rotation;
        }

        [ServerCallback]
        void OnTriggerEnter(Collider co)
        {
            health -= 10;
        }

        public void AnimComplete()
        {
            if (isServer)
            {
                if (player)
                {
                    player.AnimComplete();
                }
            }
        }

        public void updateTop (string shirt)
        {
            Color color = Color.white;

            for (int a = 0; a < shirt.Length; a++)
            {
                int x = a % 64;
                int y = Mathf.FloorToInt(a / 64);

                var index = shirt[a];
                switch (index)
                {
                    case '0':
                        color = Color.red;
                        break;
                    case '1':
                        color = Color.blue;
                        break;
                    case '2':
                        color = Color.yellow;
                        break;
                    case '3':
                        color = Color.green;
                        break;
                    case '4':
                        color = Color.cyan;
                        break;
                    case '5':
                        color = Color.white;
                        break;
                    case '6':
                        color = Color.black;
                        break;
                    case '7':
                        color = Color.grey;
                        break;

                }

                playerTopTexture.SetPixel(x, y, color);
            }

            playerTopTexture.Apply();

            // update material
            if (!playerTopMaterial)
                playerTopMaterial = playerTop.material;

            playerTopMaterial.mainTexture = playerTopTexture;
        }
    }
}